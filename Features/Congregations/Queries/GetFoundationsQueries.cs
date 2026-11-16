#nullable enable
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using CACI_Life.Core.Domain.Congregations;

namespace CACI_Life.Features.Congregations.Queries;

public sealed class FoundationSummaryDto
{
    public int Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public int? SeriesOrder { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string ScriptureReference { get; init; } = string.Empty;
    public string LeadSummary { get; init; } = string.Empty;
    public string? PdfDownloadUrl { get; init; }
}

public sealed class GetFoundationsQueries(IDbConnectionFactory connectionFactory, ILogger<GetFoundationsQueries> logger)
{
    public async Task<IReadOnlyList<FoundationSummaryDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "Church.usp_GetFoundationsList",
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
            return (await connection.QueryAsync<FoundationSummaryDto>(command)).ToList();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "usp_GetFoundationsList failed.");
            return [];
        }
    }

    public Task<StudyDetailResult?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        StudyDetailLoader.LoadAsync(connectionFactory, logger, slug, cancellationToken);
}
