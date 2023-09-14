using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class PostCategoryRepo: Repository<PostCategory>
{
    public PostCategoryRepo(TuongTlcdbContext context) : base(context)
    {
    }
}