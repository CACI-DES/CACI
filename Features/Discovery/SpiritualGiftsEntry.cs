using System;
using System.ComponentModel.DataAnnotations;

namespace CACI_Life.Features.Discovery
{
    public class SpiritualGiftsEntry
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty; // Male or Female

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        // Spiritual Gift Scores (0-35)
        [Range(0, 35)] public int Administration { get; set; }
        [Range(0, 35)] public int Apostleship { get; set; }
        [Range(0, 35)] public int Discernment { get; set; }
        [Range(0, 35)] public int Evangelism { get; set; }
        [Range(0, 35)] public int Exhortation { get; set; }
        [Range(0, 35)] public int Faith { get; set; }
        [Range(0, 35)] public int Giving { get; set; }
        [Range(0, 35)] public int Knowledge { get; set; }
        [Range(0, 35)] public int Leadership { get; set; }
        [Range(0, 35)] public int Mercy { get; set; }
        [Range(0, 35)] public int Pastoring { get; set; }
        [Range(0, 35)] public int Prophecy { get; set; }
        [Range(0, 35)] public int Serving { get; set; }
        [Range(0, 35)] public int Teaching { get; set; }
        [Range(0, 35)] public int Wisdom { get; set; }
    }
}
