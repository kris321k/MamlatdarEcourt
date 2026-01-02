using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using MamlatdarEcourt.DTOS;
namespace MamlatdarEcourt.Models
{
    public class OtpSession
    {
        public string? Email { get; set; }
        public string? HashedOtp { get; set; }
        public UserRegister? PendingUser { get; set; } 
        public DateTime ExpiresAt { get; set; }

    }
}