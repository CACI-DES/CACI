using System;
using System.ComponentModel.DataAnnotations;

namespace CACI_Life.Features.Volunteers
{
    public class VolunteerSurveyEntry
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Team is required.")]
        public string Team { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required.")]
        [StringLength(50, ErrorMessage = "Phone Number cannot exceed 50 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100, ErrorMessage = "Email Address cannot exceed 100 characters.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Preferred contact method is required.")]
        [StringLength(50)]
        public string PreferredContact { get; set; } = string.Empty; // Call, Text, Email

        [Required(ErrorMessage = "Membership length is required.")]
        [StringLength(100)]
        public string MembershipLength { get; set; } = string.Empty;

        [StringLength(255)]
        public string? OtherMinistries { get; set; }

        public string? Testimony { get; set; }

        [Required(ErrorMessage = "Feast preference is required.")]
        [StringLength(50)]
        public string FeastPreference { get; set; } = string.Empty; // EFC, Tabernacles, Both, Any

        public string? Availability { get; set; } // Comma-separated list of checked availability options

        public string? AvailabilityOther { get; set; } // Other availability text

        public string? AgeGroupPreference { get; set; } // Comma-separated list for Youth

        public string? GiftsTalentsSkills { get; set; } // Comma-separated list of selected skills

        public string? GiftsExperienceText { get; set; } // Youth specific text description

        public string? ExperienceText { get; set; } // General experience text description

        public string? AreasOfInterest { get; set; } // Comma-separated list of interests

        public string? ComfortLevelChoice { get; set; } // Comma-separated list of comfort settings

        [Required(ErrorMessage = "Please select an option for training.")]
        [StringLength(50)]
        public string OpenToTraining { get; set; } = "Maybe"; // Yes, No, Maybe

        public string? HeartVision1 { get; set; } // Heart/Vision answer 1

        public string? HeartVision2 { get; set; } // Heart/Vision answer 2

        public string? Restrictions { get; set; }

        [StringLength(255)]
        public string? SpecificRole { get; set; }

        [Required(ErrorMessage = "Please answer the background check question.")]
        [StringLength(50)]
        public string FinalStepChoice { get; set; } = string.Empty; // Yes, No

        public string? PdfFilePath { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        public string GetPdfFileName()
        {
            var sanitizedName = string.Join("", FullName.Split(System.IO.Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries))
                .Replace(" ", "_");
            return $"CACI_Survey_{Team}_{sanitizedName}_{Id}.pdf";
        }
    }
}
