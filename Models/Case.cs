using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    public enum DisputeCategory
    {
        Tenancy,
        LandMutation,
        PropertyDispute,
        RevenueAppeal,
        AgriculturalIssue
    }

    public class Case
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string CaseNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DisputeCategory DisputeCategory { get; set; }

        // Foreign Key
        [Required]
        public string ApplicantId { get; set; } = string.Empty;

        [ForeignKey(nameof(ApplicantId))]
        public User? Applicant { get; set; }

        public CaseStatus Status { get; set; } = CaseStatus.Filed;

        public DateTime FiledDate { get; set; } = DateTime.UtcNow;
    }
}