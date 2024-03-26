using Aukcionas.Auth.Model;
using Aukcionas.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Aukcionas.Models;
using Aukcionas.Utils;
using Aukcionas.Data;
using Microsoft.EntityFrameworkCore;


namespace Aukcionas.Controllers
{

    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ForumRestUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly DataContext _dataContext;
        private readonly ILogger<AuthController> _logger;
        private readonly PasswordHasher<IdentityUser> _passwordHasher;

        public AuthController(UserManager<ForumRestUser> userManager, ILogger<AuthController> logger, IJwtTokenService jwtTokenService, IConfiguration configuration,IEmailService emailservice, DataContext dataContext)
        {
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
            _emailService = emailservice;
            _dataContext = dataContext;
            _passwordHasher = new PasswordHasher<IdentityUser>();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(registerUserDto.UserName))
                {
                    return BadRequest("Username cannot be empty");
                }
                var user = await _userManager.FindByNameAsync(registerUserDto.UserName);
                if (user != null)
                {
                    return BadRequest(error: "User already exists");
                }
                user = await _userManager.FindByEmailAsync(registerUserDto.Email);
                if (user != null)
                {
                    return BadRequest(error: "Email already taken");
                }
                var newUser = new ForumRestUser
                {
                    Email = registerUserDto.Email,
                    UserName = registerUserDto.UserName
                };

                var createUserResult = await _userManager.CreateAsync(newUser, registerUserDto.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to create user: {errors}");
                }
                await _userManager.AddToRoleAsync(newUser, ForumRoles.ForumUser);
                var userDto = new UserDto(newUser.Id, newUser.UserName, newUser.Email);

                var tokenLifespan = TimeSpan.FromDays(7);
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var userToken = await _userManager.FindByEmailAsync(newUser.Email);
                userToken.EmailConfirmationToken = emailToken;
                userToken.EmailConfirmationTokenExpiry = DateTime.Now.Add(tokenLifespan);
                _dataContext.Entry(userToken).State = EntityState.Modified;
                await _dataContext.SaveChangesAsync();
                var emailModel = new EmailModel(newUser.Email, "Confirm your email", EmailBody.EmailStringConfirm(newUser.Email, emailToken),"Confirm email address");
                _emailService.SendEmail(emailModel);

                return CreatedAtAction(nameof(Register), userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration");
                return StatusCode(500, "An unexpected error occurred");
            }
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    return BadRequest(error: "Invalid username or pasword.");
                }
                if (!user.EmailConfirmed)
                {
                    return BadRequest(error: "Confirm your email address before logging in.");
                }
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);

                return Ok(new SuccessfulLoginDto(accessToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during user registration");
                return StatusCode(500, "An unexpected error occurred");
            }

        }

        [HttpPut]
        [Route("update")]
        [Authorize(Roles = "Admin, ForumUser")]
        public async Task<IActionResult> UpdateUserInfo(ForumRestUser updatedUserInfo)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.Email = updatedUserInfo.Email;
                user.Name = updatedUserInfo.Name;
                user.UserName = updatedUserInfo.UserName;
                user.PhoneNumber = updatedUserInfo.PhoneNumber;
                user.Surname = updatedUserInfo.Surname;
                user.Paypal_Email = updatedUserInfo.Paypal.GetValueOrDefault() ? updatedUserInfo.Paypal_Email : null;
                user.Account_Holder_Name = updatedUserInfo.Bank.GetValueOrDefault() ? updatedUserInfo.Account_Holder_Name : null;
                user.Account_Number = updatedUserInfo.Bank.GetValueOrDefault() ? updatedUserInfo.Account_Number : null;
                user.Bank_Name = updatedUserInfo.Bank.GetValueOrDefault() ? updatedUserInfo.Bank_Name : null;
                user.Bic_Swift_Code = updatedUserInfo.Bank.GetValueOrDefault() ? updatedUserInfo.Bic_Swift_Code : null;
                user.Paypal = updatedUserInfo.Paypal.GetValueOrDefault() ? true : null;
                user.Bank = updatedUserInfo.Bank.GetValueOrDefault() ? true : null;

                var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return Ok("User information updated successfully");
                }
                else
                {
                    _logger.LogError("User information update failed. Errors: {errors}", result.Errors);
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating user information: {exception}", ex);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete]
        [Route("user/{id}")]
        [Authorize(Roles = "Admin, ForumUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest("Delete user error");
            }

            return NoContent();
        }
        [HttpGet]
        [Route("userinfo")]
        [Authorize(Roles = "Admin, ForumUser")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var userInfo = new ForumRestUser
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Auctions_Won = user.Auctions_Won,
                Liked_Auctions = user.Liked_Auctions,
                Can_Bid = user.Can_Bid,
                Paypal = user.Paypal,
                Bank = user.Bank,
                Bank_Name = user.Bank_Name,
                Bic_Swift_Code = user.Bic_Swift_Code,
                Account_Number = user.Account_Number,
                Account_Holder_Name = user.Account_Holder_Name,
            };

            return Ok(new { userInfo });
        }
        [HttpPost]
        [Route("send-reset-email/{email}")]
        [Authorize(Roles = "Admin, ForumUser")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Email does not exist"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordToken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _configuration["EmailConfiguration:From"];
            var emailModel = new EmailModel(email,"Reset password", EmailBody.EmailStringReset(email,emailToken), "Reset your password");
            _emailService.SendEmail(emailModel);
            _dataContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _dataContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent"
            });
        }
        [HttpPost]
        [Route("reset-password")]
        [Authorize(Roles = "Admin, ForumUser")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var newToken = resetPasswordDto.EmailToken.Replace(" ", "+");
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User dosn't exist"
                });
            }
            var tokenCode = user.ResetPasswordToken;
            DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid password reset link"
                });
            }
            user.PasswordHash = _passwordHasher.HashPassword(user, resetPasswordDto.NewPassword);
            _dataContext.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _dataContext.SaveChangesAsync();
            return Ok(new
            {
                StatusCode = 200,
                Message="Password was reset"
            });
        }
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string code)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                code = code.Replace(" ", "+");
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    return Ok("Email confirmed successfully.");
                }

                return BadRequest("Failed to confirm email.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during email confirmation");
                return StatusCode(500, "An unexpected error occurred");
            }
        }

    }
}
