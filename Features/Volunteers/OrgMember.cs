using System.ComponentModel.DataAnnotations;

namespace CACI_Life.Features.Volunteers
{
    public class OrgMember
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Team Assigned is required.")]
        [StringLength(100)]
        public string TeamAssigned { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role In Team is required.")]
        [StringLength(100)]
        public string RoleInTeam { get; set; } = string.Empty;

        [Required(ErrorMessage = "CACI Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [StringLength(100)]
        public string CACIEmail { get; set; } = string.Empty;
    }
}
