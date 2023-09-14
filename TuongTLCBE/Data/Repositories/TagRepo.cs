using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class TagRepo: Repository<Tag>
{
    public TagRepo(TuongTlcdbContext context) : base(context)
    {
    }
}