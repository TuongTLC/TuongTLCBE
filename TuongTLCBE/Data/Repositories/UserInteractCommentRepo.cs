using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class UserInteractCommentRepo : Repository<UserInteractComment>
{
    public UserInteractCommentRepo(TuongTLCDBContext context) : base(context)
    {
    }
}