using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class PostRepo: Repository<Post>
{
    public PostRepo(TuongTlcdbContext context)
        : base(context)
    {
        
    }
}