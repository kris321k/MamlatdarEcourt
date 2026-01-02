using MamlatdarEcourt.DTOS;
using MamlatdarEcourt.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Net.Mail;
using MamlatdarEcourt.Helpers;

namespace MamlatdarEcourt.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;

        public OtpService(IMemoryCache cache, IConfiguration config)
        {
            _cache = cache;
            _config = config;
        }

        public string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        public async Task<string> SendOtpAsync(string email, UserRegister dto)
        {
            var otp = GenerateOtp();
            var hashedOtp = HashHelper.Hash(otp);

            var sessionId = Guid.NewGuid().ToString();

            var sessionData = new OtpSession
            {
                Email = email,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                PendingUser = dto,
                HashedOtp = hashedOtp
            };

            _cache.Set($"OtpSession:{sessionId}", sessionData, TimeSpan.FromMinutes(5));

            var smtpClient = new SmtpClient(_config["Email:Host"])
            {
                Port = _config.GetValue<int>("Email:Port"),
                Credentials = new NetworkCredential(
                    _config["Email:Username"],
                    _config["Email:Password"]
                ),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config.GetValue<string>("Email:From")!),
                Subject = "Your OTP Code",
                Body = $"Your OTP is: {otp}",
                IsBodyHtml = false
            };

            mail.To.Add(email);

            try
            {
                await smtpClient.SendMailAsync(mail);
            }
            catch (Exception)
            {
                throw new Exception("An error occurred while sending the OTP");
            }

            return sessionId;
        }

        public bool ValidateOtp(string sessionId, string otp)
        {
            if (!_cache.TryGetValue($"OtpSession:{sessionId}", out OtpSession? sessionData))
                return false;

            if (sessionData?.ExpiresAt < DateTime.UtcNow)
                return false;

            var enteredHash = HashHelper.Hash(otp);

            return HashHelper.ConstantTimeEquals(sessionData?.HashedOtp, enteredHash);
        }

        public OtpSession? GetUserData(string sessionId)
        {
            _cache.TryGetValue($"OtpSession:{sessionId}", out OtpSession? sessionData);
            return sessionData;
        }

        public void Clear(string sessionId)
        {
            _cache.Remove($"OtpSession:{sessionId}");
        }
    }
}
