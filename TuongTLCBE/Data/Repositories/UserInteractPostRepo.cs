using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class UserInteractPostRepo : Repository<UserInteractPost>
{
    public UserInteractPostRepo(TuongTLCDBContext context) : base(context)
    {
    }
}