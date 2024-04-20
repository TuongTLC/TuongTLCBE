using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class OTPCodeRepo : Repository<Otpcode>
{
    public OTPCodeRepo(TuongTLCDBContext context) : base(context)
    {
    }

    public async Task<Otpcode?> GetOtp(string code, string email)
    {
        return await context.Otpcodes.Where(x => x != null && x.Code.Equals(int.Parse(code)) && x.Email.Equals(email))
            .FirstOrDefaultAsync();
    }
}