using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories;

public class UserRepo : Repository<User>
{
    public UserRepo(TuongTLCDBContext context)
        : base(context)
    {
    }

    public async Task<UserInfoModel?> GetUserInfo(Guid userId)
    {
        try
        {
            var query =
                from u in context.Users
                join ur in context.UserRoles on u.RoleId equals ur.Id
                where u.Id.Equals(userId)
                select new { u, ur };
            UserInfoModel? userModel = await query
                .Select(
                    x =>
                        new UserInfoModel
                        {
                            Id = x.u.Id,
                            Username = x.u.Username ?? string.Empty,
                            RoleName = x.ur.RoleName ?? string.Empty,
                            Fullname = x.u.FullName ?? string.Empty,
                            Email = x.u.Email ?? string.Empty,
                            Birthday = x.u.Birthday,
                            Phone = x.u.Phone,
                            Status = x.u.Status,
                            Ban = x.u.Ban
                        }
                )
                .FirstOrDefaultAsync();
            return userModel;
        }
        catch
        {
            return null;
        }
    }

    public async Task<PostAuthor?> GetAuthor(Guid userId)
    {
        try
        {
            var query =
                from u in context.Users
                join ur in context.UserRoles on u.RoleId equals ur.Id
                where u.Id.Equals(userId)
                select new { u, ur };
            PostAuthor? userModel = await query
                .Select(
                    x =>
                        new PostAuthor
                        {
                            Id = x.u.Id,
                            RoleName = x.ur.RoleName ?? string.Empty,
                            Fullname = x.u.FullName ?? string.Empty
                        }
                )
                .FirstOrDefaultAsync();
            return userModel;
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserModel?> GetUserByUsername(string username)
    {
        try
        {
            var query =
                from u in context.Users
                join ur in context.UserRoles on u.RoleId equals ur.Id
                where u.Username != null && u.Username.Equals(username)
                select new { u, ur };
            UserModel? userModel = await query
                .Select(
                    x =>
                        new UserModel
                        {
                            Id = x.u.Id,
                            Username = x.u.Username ?? string.Empty,
                            PasswordHash = x.u.PasswordHash ?? Array.Empty<byte>(),
                            PasswordSalt = x.u.PasswordSalt ?? Array.Empty<byte>(),
                            RoleName = x.ur.RoleName ?? string.Empty,
                            Fullname = x.u.FullName ?? string.Empty,
                            Email = x.u.Email ?? string.Empty,
                            Birthday = x.u.Birthday,
                            Phone = x.u.Phone,
                            Status = x.u.Status,
                            Ban = x.u.Ban
                        }
                )
                .FirstOrDefaultAsync();
            return userModel;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<UserInfoModel>?> GetUsers(string? status)
    {
        try
        {
            if (status == null || status.Trim().ToLower().Equals("all") || status.Trim().ToLower().Equals("s"))
            {
                var query =
                    from u in context.Users
                    join ur in context.UserRoles on u.RoleId equals ur.Id
                    select new { u, ur };
                List<UserInfoModel> userModel = await query
                    .Select(
                        x =>
                            new UserInfoModel
                            {
                                Id = x.u.Id,
                                Username = x.u.Username ?? string.Empty,
                                RoleName = x.ur.RoleName ?? string.Empty,
                                Fullname = x.u.FullName ?? string.Empty,
                                Email = x.u.Email ?? string.Empty,
                                Birthday = x.u.Birthday,
                                Phone = x.u.Phone,
                                Status = x.u.Status,
                                Ban = x.u.Ban
                            }
                    )
                    .ToListAsync();
                return userModel;
            }
            else
            {
                bool statusIn = true;
                if (status.Equals("active"))
                {
                    statusIn = true;
                }

                if (status.Equals("inactive"))
                {
                    statusIn = false;
                }

                var query =
                    from u in context.Users
                    join ur in context.UserRoles on u.RoleId equals ur.Id
                    where u.Status.Equals(statusIn)
                    select new { u, ur };
                List<UserInfoModel> userModel = await query
                    .Select(
                        x =>
                            new UserInfoModel
                            {
                                Id = x.u.Id,
                                Username = x.u.Username ?? string.Empty,
                                RoleName = x.ur.RoleName ?? string.Empty,
                                Fullname = x.u.FullName ?? string.Empty,
                                Email = x.u.Email ?? string.Empty,
                                Birthday = x.u.Birthday,
                                Phone = x.u.Phone,
                                Status = x.u.Status,
                                Ban = x.u.Ban
                            }
                    )
                    .ToListAsync();
                return userModel;
            }
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> CheckEmail(string email)
    {
        try
        {
            User? user = await context.Users
                .Where(x => x.Email != null && x.Email.Equals(email))
                .FirstOrDefaultAsync();
            return user != null;
        }
        catch
        {
            return true;
        }
    }

    public async Task<bool> UpdateUser(
        UserUpdateRequestModel userUpdateRequestModel,
        string username
    )
    {
        try
        {
            User? user = await context.Users
                .Where(x => x.Username != null && x.Username.Equals(username))
                .FirstOrDefaultAsync();
            if (user != null)
            {
                if (
                    !userUpdateRequestModel.Fullname.IsNullOrEmpty()
                    && !userUpdateRequestModel.Fullname.Equals(user.FullName)
                )
                {
                    user.FullName = userUpdateRequestModel.Fullname;
                }

                if (
                    !userUpdateRequestModel.Email.IsNullOrEmpty()
                    && !userUpdateRequestModel.Email.Equals(user.Email)
                )
                {
                    user.Email = userUpdateRequestModel.Email;
                }

                if (
                    !userUpdateRequestModel.Phone.IsNullOrEmpty()
                    && !userUpdateRequestModel.Phone.Equals(user.Phone)
                )
                {
                    user.Phone = userUpdateRequestModel.Phone;
                }

                if (
                    !userUpdateRequestModel.Birthday.Equals(user.Birthday)
                )
                {
                    user.Birthday = userUpdateRequestModel.Birthday;
                }

                _ = await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdatePassword(
        string username,
        byte[] passwordHash,
        byte[] passwordSalt
    )
    {
        try
        {
            User? user = await context.Users
                .Where(x => x.Username != null && x.Username.Equals(username))
                .FirstOrDefaultAsync();
            if (user != null)
            {
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                _ = await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ChangeAccountStatus(Guid userId, bool status)
    {
        try
        {
            User? user = await context.Users.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Ban = status;
                _ = await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ChangeAccountStatusByEmail(string email, bool status)
    {
        try
        {
            User? user = await context.Users.Where(x => x.Email != null && x.Email.Equals(email)).FirstOrDefaultAsync();
            if (user != null)
            {
                user.Status = status;
                _ = await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}