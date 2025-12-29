using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Identity;

namespace MamlatdarEcourt.Models
{
    public enum Taluka
    {
        Ponda,
        Bardez
    }
    public class User : IdentityUser
    {

        public string? FirstName{get; set;}

        [Required]
        public string? LastName{get; set;}

        [AllowedValues("Ponda","Pernem","Bardez")]
        public string Taluka{get; set;} = string.Empty;


        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB{get; set;}

        
    }
}