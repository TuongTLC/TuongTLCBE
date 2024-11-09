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
    private readonly EmailService _emailService;
    private readonly UserService _userService;

    public UserController(UserService userService, EmailService emailService)
    {
        _userService = userService;
        _emailService = emailService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Register(UserRegisterRequestModel request)
    {
        object result = await _userService.Register(request);
        return result.GetType() == typeof(UserInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> Login(UserLoginRequestModel request)
    {
        object result = await _userService.Login(request);
        return result.GetType() == typeof(UserLoginResponseModel)
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("update")]
    [SwaggerOperation(Summary = "Update user information except password")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> Update(UserUpdateRequestModel request)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _userService.Update(request, token);
        return result.GetType() == typeof(UserInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpPost("change-password")]
    [SwaggerOperation(Summary = "Change user password")]
    [Authorize(Roles = "User, Admin")]
    public async Task<ActionResult<object>> ChangePassword(
        UserChangePasswordRequestModel request
    )
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _userService.UpdatePassword(request, token);
        return result is bool
            ? (bool)result
                ? Ok("Change password success.")
                : BadRequest("Change password failed.")
            : (ActionResult<object>)BadRequest(result);
    }

    [HttpPost("change-account-status")]
    [SwaggerOperation(Summary = "Admin ban/unban account")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> DisableAccount(BanUserModel userModel)
    {
        object result = await _userService.ChangeAccountStatus(userModel.UserId, userModel.Status);
        return result is bool ? Ok("User status changed!") : BadRequest(result);
    }

    [HttpGet("get-user")]
    [SwaggerOperation(Summary = "Admin get a account")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetUser(Guid userId)
    {
        object result = await _userService.GetUser(userId);
        return result.GetType() == typeof(UserInfoModel) ? Ok(result) : BadRequest(result);
    }

    [HttpGet("get-users")]
    [SwaggerOperation(Summary = "Admin get account by status: active/inactive/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> GetUsers(string? status)
    {
        object? result = await _userService.GetUsers(status);
        return result?.GetType() == typeof(List<UserInfoModel>) ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("delete-account")]
    [SwaggerOperation(Summary = "Admin delete account")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<object>> DeleteAccount(Guid userId)
    {
        object result = await _userService.DeleteAccount(userId);
        return result is bool ? Ok("User account deleted!") : BadRequest(result);
    }

    [HttpPost("verify-user")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> VerifyEmail(EmailVerifyModel request)
    {
        bool result = request.Username != null && request.Code != null &&
                     await _emailService.VerifyCode(request.Code, request.Username);
        return result ? Ok("User verified!") : BadRequest("User verified failed!");
    }

    [HttpPost("sent-new-otp-code")]
    [AllowAnonymous]
    public async Task<ActionResult<object>> SendNewOtp([FromBody] string username)
    {
        object result = await _emailService.SendNewOtpCode(username);
        return result is bool ? Ok("New OTP code sent!") : BadRequest(result);
    }
}