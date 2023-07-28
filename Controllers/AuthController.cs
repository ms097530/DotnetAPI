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

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
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
                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        // * populates passed byte array with random array of bytes
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

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
            byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

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
                {"token", CreateToken(userId)}
            });
        }

        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            // * token will be accessible via [Authorize] attribute on controller
            // * see if user id is valid on token -> if valid, send new token back to user
            string userIdSql = $"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = {User.FindFirst("userId")?.Value}";

            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return CreateToken(userId);
        }

        private byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            // * retrieve value for PasswordKey from app settings and combine with salt
            // ? using this scheme, password only lives in application... what is stored in DB will be password + key + salt that gets hashed
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                // * convert back to byte array
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );

            return passwordHash;
        }

        private string CreateToken(int userId)
        {
            // * Claim is stored in token so that it can be accessed in frontend and backend
            Claim[] claims = new Claim[] {
                new Claim("userId", userId.ToString())
            };

            // * create key -> create signer -> create token builder -> pass all to token builder
            // ? SymmetricSecurityKey ctor requires byte array -> get key from appsettings and convert to byte array
            string? tokenKeyString = _config.GetSection("Appsettings:TokenKey").Value;
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                ));

            // * signs token
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            // * setup descriptor for token
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            // * has methods to turn descriptor into actual token we can pass to user
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            // * token will come back in form of SecurityToken
            SecurityToken token = tokenHandler.CreateToken(descriptor);

            // * convert token into string (so it can be a universal format) and return it
            return tokenHandler.WriteToken(token);
        }
    }
}