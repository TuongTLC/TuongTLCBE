using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business
{
    public class UserService
    {
        private readonly UserRepo _userRepo;
        public UserService(UserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<object> Register(UserReqModel reqModel)
        {
            if (reqModel.Username.IsNullOrEmpty() || reqModel.Password.IsNullOrEmpty() || reqModel.Fullname.IsNullOrEmpty() || reqModel.Email.IsNullOrEmpty())
            {
                return "Please fill-in all the informations!!";
            }

            UserModel? userCheck = await _userRepo.GetUser(reqModel.Username);
            if (userCheck != null)
            {
                return "Duplicated username!";
            }

            if (reqModel.Username.Length < 6)
            {
                return "Username length invalid!";
            }
            Regex validatePasswordRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            if (!validatePasswordRegex.IsMatch(reqModel.Password))
            {
                return "Password invalid!";
            }

            Regex validateEmailRegex = new("(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
            if (!validateEmailRegex.IsMatch(reqModel.Email))
            {
                return "Email invalid!";
            }
            try
            {
                CreatePasswordHash(reqModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
                User userModel = new()
                {
                    Id = Guid.NewGuid(),
                    Username = reqModel.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = reqModel.Fullname,
                    Email = reqModel.Email,
                };
                User? userInsert = await _userRepo.Insert(userModel);
                if (userInsert != null)
                {
                    UserModel? user = await _userRepo.GetUser(userInsert.Username);
                    if (user != null)
                    {
                        UserLoginModel userLoginModel = new()
                        {
                            Username = user.Username,
                            RoleName = user.RoleName,
                            Fullname = user.Fullname,
                            Email = user.Email
                        };
                        return userLoginModel;
                    }
                    else
                    {
                        return "Retrieve created user information failed!";
                    }
                }
                else
                {
                    return "Create user failed!";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public async Task<UserLoginResModel?> Login(UserLoginReqModel request)
        {
            UserModel? user = await _userRepo.GetUser(request.Username);
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            string token = await CreateToken(user);
            UserLoginModel userLoginModel = new()
            {
                Username = user.Username,
                RoleName = user.RoleName,
                Fullname = user.Fullname,
                Email = user.Email
            };
            UserLoginResModel userLoginResModel = new()
            {
                Token = token,
                UserInfo = userLoginModel
            };
            return userLoginResModel;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new(passwordSalt);
            byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
        public async Task<string> CreateToken(UserModel user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim("userID", user.Id.ToString()),
                new Claim("name", user.Fullname),
                new Claim("role", user.RoleName),

            };

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(await VaultHelper.GetSecrets("jwt")));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha512Signature);

            JwtSecurityToken token = new(
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}

