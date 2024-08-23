using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splenduel.Core.Auth;
using Splenduel.Core.Auth.Interfaces;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SplenduelAPI.Controllers
{
    [Route("[controller]")]    
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            string? userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(userName ?? string.Empty);
        }

        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginViewModel)
        {
            var loginData = await _userService.Login(loginViewModel.UserName, loginViewModel.Password);
            if (string.IsNullOrEmpty(loginData.Token) || loginData.Token.Contains("Error"))
            {
                return BadRequest(loginData);
            }
            else
            {
                return Ok(new { access_token = loginData.Token, user = loginData.UserName });
            }
        }
        [HttpGet("ValidateToken")]
        [Authorize]
        public IActionResult Validate()
        {
            return Ok();
        }

        //    // PUT api/<UserController>/5
        //    [HttpPut("{id}")]
        //    public void Put(int id, [FromBody] string value)
        //    {
        //    }

        //    // DELETE api/<UserController>/5
        //    [HttpDelete("{id}")]
        //    public void Delete(int id)
        //    {
        //    }
    }
}
