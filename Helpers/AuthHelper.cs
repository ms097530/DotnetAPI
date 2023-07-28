using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        IConfiguration _config;
        public AuthHelper(IConfiguration config)
        {
            _config = config;
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
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

        public string CreateToken(int userId)
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
