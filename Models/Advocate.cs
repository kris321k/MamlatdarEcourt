using System.ComponentModel.DataAnnotations;

namespace MamlatdarEcourt.Models
{
    public class Advocate
    {
        public int Id {get; set;}
        [Required]
        public string? BarNumber{get;set;}
    }
}