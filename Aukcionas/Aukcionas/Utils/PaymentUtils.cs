using Aukcionas.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public int DecodePaymentToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            var auctionIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "AuctionId");
            if (auctionIdClaim != null && int.TryParse(auctionIdClaim.Value, out int auctionId))
            {
                return auctionId;
            }
            else
            {
                throw new ArgumentException("Invalid auction ID or token is expired");
            }
        }

    }
}
