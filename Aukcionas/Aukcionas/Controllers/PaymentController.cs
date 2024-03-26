using Aukcionas.Auth;
using Aukcionas.Auth.Model;
using Aukcionas.Data;
using Aukcionas.Models;
using Aukcionas.Services;
using Aukcionas.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using System.Text.Json;

namespace Aukcionas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<ForumRestUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly HttpClient _httpClient;

        public PaymentController(UserManager<ForumRestUser> userManager, IConfiguration configuration, IEmailService emailservice, DataContext dataContext, HttpClient httpClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _emailService = emailservice;
            _dataContext = dataContext; ;
            _httpClient = httpClient;
        }

        [HttpPost("decode")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<DecodedPaymentToken> DecodeToken([FromBody] string token)
        {
            try
            {
                var paymentUtils = new PaymentUtils(_configuration);
                var decodedTokenResult = paymentUtils.DecodePaymentToken(token);
                var decodedToken = new DecodedPaymentToken();
                decodedToken.AuctionId = decodedTokenResult.auctionId;
                decodedToken.UserId = decodedTokenResult.userId;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == decodedTokenResult.userId)
                {
                    return Ok(decodedToken);
                }
                else
                {
                    return Forbid();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to decode token: {ex.Message}");
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = ForumRoles.ForumUser)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Payment>> CreatePayment([FromBody] Payment payment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                var auction = _dataContext.Auctions.FirstOrDefault(a => a.id.ToString() == payment.Auction_Id);
                var auction_Owner = await _userManager.FindByIdAsync(auction.username);
                if (user == null || auction == null || auction_Owner == null)
                {
                    return BadRequest("Not found (Buyer,Owner or Auction)");
                }
                auction.is_Paid = true;
                payment.Buyer_Id = userId;
                payment.Buyer_Email = user.Email;
                if (auction_Owner.Paypal ?? false)
                {
                    payment.Auction_Owner_Email = auction_Owner.Paypal_Email;
                    payment.Payout_Successful = await PayoutToPaypalAccount(payment);
                }
                else if (auction_Owner.Bank ?? false)
                {
                    payment.Auction_Owner_Email = auction_Owner.Email;
                    payment.Payout_Successful = await PayoutToBankAccount(payment, auction_Owner);
                }
                else
                {
                    return BadRequest("Auction owner has not provided payout information.");
                }
                payment.Payment_Currency = "EUR";
                payment.Payout_Currency = payment.Payment_Currency;
                payment.Payout_Amount= payment.Payment_Amount;
                payment.Payout_Time = DateTime.Now;
                _dataContext.Payments.Add(payment);
                _dataContext.SaveChanges();

                string from = _configuration["EmailConfiguration:From"];
                var emailModel = new EmailModel(user.Email, "Payment information", EmailBody.EmailAuctionWinnerOnPayment(from, payment), "Payment information");
                _emailService.SendEmail(emailModel);

                var emailModel2 = new EmailModel(auction_Owner.Email, "Successful payment on you'r auction", EmailBody.EmailAuctionOwnerOnPayment(from, payment), "Shipping information");
                _emailService.SendEmail(emailModel2);

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }

        }
        private async Task<bool> PayoutToPaypalAccount(Payment payment)
        {
            try
            {
                string accessToken = await GetAccessToken();

                var payoutRequest = new
                {
                    sender_batch_header = new
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        recipient_type = "EMAIL",
                        email_subject = "Payment from Your Auction",
                        email_message = "You have received a payment from your auction.",
                        note = "Note"
                    },
                    items = new[]
                    {
                        new
                        {
                            recipient_type = "EMAIL",
                            receiver = payment.Auction_Owner_Email,
                            amount = new
                            {
                                value = payment.Payment_Amount,
                                currency = payment.Payment_Currency
                            }
                        }
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payoutRequest);
                var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.PostAsync("https://api.sandbox.paypal.com/v1/payments/payouts", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Failed to process PayPal payout. Status Code: {response.StatusCode}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to process PayPal payout.", ex);
            }
        }
        private async Task<bool> PayoutToBankAccount(Payment payment, ForumRestUser auctionOwner)
        {
            try
            {
                if (string.IsNullOrEmpty(auctionOwner.Account_Holder_Name) ||
                    string.IsNullOrEmpty(auctionOwner.Account_Number) ||
                    string.IsNullOrEmpty(auctionOwner.Bank_Name) ||
                    string.IsNullOrEmpty(auctionOwner.Bic_Swift_Code))
                {
                    Console.WriteLine("Bank account information is missing. Payout cannot be processed.");
                    return false;
                }

                string accessToken = await GetAccessToken();

                var payoutRequest = new
                {
                    sender_batch_header = new
                    {
                        sender_batch_id = Guid.NewGuid().ToString(),
                        email_subject = "Payment from Your Auction",
                        email_message = "You have received a payment from your auction."
                    },
                    items = new[]
                    {
                new
                {
                    recipient_type = "EMAIL",
                    recipient_name = auctionOwner.Account_Holder_Name,
                    recipient_bank_account = auctionOwner.Account_Number,
                    recipient_bank_name = auctionOwner.Bank_Name,
                    recipient_bank_swift = auctionOwner.Bic_Swift_Code,
                    amount = new
                    {
                        value = payment.Payment_Amount,
                        currency = payment.Payment_Currency
                    }
                }
            }
                };
                var jsonPayload = JsonSerializer.Serialize(payoutRequest);
                var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await _httpClient.PostAsync("https://api.sandbox.paypal.com/v1/payments/payouts", requestContent);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to transfer funds to bank account. PayPal API response: {response.ReasonPhrase}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to process PayPal payout.", ex);
            }
        }

        private async Task<string> GetAccessToken()
        {
            try
            {
                var clientId = _configuration["PayPal:ClientIdSandbox"];
                var clientSecret = _configuration["PayPal:ClientSecretSandbox"];

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                        "Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"))
                    );

                    var requestBody = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    };

                    var requestContent = new FormUrlEncodedContent(requestBody);
                    var response = await client.PostAsync("https://api.sandbox.paypal.com/v1/oauth2/token", requestContent);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var tokenResponse = JsonSerializer.Deserialize<PayPalAccessToken>(responseContent);

                        return tokenResponse?.access_token;
                    }
                    else
                    {

                        Console.WriteLine($"Failed to get access token. Status Code: {response.StatusCode}");
                        return response.StatusCode.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get access token.", ex);
            }
        }
    }
}
