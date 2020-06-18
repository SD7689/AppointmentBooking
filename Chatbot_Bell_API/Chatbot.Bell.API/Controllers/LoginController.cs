using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Chatbot.Bell.API.Model;
using Chatbot.Bell.API.Processor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Chatbot.Bell.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private readonly ChatBotBusinessLogic _chatBotBusinessLogic;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IConfiguration config, ILogger<LoginController> logger, ChatBotBusinessLogic chatBotBusinessLogic )
        {
            _logger = logger;
            _config = config;
            _chatBotBusinessLogic = chatBotBusinessLogic;
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]AuthUser login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = _chatBotBusinessLogic.Generate_DR_JSONWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(AuthUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
                //new Claim("DateOfJoing", userInfo.DateOfJoing.ToString("yyyy-MM-dd")),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private AuthUser AuthenticateUser(AuthUser login)
        {
            AuthUser user = null;

            //Validate the User Credentials
            //Demo Purpose, I have Passed HardCoded User Information
            if (login.Username == AppConfig.AuthenticateUsername_DR && login.Password == AppConfig.AuthenticatePassword_DR && !String.IsNullOrEmpty(login.Program_Code))
            {
                user = new AuthUser
                {
                    Username = AppConfig.AuthenticateUsername_DR,
                    EmailAddress = AppConfig.AuthenticateUsername_DR,
                    Program_Code = login.Program_Code,
                    Password= login.Password
                };
            }
            return user;
        }
    }
}