using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;
namespace TuongTLCBE.API
{
    [Route("user/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserInfoModel>> Register(UserRegisterRequestModel request)
        {
            object result = await _userService.Register(request);
            return (result.GetType() == typeof(UserInfoModel)) ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserLoginResponseModel>> Login(UserLoginRequestModel request)
        {
            UserLoginResponseModel? userLoginResModel = await _userService.Login(request);
            return (userLoginResModel != null) ? Ok(userLoginResModel) : BadRequest("Username or password invalid!");
        }

        [HttpGet("weather")]
        [Authorize(Roles = "User")]
        public IActionResult Get()
        {
            return Ok("Swashbuckle.AspNetCore.SwaggerGen");
        }
    }
}