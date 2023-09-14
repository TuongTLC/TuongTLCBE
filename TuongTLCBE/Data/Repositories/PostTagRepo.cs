using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class PostTagRepo: Repository<PostTag>
{
    public PostTagRepo(TuongTlcdbContext context) : base(context)
    {
    }
}