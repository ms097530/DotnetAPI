using System.Text;
using System.Security.Cryptography;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using DotnetAPI.Helpers;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        /* 
            request will expect object with form of
            {
                string Email,
                string Password,
                string PasswordConfirm
            }
        */
        public IActionResult Register(UserForRegistrationDTO userForRegistration)
        {
            // * want to make sure user's PW and confirmation match before sending to DB
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = $"SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '{userForRegistration.Email}';";

                // * looking for one piece of data, but use LoadData instead of LoadDataSingle so that we don't have to model object used only for this purpose
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);

                // * for successful registration, want no users to be returned
                // * now we know email is available, AND that passwords match
                if (existingUsers.Any())
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        // * populates passed byte array with random array of bytes
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    // ? using SQL variables (i.e. @PasswordHash)
                    // * NOTE: using ""{...}"" where double quotes are doubled to escape in string
                    string sqlAddAuth = @$"INSERT INTO TutorialAppSchema.Auth (
                            [Email],
                            [PasswordHash],
                            [PasswordSalt]
                        ) VALUES (
                            '{userForRegistration.Email}',
                            @PasswordHash,
                            @PasswordSalt
                        )";

                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    // ! NOTE: using passwordSalt for value, NOT passwordSaltPlusString
                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;
                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    sqlParameters.Add(passwordSaltParameter);
                    sqlParameters.Add(passwordHashParameter);

                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                    {
                        // * using 1 in place of provided value for Active since we don't want to create users that start as inactive
                        string sqlAddUser = @$"
                            INSERT INTO TutorialAppSchema.Users(
                                [FirstName],
                                [LastName],
                                [Email],
                                [Gender],
                                [Active]
                            ) VALUES (
                                '{userForRegistration.FirstName}',
                                '{userForRegistration.LastName}',
                                '{userForRegistration.Email}',
                                '{userForRegistration.Gender}',
                                '1'
                            )
                        ";

                        if (_dapper.ExecuteSql(sqlAddUser))
                        {

                            return Ok();
                        }
                        throw new Exception("Unable to add user");
                    }

                    throw new Exception("Failed to register user");
                }

                throw new Exception("User with this email already exists");
            }

            throw new Exception("Passwords do not match");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {
            string sqlForHashAndSalt = @$"SELECT [PasswordHash],
                [PasswordSalt] 
                FROM TutorialAppSchema.Auth 
                WHERE Email = '{userForLogin.Email}'";

            UserForLoginConfirmationDTO userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDTO>(sqlForHashAndSalt);

            // * userForLogin provides password -> get hash using provided password and stored salt for user (userForConfirmation) -> compare hashes
            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            // * providing same password and salt to hashing algorithm should provide matching results -> if they match, login user
            // ? can't directly compare results because they are arrays of bytes (pointer)
            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Credentials incorrect");
                }
            }

            // * made it through loop - credentials matched!
            // * get userId of user attempting to login using provided email
            string userIdSql = $"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '{userForLogin.Email}'";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string> {
                {"token", _authHelper.CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            // * token will be accessible via [Authorize] attribute on controller
            // * see if user id is valid on token -> if valid, send new token back to user
            string userIdSql = $"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = {User.FindFirst("userId")?.Value}";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }

    }
}