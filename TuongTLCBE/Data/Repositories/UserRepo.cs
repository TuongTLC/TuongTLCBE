using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Models;

namespace TuongTLCBE.Data.Repositories
{
    public class UserRepo : Repository<User>
    {
        public UserRepo(TuongTlcdbContext context) : base(context)
        {
        }
        public async Task<UserModel?> GetUser(UserLoginReqModel userReqModel)
        {
            try
            {
                var query = from u in context.Users
                            join ur in context.UserRoles
                            on u.RoleId equals ur.Id
                            where u.Username.ToLower().Trim().Equals(userReqModel.Username.ToLower().Trim())
                            select new { u, ur };
                UserModel? userModel = await query.Select(x => new UserModel()
                {
                    Id = x.u.Id,
                    Username = x.u.Username,
                    PasswordHash = x.u.PasswordHash,
                    PasswordSalt = x.u.PasswordSalt,
                    RoleName = x.ur.RoleName,
                    Fullname = x.u.FullName,
                    Email = x.u.Email

                }).FirstOrDefaultAsync();
                return userModel;
            }
            catch
            {
                return null;
            }
        }
    }
}

