using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class UserService
{
    private readonly DecodeToken _decodeToken;
    private readonly EmailService _emailService;
    private readonly UserRepo _userRepo;

    public UserService(UserRepo userRepo, DecodeToken decodeToken, EmailService emailService)
    {
        _decodeToken = decodeToken;
        _emailService = emailService;
        _userRepo = userRepo;
    }

    public async Task<object> GetUser(Guid userId)
    {
        try
        {
            User? user = await _userRepo.Get(userId);
            if (user != null)
            {
                UserInfoModel? userInfoModel = await _userRepo.GetUserInfo(userId);
                return userInfoModel != null ? userInfoModel : "Get user failed!";
            }

            return "User not exist!";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object?> GetUsers(string? status)
    {
        try
        {
            List<UserInfoModel>? users = await _userRepo.GetUsers(status);
            return users;
        }
        catch (Exception? e)
        {
            return e;
        }
    }

    public async Task<object> Register(UserRegisterRequestModel reqModel)
    {
        if (
            reqModel.Username.IsNullOrEmpty()
            || reqModel.Password.IsNullOrEmpty()
            || reqModel.Fullname.IsNullOrEmpty()
            || reqModel.Email.IsNullOrEmpty()
        )
        {
            return "Please fill-in all the information!!";
        }

        UserModel? userCheck = await _userRepo.GetUserByUsername(reqModel.Username);
        if (userCheck != null)
        {
            return "Duplicated username!";
        }

        bool emailDup = await _userRepo.CheckEmail(reqModel.Email);
        if (emailDup)
        {
            return "Duplicated email!";
        }

        if (reqModel.Username.Length < 6)
        {
            return "Username length invalid!";
        }

        Regex validatePasswordRegex =
            new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        if (!validatePasswordRegex.IsMatch(reqModel.Password))
        {
            return "Password invalid!";
        }

        Regex validateEmailRegex =
            new(
                "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"
            );
        if (!validateEmailRegex.IsMatch(reqModel.Email))
        {
            return "Email invalid!";
        }

        try
        {
            CreatePasswordHash(
                reqModel.Password,
                out byte[]? passwordHash,
                out byte[]? passwordSalt
            );
            User userModel =
                new()
                {
                    Id = Guid.NewGuid(),
                    Username = reqModel.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = reqModel.Fullname,
                    Email = reqModel.Email,
                    Birthday = reqModel.Birthdate,
                    Phone = reqModel.Phone,
                    Status = false,
                    Ban = false
                };
            User? userInsert = await _userRepo.Insert(userModel);

            if (userInsert != null)
            {
                _ = await _emailService.SendConfirmEmail(userInsert.Id);
                UserModel? user = await _userRepo.GetUserByUsername(userInsert.Username);
                if (user != null)
                {
                    UserInfoModel userLoginModel =
                        new()
                        {
                            Id = user.Id,
                            Username = user.Username,
                            RoleName = user.RoleName,
                            Fullname = user.Fullname,
                            Email = user.Email,
                            Birthday = user.Birthday,
                            Phone = user.Phone,
                            Status = user.Status,
                            Ban = user.Ban
                        };
                    return userLoginModel;
                }

                return "Retrieve created user information failed!";
            }

            return "Create user failed!";
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

    public async Task<object> Login(UserLoginRequestModel request)
    {
        try
        {
            UserModel? user = await _userRepo.GetUserByUsername(request.Username);
            if (user == null)
            {
                return "Username or password incorrect!";
            }

            if (user.Ban == true)
            {
                return "User Banned!";
            }

            if (user.Status == false)
            {
                return "User inactive!";
            }

            if (user.Status == null)
            {
                return "User account is disabled!";
            }

            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return "Username or password incorrect!";
            }

            string token = await CreateToken(user);
            UserInfoModel userLoginModel =
                new()
                {
                    Id = user.Id,
                    Username = user.Username,
                    RoleName = user.RoleName,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Birthday = user.Birthday,
                    Phone = user.Phone,
                    Status = user.Status,
                    Ban = user.Ban
                };
            UserLoginResponseModel userLoginResModel =
                new() { Token = token, UserInfo = userLoginModel };
            return userLoginResModel;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<object> Update(
        UserUpdateRequestModel userUpdateRequestModel,
        string token
    )
    {
        try
        {
            Regex validateEmailRegex =
                new(
                    "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"
                );
            if (!validateEmailRegex.IsMatch(userUpdateRequestModel.Email))
            {
                return "Email invalid!";
            }

            if (userUpdateRequestModel.Birthday == DateTime.MinValue)
            {
                return "Birthday invalid";
            }

            string username = _decodeToken.Decode(token, "username");
            bool update = await _userRepo.UpdateUser(userUpdateRequestModel, username);
            if (update)
            {
                UserModel? user = await _userRepo.GetUserByUsername(username);
                if (user != null)
                {
                    UserInfoModel userInfoModel =
                        new()
                        {
                            Id = user.Id,
                            Username = user.Username,
                            RoleName = user.RoleName,
                            Fullname = user.Fullname,
                            Email = user.Email,
                            Birthday = user.Birthday,
                            Phone = user.Phone,
                            Status = user.Status,
                            Ban = user.Ban
                        };
                    return userInfoModel;
                }

                return "Failed to get user information";
            }

            return "Update user information failed!";
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<object> UpdatePassword(
        UserChangePasswordRequestModel passwordRequestModel,
        string token
    )
    {
        try
        {
            string username = _decodeToken.Decode(token, "username");
            UserModel? user = await _userRepo.GetUserByUsername(passwordRequestModel.Username);
            if (user == null || !username.Equals(user.Username))
            {
                return "Username or password incorrect!";
            }

            if (
                !VerifyPasswordHash(
                    passwordRequestModel.OldPassword,
                    user.PasswordHash,
                    user.PasswordSalt
                )
            )
            {
                return "Username or password incorrect!";
            }

            Regex validatePasswordRegex =
                new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (!validatePasswordRegex.IsMatch(passwordRequestModel.NewPassword))
            {
                return "New password invalid!";
            }

            CreatePasswordHash(
                passwordRequestModel.NewPassword,
                out byte[]? newPasswordHash,
                out byte[]? newPasswordSalt
            );
            bool update = await _userRepo.UpdatePassword(
                username,
                newPasswordHash,
                newPasswordSalt
            );
            return update;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public async Task<object> ChangeAccountStatus(Guid userId, bool status)
    {
        try
        {
            bool result = await _userRepo.ChangeAccountStatus(userId, status);
            return result ? true : "Change account ban status failed!";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<object> DeleteAccount(Guid userId)
    {
        try
        {
            User? user = await _userRepo.Get(userId);
            if (user != null)
            {
                int deleteResult = await _userRepo.Delete(user);
                return deleteResult > 0 ? true : "Delete user failed!";
            }

            return "User does not exist!";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private void CreatePasswordHash(
        string password,
        out byte[] passwordHash,
        out byte[] passwordSalt
    )
    {
        using HMACSHA512 hmac = new();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using HMACSHA512 hmac = new(passwordSalt);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
    private async Task<string> CreateToken(UserModel user)
    {
        List<Claim> claims =
            new()
            {
                new Claim("userid", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("fullname", user.Fullname),
                new Claim("role", user.RoleName)
            };

        SymmetricSecurityKey key =
            new(Encoding.UTF8.GetBytes(await VaultHelper.GetSecrets("jwt")));

        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token =
            new(
                claims: claims,
                audience: "https://tuongtlc.site",
                issuer: "https://tuongtlc.site",
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}