
using System.ComponentModel.DataAnnotations;
using MamlatdarEcourt.Models;

namespace MamlatdarEcourt.DTOS
{

    public class CaseRegisterDto
    {
        [MaxLength(50)]
        public string CaseNumber { get; set; } = string.Empty;
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DisputeCategory DisputeCategory { get; set; }

        // Foreign Key

    }
}