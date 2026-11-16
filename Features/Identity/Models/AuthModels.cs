using System;
using System.Collections.Generic;

namespace CACI_Life.Features.Identity.Models;

public class AuthResponse
{
    public bool Succeeded { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public int? ChurchId { get; set; }
    public List<string> Roles { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class MinistryProfileDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserId { get; set; }
}

public class RequestAccessModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Congregation { get; set; }
    public string? Notes { get; set; }
}

public class RequestAccessResult
{
    public bool Succeeded { get; set; }
    public string? ErrorMessage { get; set; }
}

