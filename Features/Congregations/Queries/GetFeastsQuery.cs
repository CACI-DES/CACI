#nullable enable
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using CACI_Life.Core.Domain.Congregations;

namespace CACI_Life.Features.Congregations.Queries;

public sealed class FeastDto
{
    public int Id { get; init; }
    public string FeastName { get; init; } = string.Empty;
    public string? HebrewName { get; init; }
    public string ScriptureReference { get; init; } = string.Empty;
    public DateTime StartUtc { get; init; }
    public DateTime EndUtc { get; init; }
    public string SummaryMarkdown { get; init; } = string.Empty;
    public string? AssemblyInstructions { get; init; }
    public string? LinkedStudySlug { get; init; }
    public bool IsGatheringLocally { get; init; } = true;

    public bool IsCurrent(DateTime nowUtc) => StartUtc <= nowUtc && nowUtc < EndUtc;
}

public sealed class GetFeastsQuery(
    IDbConnectionFactory connectionFactory, 
    ICongregationService congregationService,
    ILogger<GetFeastsQuery> logger)
{
    public async Task<IReadOnlyList<FeastDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await congregationService.GetCurrentAsync(cancellationToken);
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "CACI.usp_GetChurchFeasts",
                new { ChurchId = profile.Id, YearStartUtc = DateTime.UtcNow.Date },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
            return (await connection.QueryAsync<FeastDto>(command)).ToList();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "CACI.usp_GetChurchFeasts failed.");
            return [];
        }
    }
}
