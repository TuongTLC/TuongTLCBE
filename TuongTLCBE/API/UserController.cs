using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.API;

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
        var result = await _userService.Register(request);
        return result.GetType() == typeof(UserInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Login(UserLoginRequestModel request)
    {
        var result = await _userService.Login(request);
        return result.GetType() == typeof(UserLoginResponseModel)
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("update")]
    [SwaggerOperation(Summary = "Update user information except password")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> Update(UserUpdateRequestModel request)
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _userService.Update(request, token);
        return result.GetType() == typeof(UserInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("change-password")]
    [SwaggerOperation(Summary = "Change user password")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> ChangePassword(
        UserChangePasswordRequestModel request
    )
    {
        var token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        var result = await _userService.UpdatePassword(request, token);
        if (result is bool)
            return (bool)result
                ? Ok("Change password success.")
                : BadRequest("Change password failed.");
        return BadRequest(result);
    }

    [HttpPost("change-account-status")]
    [SwaggerOperation(Summary = "Admin ban/unban account")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> DisableAccount(BanUserModel userModel)
    {
        var result = await _userService.ChangeAccountStatus(userModel.UserId, userModel.Status);
        return result is bool ? Ok("User status changed!") : BadRequest(result);
    }

    [HttpGet("get-user")]
    [SwaggerOperation(Summary = "Admin get a account")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetUser(Guid userId)
    {
        var result = await _userService.GetUser(userId);
        return result.GetType() == typeof(UserInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-users")]
    [SwaggerOperation(Summary = "Admin get account by status: active/inactive/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetUsers(string? status)
    {
        var result = await _userService.GetUsers(status);
        return result?.GetType() == typeof(List<UserInfoModel>) ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("delete-account")]
    [SwaggerOperation(Summary = "Admin delete account")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> DeleteAccount(Guid userId)
    {
        var result = await _userService.DeleteAccount(userId);
        return result is bool ? Ok("User account deleted!") : BadRequest(result);
    }
}