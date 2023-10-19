using System.ComponentModel.DataAnnotations;

namespace TuongTLCBE.Data.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string RoleName { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Phone { get; set; }
    public bool? Status { get; set; }
}

public class UserRegisterRequestModel
{
    [Required(ErrorMessage = "Required")] public string Username { get; set; }

    [Required(ErrorMessage = "Required")] public string Password { get; set; }

    [Required(ErrorMessage = "Required")] public string Fullname { get; set; }

    [Required(ErrorMessage = "Required")] public string Email { get; set; }

    public DateTime? Birthday { get; set; }
    public string? Phone { get; set; }
}

public class UserLoginRequestModel
{
    [Required(ErrorMessage = "Required")] public string Username { get; set; }

    [Required(ErrorMessage = "Required")] public string Password { get; set; }
}

public class UserInfoModel
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string RoleName { get; set; }
    public string Fullname { get; set; }
    public string Email { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Phone { get; set; }

    public bool? Status { get; set; }
}

public class UserLoginResponseModel
{
    public string Token { get; set; }
    public UserInfoModel UserInfo { get; set; }
}

public class BanUserModel
{
    public Guid UserId { get; set; }
    public bool Status { get; set; }
}

public class UserUpdateRequestModel
{
    public string Fullname { get; set; }
    public string Email { get; set; }
    public DateTime Birthday { get; set; }
    public string Phone { get; set; }
}

public class UserChangePasswordRequestModel
{
    [Required(ErrorMessage = "Required")] public string Username { get; set; }

    [Required(ErrorMessage = "Required")] public string OldPassword { get; set; }

    [Required(ErrorMessage = "Required")] public string NewPassword { get; set; }
}

public class EmailSecretModel
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class EmailVerifyModel
{
    public string? Code { get; set; }
    public string? Email { get; set; }
}