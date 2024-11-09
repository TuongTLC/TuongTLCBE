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
        return await context.Otpcodes.Where(x => x.Code.Equals(code) && x.Email.Equals(email))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CheckOtpExist(string email)
    {
        Otpcode? otp = await context.Otpcodes.Where(x => x.Email.Equals(email))
            .FirstOrDefaultAsync();
        return otp != null;
    }
}