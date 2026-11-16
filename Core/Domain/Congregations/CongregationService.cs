#nullable enable
using System.Data;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CACI_Life.Core.Domain.Congregations;

public interface ICongregationService
{
    Task<CongregationProfile> GetCurrentAsync(CancellationToken cancellationToken = default);
    Task<CongregationProfile?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CongregationProfile>> GetAllActiveAsync(CancellationToken cancellationToken = default);
    int ConfiguredCongregationId { get; }
    string ConfiguredSlug { get; }
}

public sealed class CongregationService(
    IDbConnectionFactory connectionFactory,
    IConfiguration configuration,
    TenantContext tenantContext,
    IMemoryCache cache,
    ILogger<CongregationService> logger) : ICongregationService
{
    public int ConfiguredCongregationId =>
        configuration.GetValue<int?>("Congregation:Id") ?? 1;

    public string ConfiguredSlug =>
        configuration.GetValue<string>("Congregation:Slug") ?? "kingdomcovenantapostolic";

    public async Task<CongregationProfile?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var cleanSlug = slug.Trim().ToLowerInvariant();
        var cacheKey = $"CACI_Church_{cleanSlug}";

        if (cache.TryGetValue<CongregationProfile>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "CACI.usp_GetChurch",
                new 
                { 
                    Id = (int?)null, 
                    Slug = cleanSlug, 
                    CustomDomain = (string?)null 
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var profile = await connection.QueryFirstOrDefaultAsync<CongregationProfile>(command);
            if (profile is not null)
            {
                profile.LinkStudiesToCaciLife = configuration.GetValue<bool?>("Congregation:LinkStudiesToCaciLife") ?? true;
                profile.CaciBaseUrl = (configuration.GetValue<string>("Congregation:CaciBaseUrl") ?? "https://caci.life").TrimEnd('/');

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                cache.Set(cacheKey, profile, cacheEntryOptions);
                return profile;
            }

            logger.LogWarning("No church found in CACI.Churches for Slug='{Slug}'.", cleanSlug);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to load church for slug '{Slug}' from CACI DB.", cleanSlug);
        }

        return null;
    }

    public async Task<CongregationProfile> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var resolvedSlug = tenantContext.CongregationSlug;
        var resolvedDomain = tenantContext.CustomDomain;

        var effectiveSlug = resolvedSlug ?? (resolvedDomain is null ? ConfiguredSlug : null);
        var cacheKey = $"CACI_Church_{resolvedDomain ?? effectiveSlug ?? ConfiguredCongregationId.ToString()}";

        if (cache.TryGetValue<CongregationProfile>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "CACI.usp_GetChurch",
                new 
                { 
                    Id = (resolvedSlug is null && resolvedDomain is null) ? (int?)ConfiguredCongregationId : null, 
                    Slug = effectiveSlug, 
                    CustomDomain = resolvedDomain 
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var profile = await connection.QueryFirstOrDefaultAsync<CongregationProfile>(command);
            if (profile is not null)
            {
                profile.LinkStudiesToCaciLife = configuration.GetValue<bool?>("Congregation:LinkStudiesToCaciLife") ?? true;
                profile.CaciBaseUrl = (configuration.GetValue<string>("Congregation:CaciBaseUrl") ?? "https://caci.life").TrimEnd('/');

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                cache.Set(cacheKey, profile, cacheEntryOptions);
                return profile;
            }

            logger.LogWarning("No church found in CACI.Churches for Slug='{Slug}', Domain='{Domain}', FallbackId={Id}.", effectiveSlug, resolvedDomain, ConfiguredCongregationId);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to load church from CACI DB. Using fallback.");
        }

        // Resilient Fallback
        return new CongregationProfile
        {
            Id = ConfiguredCongregationId,
            CongregationName = configuration.GetValue<string>("Congregation:Fallback:Name") ?? "Kingdom Covenant Apostolic",
            UrlSlug = ConfiguredSlug,
            PastorName = configuration.GetValue<string>("Congregation:Fallback:PastorName") ?? "Eldership",
            ServiceSchedule = configuration.GetValue<string>("Congregation:Fallback:ServiceSchedule") ?? "Sabbath Prayer 12:00 PM · Service 12:30 PM (Central Time)",
            RegionDescription = configuration.GetValue<string>("Congregation:Fallback:RegionDescription") ?? "A Covenant Apostolic Congregation",
            WelcomeMessage = configuration.GetValue<string>("Congregation:Fallback:WelcomeMessage") ?? "Welcome to our fellowship.",
            LogoUrl = configuration.GetValue<string>("Congregation:Fallback:LogoUrl") ?? "/logos/KCA_Logo.png",
            LinkStudiesToCaciLife = configuration.GetValue<bool?>("Congregation:LinkStudiesToCaciLife") ?? true,
            CaciBaseUrl = (configuration.GetValue<string>("Congregation:CaciBaseUrl") ?? "https://caci.life").TrimEnd('/')
        };
    }

    public async Task<IReadOnlyList<CongregationProfile>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        const string cacheKey = "CACI_All_Active_Churches";
        if (cache.TryGetValue<IReadOnlyList<CongregationProfile>>(cacheKey, out var cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "CACI.usp_GetAllActiveChurches",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var list = (await connection.QueryAsync<CongregationProfile>(command)).ToList();
            var baseUrl = (configuration.GetValue<string>("Congregation:CaciBaseUrl") ?? "https://caci.life").TrimEnd('/');
            foreach (var item in list)
            {
                item.CaciBaseUrl = baseUrl;
            }

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            cache.Set(cacheKey, (IReadOnlyList<CongregationProfile>)list, cacheOptions);
            return list;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to query active churches from CACI.Churches.");
            return [];
        }
    }
}
