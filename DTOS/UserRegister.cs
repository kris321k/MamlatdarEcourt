using System.ComponentModel.DataAnnotations;
using System.Data;
using MamlatdarEcourt.Models;

namespace MamlatdarEcourt.DTOS
{
    public class UserRegister
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]

        public string Password { get; set; } = string.Empty;

        [Required]

        [AllowedValues("Ponda", "Pernem", "Bardez")]
        public string Taluka { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]

        public DateTime DOB { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
    }
}