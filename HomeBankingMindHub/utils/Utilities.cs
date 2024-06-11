using HomeBankingMindHub.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HomeBankingMindHub.utils
{
    public class Utilities
    {
        private readonly IConfiguration _configuration;
        public Utilities(IConfiguration configuration)
        {
             _configuration = configuration;
        }

        public string generateJWT(Client client)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, client.Email.ToString()),
                new Claim("Client",client.Email)
            };

            if (client.Email == "kobe23@gmail.com")
            {
                userClaims.Add(new Claim("Admin", client.Email));
            }

            var claimsIdentity = new ClaimsIdentity(userClaims, JwtBearerDefaults.AuthenticationScheme);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var credentias = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new SecurityTokenDescriptor()
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = credentias
            };

            var token = new JwtSecurityTokenHandler().CreateToken(jwtConfig);

            return new JwtSecurityTokenHandler().WriteToken(token);


        }

    }
}
