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
        public async Task<ActionResult<object>> Register(UserRegisterRequestModel request)
        {
            object result = await _userService.Register(request);
            return (result.GetType() == typeof(UserInfoModel)) ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Login(UserLoginRequestModel request)
        {
            object result = await _userService.Login(request);
            return (result.GetType() == typeof(UserLoginResponseModel))
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpPost("update")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<object>> Update(UserUpdateRequestModel request)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            object result = await _userService.Update(request, token);
            return (result.GetType() == typeof(UserInfoModel)) ? Ok(result) : BadRequest(result);
        }

        [HttpPost("change-password")]
        [Authorize(Roles = "User, Admin")]
        public async Task<ActionResult<object>> ChangePassword(
            UserChangePasswordRequestModel request
        )
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            object result = await _userService.UpdatePassword(request, token);
            if (result.GetType() == typeof(bool))
            {
                return (bool)result == true
                    ? Ok("Change password success.")
                    : BadRequest("Change password failed.");
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
