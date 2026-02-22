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


            var SenderEmail = _config.GetValue<string>("Email:Username")!;


            var mail = new MailMessage
            {
                From = new MailAddress(_config.GetValue<string>("Email:Username")!),
                Subject = "Your OTP Code",
                Body = $"Hi {dto.FirstName} {dto.LastName} your verfication OTP is: {otp}/n PLEASE DO NOT SHARE WITH ANYONE",
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


            Console.WriteLine($"this is the system generated {sessionId}");

            return sessionId;
        }

        public bool ValidateOtp(string sessionId, string otp)
        {
            Console.WriteLine($"this is user provided sessionId {sessionId}");

            if (!_cache.TryGetValue($"OtpSession:{sessionId}", out OtpSession? sessionData))
            {
                Console.WriteLine("wrong session id");

                return false;
            }

            if (sessionData?.ExpiresAt < DateTime.UtcNow)
            {
                Console.WriteLine("the token expired");
                return false;
            }

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
