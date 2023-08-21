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
        public async Task<ActionResult<string>> Login(UserLoginModel request)
        {
            UserModel? user = await _userService.Login(request);

            if (user == null || user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }

            if (!_userService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Wrong password.");
            }

            string token = await _userService.CreateToken(user);

            RefreshToken refreshToken = _userService.GenerateRefreshToken(user.Id);

            _ = await _userService.SetRefreshToken(refreshToken);

            return Ok(token);
        }
    }
}

