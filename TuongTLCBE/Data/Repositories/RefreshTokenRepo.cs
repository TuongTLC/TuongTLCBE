using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories
{
    public class RefreshTokenRepo : Repository<RefreshToken>
    {
        public RefreshTokenRepo(TuongTlcdbContext context) : base(context)
        {
        }
    }
}

