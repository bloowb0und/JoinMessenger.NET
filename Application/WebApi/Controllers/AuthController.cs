using System.Threading;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using DAL.Abstractions.Interfaces;
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
        public async Task<ActionResult<User>> SignIn([FromBody] LoginPswd loginPswd)
        {
            var user = _userService.SignIn(loginPswd.Login, loginPswd.Password);

            if (user == null)
            {
                ModelState.AddModelError("Login", "User was not found");
                
                return Conflict(ModelState);
            }
            
            return Ok(user);
        }
        
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            if (!await _userService.Register(user))
            {
                return BadRequest();
            }
            
            return Ok(user);
        }
        
        [HttpPost]
        [Route("forgot")]
        public async Task<ActionResult> ForgotPassword([FromBody] EmailHttpModel email)
        {
            if (string.IsNullOrWhiteSpace(email.Email))
            {
                ModelState.AddModelError("email", "Email can't be null or empty");
                
                return BadRequest(ModelState);
            }
            Thread.Sleep(1000);
            if (! _userService.PasswordRecovery(email.Email))
            {
                ModelState.AddModelError("email", "Email not found");
                
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}