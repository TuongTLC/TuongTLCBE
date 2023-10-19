using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class EmailService
{
    private readonly OTPCodeRepo _otpCodeRepo;
    private readonly UserRepo _userRepo;

    public EmailService(UserRepo userRepo, OTPCodeRepo otpCodeRepo)
    {
        _userRepo = userRepo;
        _otpCodeRepo = otpCodeRepo;
    }


    public async Task<object> SendConfirmEmail(Guid userId)
    {
        try
        {
            var user = await _userRepo.Get(userId);
            if (user == null) return "User does not exist!";
            Random rnd = new();
            var otp = rnd.Next(000000, 999999);
            var otpcode = new Otpcode
            {
                Code = otp,
                Email = user.Email
            };
            var insertOtp = await _otpCodeRepo.Insert(otpcode);
            if (insertOtp != null)
            {
                var emailSecretModel = await VaultHelper.GetEmailSecrets();
                if (emailSecretModel.Email == null || emailSecretModel.Password == null) return "Get email failed!";
                var from = emailSecretModel.Email;
                var password = emailSecretModel.Password;
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(from));
                message.Subject = "TuongTLC account confirmation.";
                message.To.Add(MailboxAddress.Parse(user.Email));
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text =
                        "<html>" +
                        "<body>" +
                        "<h1>TuongTLC<h1>" +
                        "<h3>You have just create the account:  " + user.Username + ".</h3>" +
                        "<p>Please use the OTP code to activate your account.</p>" +
                        "<p>Code: " + otp + "</p>" +
                        "</body>" +
                        "</html>"
                };

                using SmtpClient smtp = new();
                await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(from, password);
                _ = await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<bool> VerifyCode(string code, string email)
    {
        try
        {
            var otp = await _otpCodeRepo.GetOtp(code, email);
            if (otp != null)
            {
                var deleteCode = await _otpCodeRepo.Delete(otp);
                var updateUser = await _userRepo.ChangeAccountStatusByEmail(email, true);
                if (deleteCode > 0 && updateUser) return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}