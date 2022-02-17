using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController  : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<User> SignIn([FromBody] LoginPswd loginPswd)
        {
            if (loginPswd == null
                || string.IsNullOrWhiteSpace(loginPswd.Login)
                || string.IsNullOrWhiteSpace(loginPswd.Password))
            {
                return BadRequest("Invalid value provided");
            }
            
            var user = _userService.SignInAsync(loginPswd.Login, loginPswd.Password);

            if (user == null)
            {
                return Conflict("User was not found");
            }
            
            return Ok(user);
        }

        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (user == null
                || string.IsNullOrWhiteSpace(user.Name)
                || string.IsNullOrWhiteSpace(user.Email)
                || string.IsNullOrWhiteSpace(user.Login)
                || string.IsNullOrWhiteSpace(user.Password)) // check if any values are null or empty
            {
                return BadRequest("User values can't be empty");
            }

            if (!await _userService.RegisterAsync(user))
            {
                return BadRequest();
            }
            
            return Ok(user);
        }
        
        [HttpPost]
        [Route("forgot")]
        public async Task<ActionResult> ForgotPassword([FromBody] EmailHttpModel email)
        {
            if (email == null 
            || string.IsNullOrWhiteSpace(email.Email))
            {
                return BadRequest("Email can't be null or empty");
            }

            if (!await _userService.PasswordRecoveryAsync(email.Email))
            {
                return BadRequest("Email not found");
            }

            return Ok();
        }
    }
}