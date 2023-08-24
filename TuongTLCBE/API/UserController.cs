using Microsoft.AspNetCore.Mvc;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Entities;
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
        public async Task<ActionResult<User>> Register(UserReqModel request)
        {
            try
            {
                string result = await _userService.Register(request);
                return Ok(result);
            }
            catch (Exception e)
            {

                return BadRequest(e.ToString());
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResModel>> Login(UserLoginReqModel request)
        {
            UserLoginResModel? userLoginResModel = await _userService.Login(request);
            if (userLoginResModel != null)
            {
                return Ok(userLoginResModel);
            }
            else
            {
                return BadRequest("Username or password invalid!");
            }
        }
    }
}

