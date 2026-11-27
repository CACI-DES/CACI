using System.Data;
using CACI_Life.Core.Domain.Congregations;
using Dapper;
using Microsoft.Extensions.Caching.Memory;

namespace CACI_Life.Features.SiteMedia;

public sealed class SiteMediaService(
    IDbConnectionFactory connectionFactory,
    IMemoryCache cache,
    ILogger<SiteMediaService> logger) : ISiteMediaService
{
    private const string CacheKey = "CACI_SiteMedia_All";
    private static readonly IReadOnlyDictionary<string, SiteMediaItem> EmptyMap =
        new Dictionary<string, SiteMediaItem>(StringComparer.OrdinalIgnoreCase);

    public async Task<IReadOnlyDictionary<string, SiteMediaItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out IReadOnlyDictionary<string, SiteMediaItem>? cached) && cached is not null)
        {
            return cached;
        }

        try
        {
            using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var rows = await conn.QueryAsync<SiteMediaItem>(
                new CommandDefinition(
                    "CACI.usp_SiteMedia_GetAll",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));

            var dict = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.SlotKey))
                .GroupBy(r => r.SlotKey, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            cache.Set(CacheKey, (IReadOnlyDictionary<string, SiteMediaItem>)dict, TimeSpan.FromSeconds(30));
            return dict;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Unable to load CACI.SiteMedia; falling back to built-in default media.");
            cache.Set(CacheKey, EmptyMap, TimeSpan.FromSeconds(15));
            return EmptyMap;
        }
    }
}
