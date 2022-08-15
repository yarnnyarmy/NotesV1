using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotesV1.Services;

namespace NotesV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private LogoutService _logoutService;

        public LogoutController(LogoutService logoutService)
        {
            _logoutService = logoutService;
        }

        [HttpPost]
        public ActionResult UserLogout()
        {
           var logOut =  _logoutService.UserLogOut();
            if(logOut == false)
            {
                BadRequest("User not logged in");
            }
            return Ok(logOut);
        }
    }
}
