using Aukcionas.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace Aukcionas.Utils
{
    public class PaymentUtils
    {
        private readonly IConfiguration _configuration;

        public PaymentUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GeneratePaymentToken(string userId,int auction_Id)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId), new Claim("AuctionId", auction_Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public (string userId, int auctionId) DecodePaymentToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                var auctionIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "AuctionId")?.Value;

                if (userIdClaim == null || auctionIdClaim == null || !int.TryParse(auctionIdClaim, out int auctionId))
                {
                    throw new ArgumentException("Invalid user ID, auction ID, or token is expired");
                }

                return (userIdClaim, auctionId);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Failed to decode token", ex);
            }
        }


    }
}
