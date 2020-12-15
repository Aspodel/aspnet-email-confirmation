using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailConfirmation.Api.Models;
using EmailConfirmation.Api.Models.DTOs;
using EmailConfirmation.Api.Services;
using EmailConfirmation.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EmailConfirmation.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private readonly JwTokenConfig _jwTokenConfig;
        private IEmailSender _emailSender;
        private IConfiguration _configuration;


        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, IOptions<JwTokenConfig> jwTokenConfig,IEmailSender emailSender,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwTokenConfig = jwTokenConfig.Value;
            _emailSender = emailSender;
            _configuration = configuration;
        }


        [HttpPost]
        public async Task<Object> Register(RegisterDTO dTO)
        {
            var user = new User()
            {
                UserName = dTO.Email,
                Email = dTO.Email,
            };
            try
            {
                var result = await _userManager.CreateAsync(user, dTO.Password);

                if (result.Succeeded)
                {
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var validEmailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmEmailToken));

                    string confirmUrl = $"{_configuration["ApiUrl"]}/api/user/ConfirmEmail?userid={user.Id}&token={validEmailToken}";

                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email", "<h1>Hello, this is my demo</h1>" + $"<p>Please confirm your email by <a href=`{confirmUrl}`>Click here</a></p>");

                    return Ok(result);
                }

                return BadRequest(result);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IActionResult> Login(LoginDTO dTO)
        {
            var user = await _userManager.FindByNameAsync(dTO.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, dTO.Password))
            {
                //await _emailSender.SendEmailAsync(dTO.UserName, "New login", "<h1>New login to your account noticed</h1><p>New login at " + DateTime.Now + "</p>");
                return Ok();
            }
            else
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                return NotFound();

            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");


            var decodedToken = WebEncoders.Base64UrlDecode(token);
            string normalToken = Encoding.UTF8.GetString(decodedToken);
            var result = await _userManager.ConfirmEmailAsync(user, normalToken);

            if (result.Succeeded)
                return Ok("Email confirmed successfully");

            return BadRequest(result);
        }
    }
}
