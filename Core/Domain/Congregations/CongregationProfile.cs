#nullable enable
namespace CACI_Life.Core.Domain.Congregations;

public sealed class CongregationProfile
{
    public int Id { get; init; } = 1;
    public string CongregationName { get; init; } = "Kingdom Covenant Apostolic";
    public string UrlSlug { get; init; } = "kingdomcovenantapostolic";
    public bool IsActive { get; init; } = true;
    public string? GivingUrl { get; init; }
    public string? LivestreamUrl { get; init; }
    public string? CustomDomain { get; init; }
    public string? PastorName { get; init; } = "Eldership";
    public string? ServiceSchedule { get; init; } = "Sabbath Prayer 12:00 PM · Service 12:30 PM (Central Time)";
    public string? StreetAddress { get; init; }
    public string? CityStateZip { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Zip { get; init; }
    public string? WelcomeMessage { get; init; }
    public string? LogoUrl { get; init; } = "/logos/KCA_Logo.png";
    public string? HeroTagline { get; init; } = "Restoring the Original Apostolic Faith";
    public string? Subtitle { get; init; } = "A Covenant Apostolic Congregation";
    public string? RegionDescription { get; init; } = "A Covenant Apostolic Congregation";
    public string? FacebookUrl { get; init; }
    public string? YouTubeUrl { get; init; }
    public string? ContactEmail { get; init; }
    public string? ContactPhone { get; init; }
    public string? GoogleAnalyticsId { get; init; }
    public string? GoogleSearchVerification { get; init; }

    public bool LinkStudiesToCaciLife { get; set; } = true;
    public string CaciBaseUrl { get; set; } = "https://caci.life";

    public string FullAddress =>
        !string.IsNullOrWhiteSpace(CityStateZip)
            ? (!string.IsNullOrWhiteSpace(StreetAddress) ? $"{StreetAddress}, {CityStateZip}" : CityStateZip)
            : string.Join(", ", new[] { StreetAddress, City, State, Zip }.Where(s => !string.IsNullOrWhiteSpace(s)));

    public string EffectiveLogo =>
        !string.IsNullOrWhiteSpace(LogoUrl) ? LogoUrl : "/logos/KCA_Logo.png";

    public string EffectiveGivingUrl =>
        !string.IsNullOrWhiteSpace(GivingUrl) ? GivingUrl : $"{CaciBaseUrl}/giving?church={UrlSlug}";

    public string CanonicalBaseUrl =>
        !string.IsNullOrWhiteSpace(CustomDomain) 
            ? (CustomDomain.StartsWith("http") ? CustomDomain : $"https://{CustomDomain}").TrimEnd('/')
            : $"{CaciBaseUrl}/congregation/{UrlSlug}";
}
