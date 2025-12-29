using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace MamlatdarEcourt.Models
{
    public enum CaseStatus
    {
        Filed,
        UnderReview,
        ScheduledForHearing,
        HearingInProgress,
        JudgmentPassed,
        Closed

    }



    public class Case
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string DisputeCategory { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = String.Empty;

        [ForeignKey(nameof(UserId))]

        public User? Applicant { get; set; }
        public CaseStatus Status { get; set; } = CaseStatus.Filed;

    }
}