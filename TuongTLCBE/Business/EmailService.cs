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
            User? user = await _userRepo.Get(userId);
            if (user == null)
            {
                return "User does not exist!";
            }

            Random rnd = new();
            string otp = rnd.Next(000000, 999999).ToString("D6");
            Otpcode otpcode = new()
            {
                Code = otp,
                Email = user.Email
            };
            Otpcode? insertOtp = await _otpCodeRepo.Insert(otpcode);
            if (insertOtp != null)
            {
                Data.Models.EmailSecretModel emailSecretModel = await VaultHelper.GetEmailSecrets();
                if (emailSecretModel.Email == null || emailSecretModel.Password == null)
                {
                    return "Get email failed!";
                }

                string from = emailSecretModel.Email;
                string password = emailSecretModel.Password;
                MimeMessage message = new();
                message.From.Add(MailboxAddress.Parse(from));
                message.Subject = "TuongTLC account verification.";
                message.To.Add(MailboxAddress.Parse(user.Email));
                message.Body = new TextPart(TextFormat.Html)
                {
                    Text =
                        "<html>" +
                        "<body>" +
                        "<h3>Dear " + user.Username + ",</h3>" +
                        "<p>We noticed you want to verify your account using this email." +
                        " To complete verify process, please enter the following one-time code in the verification form:</p>" +
                        "<h1>" + otp + "</h1>" +
                        "<p>This code is valid for 3 minutes.<p>" +
                        "<h3>Wasn’t you?</h3>" +
                        "<p>If this wasn’t you, don’t worry, your email address may have been entered by mistake." +
                        "You can simply ignore or delete this email.</p>" +
                        "<p>Thank you for being part of the TuongTLC community!<p><br/>" +
                        "<p>Warm regards, <p>" +
                        "<p>TuongTLC" +
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
            Console.Write(e);
            return e;
        }
    }

    public async Task<object> NewPostNotification(Guid postId)
    {
        try
        {
            Data.Models.EmailSecretModel emailSecretModel = await VaultHelper.GetEmailSecrets();
            if (emailSecretModel.Email == null || emailSecretModel.Password == null)
            {
                return "Get email failed!";
            }

            string from = emailSecretModel.Email;
            string password = emailSecretModel.Password;
            MimeMessage message = new();
            message.From.Add(MailboxAddress.Parse(from));
            message.Subject = "TuongTLC new post notification.";
            message.To.Add(MailboxAddress.Parse("trinhtuong98@gmail.com"));
            message.Body = new TextPart(TextFormat.Html)
            {
                Text =
                    "<html>" +
                    "<body>" +
                    "<h1>TuongTLC<h1>" +
                    "<h3>A new post have been created and await your approval.</h3>" +
                    "<p>Please follow the link to view post detail.</p>" +
                    "<p>Post detail: https://tuongtlc.site/preview-post?postId=" + postId + "</p>" +
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
        catch (Exception e)
        {
            Console.Write(e);
            return e;
        }
    }

    public async Task<bool> VerifyCode(string code, string username)
    {
        try
        {
            Data.Models.UserModel? user = await _userRepo.GetUserByUsername(username);
            if (user != null)
            {
                Otpcode? otp = await _otpCodeRepo.GetOtp(code, user.Email);
                if (otp != null)
                {
                    int deleteCode = await _otpCodeRepo.Delete(otp);
                    bool updateUser = await _userRepo.ChangeAccountStatusByEmail(user.Email, true);
                    if (deleteCode > 0 && updateUser)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<object> SendNewOtpCode(string username)
    {
        try
        {
            Data.Models.UserModel? user = await _userRepo.GetUserByUsername(username);

            if (user != null)
            {
                if (user.Status == true)
                {
                    return "User already activated!";
                }
                bool otpExist = await _otpCodeRepo.CheckOtpExist(user.Email);
                if (otpExist)
                {
                    return "OTP code can only be sent once every 3 minutes!";
                }
                object sent = await SendConfirmEmail(user.Id);
                if ((bool)sent)
                {
                    return true;
                }
            }
            return "Send OTP Code failed!";
        }
        catch (Exception)
        {
            return "Send OTP Code failed!";
        }
    }
}