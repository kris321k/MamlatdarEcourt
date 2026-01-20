using System.ComponentModel.DataAnnotations;

namespace MamlatdarEcourt.Models
{
    public class Advocate
    {
        public string Id {get; set;}
        [Required]
        public string? BarNumber{get;set;}
    }
}