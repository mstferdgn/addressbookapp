using AddressBookEL.IdentityModels;
using AddressBookEL.JWTModels;
using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AddressBookAPI.JWTProcess
{
    public class TokenManager : ITokenManager
    {
       private readonly IConfiguration _configuration; //appsettings.json

        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<UserLoginResponse> GenerateToken(AppUser user)
        {
            try
            {
                SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTSettings:Secret"]));

                var dateTimeNow = DateTime.UtcNow;

                JwtSecurityToken jwt = new JwtSecurityToken(
                        issuer: _configuration["JWTSettings:ValidIssuer"],
                        audience: _configuration["JWTSettings:ValidAudience"],
                        
                        claims: new List<Claim> {

                    new Claim("nameSurname", $"{user.Name} {user.Surname}"),
                    new Claim("username", $"{user.UserName}"),
                    new Claim("gender", $"{user.Gender}"),
                    new Claim("profilepic", $"{user.ProfilePicture}")
                    
                        },
                        notBefore: dateTimeNow,
                        expires: dateTimeNow.Add(TimeSpan.FromMinutes(2)),
                        signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
                    );

                return Task.FromResult(new UserLoginResponse
                {
                    AuthToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                    AccessTokenExpireDate = dateTimeNow.Add(TimeSpan.FromMinutes(2)),
                    AuthenticateResult=true
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
