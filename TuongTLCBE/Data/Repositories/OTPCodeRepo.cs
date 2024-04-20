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
        return await context.Otpcodes.Where(x => x.Code.Equals(int.Parse(code)) && x.Email.Equals(email))
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CheckOtpExist(string email)
    {
        var otp = await context.Otpcodes.Where(x => x.Email.Equals(email))
            .FirstOrDefaultAsync();
        if (otp == null)
        {
            return false;
        }
        return true;
    }
}