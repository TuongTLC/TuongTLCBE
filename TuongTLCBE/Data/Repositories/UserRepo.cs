using System.Runtime.InteropServices.ComTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories
{
    public class UserRepo : Repository<User>
    {
        public UserRepo(TuongTlcdbContext context)
            : base(context) { }

        public async Task<UserModel?> GetUser(string username)
        {
            try
            {
                var query =
                    from u in context.Users
                    join ur in context.UserRoles on u.RoleId equals ur.Id
                    where u.Username.Equals(username)
                    select new { u, ur };
                UserModel? userModel = await query
                    .Select(
                        x =>
                            new UserModel()
                            {
                                Id = x.u.Id,
                                Username = x.u.Username,
                                PasswordHash = x.u.PasswordHash,
                                PasswordSalt = x.u.PasswordSalt,
                                RoleName = x.ur.RoleName,
                                Fullname = x.u.FullName,
                                Email = x.u.Email,
                                Birthday = x.u.Birthday,
                                Phone = x.u.Phone,
                                Status = x.u.Status
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

        public async Task<bool> CheckEmail(string email)
        {
            try
            {
                User? user = await context.Users
                    .Where(x => x.Email.Equals(email))
                    .FirstOrDefaultAsync();
                if (user != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
                    .Where(x => x.Username.Equals(username))
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
                    _ = context.Update(user);
                    _ = await context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
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
                    .Where(x => x.Username.Equals(username))
                    .FirstOrDefaultAsync();
                if (user != null)
                {
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    _ = context.Update(user);
                    _ = await context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
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
                if (user!= null)
                {
                    user.Status = status;
                    _ = context.Update(user);
                    _ = await context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            catch
            {
                return false;
            }   
        }
    }
}
