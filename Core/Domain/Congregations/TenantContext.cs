#nullable enable
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CACI_Life.Core.Domain.Congregations;

public sealed class TenantContext
{
    private string? _explicitSlug;

    public string? CongregationSlug
    {
        get => _explicitSlug ?? _inferredSlug;
        set => _explicitSlug = value?.Trim().ToLowerInvariant();
    }

    private string? _inferredSlug;
    public string? CustomDomain { get; private set; }
    public bool IsCongregationSite => !string.IsNullOrEmpty(CongregationSlug) || !string.IsNullOrEmpty(CustomDomain);

    public void SetSlug(string? slug)
    {
        if (!string.IsNullOrWhiteSpace(slug))
        {
            _explicitSlug = slug.Trim().ToLowerInvariant();
        }
    }

    public TenantContext(IHttpContextAccessor accessor, IConfiguration config)
    {
        var ctx = accessor.HttpContext;
        if (ctx is null) return;

        // 1. Path detection: /congregation/{slug} or /congregation/{slug}/...
        var path = ctx.Request.Path.Value;
        if (!string.IsNullOrEmpty(path))
        {
            var segments = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0].Equals("congregation", StringComparison.OrdinalIgnoreCase))
            {
                _inferredSlug = segments[1].Trim().ToLowerInvariant();
                return;
            }
        }

        // 2. Query param override for testing & local development (?church=keystone or ?c=keystone)
        var querySlug = ctx.Request.Query["church"].ToString();
        if (string.IsNullOrWhiteSpace(querySlug))
        {
            querySlug = ctx.Request.Query["c"].ToString();
        }

        if (!string.IsNullOrWhiteSpace(querySlug))
        {
            _inferredSlug = querySlug.Trim().ToLowerInvariant();
            return;
        }

        // 3. Reverse Proxy / Cloudflare Header or Host
        var host = ctx.Request.Headers["X-Original-Host"].FirstOrDefault()
            ?? ctx.Request.Headers["X-Forwarded-Host"].FirstOrDefault()
            ?? ctx.Request.Host.Host;

        if (host.Contains(':'))
        {
            host = host.Split(':')[0];
        }

        host = host.Trim().ToLowerInvariant();

        // Subdomain extraction: *.caci.life (e.g. keystone.caci.life)
        if (host.EndsWith(".caci.life", StringComparison.OrdinalIgnoreCase))
        {
            var sub = host[..^".caci.life".Length];
            if (sub is not ("www" or "4keys" or "pillars" or "foundations" or ""))
            {
                _inferredSlug = sub;
            }
        }
        else if (!host.Equals("localhost", StringComparison.OrdinalIgnoreCase) &&
                 !host.Equals("127.0.0.1") &&
                 !host.EndsWith("site4now.net", StringComparison.OrdinalIgnoreCase) &&
                 !host.EndsWith("qtempurl.com", StringComparison.OrdinalIgnoreCase))
        {
            // Custom domain like "thekeystone.life"
            CustomDomain = host;
        }
    }
}
