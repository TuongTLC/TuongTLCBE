using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business
{
    public class UserService
    {
        private readonly UserRepo _userRepo;
        private readonly RefreshTokenRepo _refreshTokenRepo;

        public UserService(RefreshTokenRepo refreshTokenRepo, UserRepo userRepo)
        {
            _userRepo = userRepo;
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task<string> Register(UserReqModel reqModel)
        {
            try
            {
                CreatePasswordHash(reqModel.Password, out byte[] passwordHash, out byte[] passwordSalt);
                User userModel = new()
                {
                    Username = reqModel.Username,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    FullName = reqModel.Fullname,
                    Email = reqModel.Email,
                    RoleId = Guid.Parse("38b3d081-a7bc-4ed2-a394-f47d01263e0e")
                };
                _ = await _userRepo.Insert(userModel);
                return "User registered!!!";

            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.ToString());
            }

        }
        public async Task<UserModel?> Login(UserLoginModel request)
        {
            try
            {
                UserModel? userModel = await _userRepo.GetUser(request);
                return userModel;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.ToString());
            }
        }
        public async Task<RefreshToken?> RefreshToken(RefreshToken refreshToken)
        {
            try
            {
                RefreshToken? result = await _refreshTokenRepo.Insert(refreshToken);
                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.ToString());
            }
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using HMACSHA512 hmac = new(passwordSalt);
            byte[] computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
        public async Task<string> CreateToken(UserModel user)
        {
            List<Claim> claims = new()
            {
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
        public RefreshToken GenerateRefreshToken(Guid userID)
        {
            RefreshToken refreshToken = new()
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(30),
                Created = DateTime.Now,
                UserId = userID
            };

            return refreshToken;
        }
        public async Task<RefreshToken?> SetRefreshToken(RefreshToken newRefreshToken)
        {
            RefreshToken refreshToken = new()
            {
                Token = newRefreshToken.Token,
                Created = newRefreshToken.Created,
                Expires = newRefreshToken.Expires,
                UserId = newRefreshToken.UserId
            };
            RefreshToken? result = await RefreshToken(refreshToken);
            return result;
        }

    }
}

