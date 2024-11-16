using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using api.Data.DB;
using api.Helpers;
using api.Models;
using auth.Core.BL.Interfaces;
using auth;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace auth.Core.BL.Services;

public class AuthServices : IAuth
{
    public Task<string?> Login(string username, string password)
    {
        using (var connection = new InitDb())
        {
            var query = (from item in connection.Users
                    where item.Username == username
                    select item
                ).FirstOrDefault();
            if (query != null)
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password!,
                    salt: query.Salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                if (query.Password == hashed)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Settings.Key;
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.Name, query.Username),
                        }),
                        Expires = DateTime.UtcNow.AddHours(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                            SecurityAlgorithms.HmacSha256Signature),
                        Issuer = Settings.Issuer,
                        Audience = Settings.Issuer
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    return Task.FromResult<string?>(tokenHandler.WriteToken(token));   
                }
                return Task.FromResult<string>(null);

            }

            return Task.FromResult<string>(null);
        }
    }

    public Task<bool> Register(string username, string password)
    {
        bool result = false;
        using (var connection = new InitDb())
        {
            var query = (
                from item in connection.Users
                where item.Username == username
                select item
            ).FirstOrDefault();

            if (query == null)
            {   
                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                
                Users usr = new Users();
                usr.Username = username;
                usr.Password = hashed;
                usr.Salt = salt;
                connection.Users.Add(usr);
                result = connection.SaveChanges() > 0;
            }
        } 
        return Task.FromResult(result);
    }

    public Task<List<Users>> List()
    {
        using (var connection = new InitDb() )
        {
            var query = (from item in connection.Users
                    select item
                ).ToList();
            
            return Task.FromResult(query);
        }

    }
}