using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace CACI.Core.Domain.Funnels;

public enum SubsiteType
{
    Main,
    FourKeys,
    FourPillars,
    TwelveFoundations
}

public interface ISubsiteContext
{
    bool IsSubsite { get; }
    SubsiteType Type { get; }
    FunnelConfiguration? Configuration { get; }
    string SubsiteName { get; }
    string ParentSiteUrl { get; }
    string SubsiteBadge { get; }
}

public class SubsiteContextService : ISubsiteContext
{
    private readonly NavigationManager _navigationManager;

    public SubsiteContextService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public SubsiteType Type
    {
        get
        {
            try
            {
                var uri = new Uri(_navigationManager.Uri);
                var host = uri.Host.ToLowerInvariant();

                // 1. Host header / domain detection (e.g. 4keys.caci.life, 4keys.localhost)
                if (host.StartsWith("4keys.") || host.Equals("4keys.caci.life", StringComparison.OrdinalIgnoreCase))
                {
                    return SubsiteType.FourKeys;
                }
                if (host.StartsWith("pillars.") || host.StartsWith("4pillars.") || host.Equals("pillars.caci.life", StringComparison.OrdinalIgnoreCase))
                {
                    return SubsiteType.FourPillars;
                }
                if (host.StartsWith("foundations.") || host.StartsWith("12foundations.") || host.Equals("foundations.caci.life", StringComparison.OrdinalIgnoreCase))
                {
                    return SubsiteType.TwelveFoundations;
                }

                // 2. Query parameter override (ideal for local testing: ?subsite=4keys)
                if (!string.IsNullOrEmpty(uri.Query))
                {
                    var query = QueryHelpers.ParseQuery(uri.Query);
                    if (query.TryGetValue("subsite", out var subsiteVal))
                    {
                        var val = subsiteVal.ToString().ToLowerInvariant();
                        if (val == "4keys" || val == "keys") return SubsiteType.FourKeys;
                        if (val == "4pillars" || val == "pillars") return SubsiteType.FourPillars;
                        if (val == "12foundations" || val == "foundations") return SubsiteType.TwelveFoundations;
                    }
                }
            }
            catch
            {
                // Fallback to Main on any URI parse error
            }

            return SubsiteType.Main;
        }
    }

    public bool IsSubsite => Type != SubsiteType.Main;

    public FunnelConfiguration? Configuration => Type switch
    {
        SubsiteType.FourKeys => FunnelPresets.Get4KeysConfig(isSubsite: true),
        SubsiteType.FourPillars => FunnelPresets.Get4PillarsConfig(isSubsite: true),
        SubsiteType.TwelveFoundations => FunnelPresets.Get12FoundationsConfig(isSubsite: true),
        _ => null
    };

    public string SubsiteName => Type switch
    {
        SubsiteType.FourKeys => "4 Keys of the Kingdom",
        SubsiteType.FourPillars => "The 4 Pillars",
        SubsiteType.TwelveFoundations => "The 12 Foundations",
        _ => "CACI Life"
    };

    public string SubsiteBadge => Type switch
    {
        SubsiteType.FourKeys => "4keys.caci.life",
        SubsiteType.FourPillars => "pillars.caci.life",
        SubsiteType.TwelveFoundations => "foundations.caci.life",
        _ => "caci.life"
    };

    public string ParentSiteUrl => "https://caci.life";
}
