using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace MamlatdarEcourt.DTOS
{
    
    public class LoginDto
    {
        [Required]
        public string Email{get; set;} = string.Empty;

        [Required]

        public string Password{get; set;} = string.Empty;

        
    }
}