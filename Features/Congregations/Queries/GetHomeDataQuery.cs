#nullable enable
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using CACI_Life.Core.Domain.Congregations;

namespace CACI_Life.Features.Congregations.Queries;

public sealed class AnnouncementDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string NoticeMarkdown { get; init; } = string.Empty;
    public string BadgeText { get; init; } = string.Empty;
    public int Priority { get; init; }
}

public sealed class HomeFeastDto
{
    public int Id { get; init; }
    public string FeastName { get; init; } = string.Empty;
    public string? HebrewName { get; init; }
    public string ScriptureReference { get; init; } = string.Empty;
    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; init; }
    public string SummaryMarkdown { get; init; } = string.Empty;
    public string? LinkedStudySlug { get; init; }
    public string? AssemblyInstructions { get; init; }
    public bool IsGatheringLocally { get; init; } = true;
}

public sealed class HomeFoundationDto
{
    public int Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public int? SeriesOrder { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string ScriptureReference { get; init; } = string.Empty;
    public string LeadSummary { get; init; } = string.Empty;
}

public sealed record HomeData(
    IReadOnlyList<AnnouncementDto> Announcements,
    HomeFeastDto? NextFeast,
    IReadOnlyList<HomeFoundationDto> Foundations)
{
    public static HomeData Empty { get; } = new([], null, []);
}

public sealed class GetHomeDataQuery(
    IDbConnectionFactory connectionFactory, 
    ICongregationService congregationService,
    ILogger<GetHomeDataQuery> logger)
{
    public async Task<HomeData> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await congregationService.GetCurrentAsync(cancellationToken);
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "CACI.usp_GetChurchHomeData",
                new { ChurchId = profile.Id, NowUtc = DateTime.UtcNow },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await using var grid = await connection.QueryMultipleAsync(command);
            var announcements = (await grid.ReadAsync<AnnouncementDto>()).ToList();
            var feast = await grid.ReadFirstOrDefaultAsync<HomeFeastDto>();
            var foundations = (await grid.ReadAsync<HomeFoundationDto>()).ToList();
            return new HomeData(announcements, feast, foundations);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "CACI.usp_GetChurchHomeData failed.");
            return HomeData.Empty;
        }
    }
}
