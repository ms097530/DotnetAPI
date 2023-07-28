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

namespace DotnetAPI.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }

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

                    // * retrieve value for PasswordKey from app settings and combine with salt
                    // ? using this scheme, password only lives in application... what is stored in DB will be password + key + salt that gets hashed
                    string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value + Convert.ToBase64String(passwordSalt);

                    byte[] passwordHash = KeyDerivation.Pbkdf2(
                        password: userForRegistration.Password,
                        // * convert back to byte array
                        salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8
                    );

                    // ? using SQL variables (i.e. @PasswordHash)
                    string sqlAddAuth = @$"INSERT INTO TutorialAppSchema.Auth (
                            [Email],
                            [PasswordHash],
                            [PasswordSalt]
                        ) VALUES(
                            {userForRegistration.Email},
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

                        return Ok();
                    }

                    throw new Exception("Failed to register user");
                }

                throw new Exception("User with this email already exists");
            }

            throw new Exception("Passwords do not match");
        }

        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin)
        {
            return Ok();
        }
    }
}