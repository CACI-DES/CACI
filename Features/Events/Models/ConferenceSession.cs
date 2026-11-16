using System;

namespace CACI_Life.Features.Events.Models;

public class ConferenceSession
{
    public int Id { get; set; }
    public int ConferenceId { get; set; }
    public DateTime SessionTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Speaker { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? SpecialNote { get; set; }
    public string? SessionType { get; set; } // 'Worship', 'Session', 'Meal', 'Registration', 'Training', 'Store', 'Youth'
}
