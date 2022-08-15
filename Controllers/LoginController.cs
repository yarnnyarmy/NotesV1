using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesV1.Models;
using NotesV1.Models.Token;
using NotesV1.Services;

namespace NotesV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        
       // private readonly JwtAuthenticationManager _jwtAuthenticationManager;
        private readonly LoginService _loginService;
        private readonly Token _token;

        public LoginController(LoginService loginService, Token token)
        {
            //_jwtAuthenticationManager = jwtAuthenticationManager;
            _loginService = loginService;
            _token = token;
        }

        //[HttpPost]
        //public IActionResult Authorize([FromBody] User user)
        //{
        //    var token = _jwtAuthenticationManager.Authenticate(user.UserName, user.Password);
        //    if (string.IsNullOrEmpty(token));        
        //    return Ok(token);
            
            
        //}

        [HttpPost("login")]
        public ActionResult GetUser(string username, string password)
        {
            var user =  _loginService.VerifyUser(username, password);
            
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var getToken = _loginService.Token();
            return Ok(getToken);
        }

    }
}
