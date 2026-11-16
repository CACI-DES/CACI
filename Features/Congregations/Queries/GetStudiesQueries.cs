#nullable enable
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using CACI_Life.Core.Domain.Congregations;

namespace CACI_Life.Features.Congregations.Queries;

public sealed class StudySummaryDto
{
    public int Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string ScriptureReference { get; init; } = string.Empty;
    public string LeadSummary { get; init; } = string.Empty;
    public string? PdfDownloadUrl { get; init; }
    public string? AuthorByline { get; init; }
    public string? YouTubeVideoId { get; init; }
    public DateTime PublishedUtc { get; init; }
}

public sealed class StudyDetailDto
{
    public int Id { get; init; }
    public string Slug { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int? SeriesOrder { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Subtitle { get; init; }
    public string ScriptureReference { get; init; } = string.Empty;
    public string LeadSummary { get; init; } = string.Empty;
    public string BodyMarkdown { get; init; } = string.Empty;
    public string? YouTubeVideoId { get; init; }
    public string? PdfDownloadUrl { get; init; }
    public string AuthorByline { get; init; } = string.Empty;
    public DateTime PublishedUtc { get; init; }
    public DateTime UpdatedUtc { get; init; }
}

public sealed class StudyFaqDto
{
    public int Id { get; init; }
    public string Question { get; init; } = string.Empty;
    public string AnswerMarkdown { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}

public sealed record StudyDetailResult(StudyDetailDto Study, IReadOnlyList<StudyFaqDto> Faqs);

public static class StudyDetailLoader
{
    public static async Task<StudyDetailResult?> LoadAsync(
        IDbConnectionFactory connectionFactory, ILogger logger, string slug, CancellationToken cancellationToken)
    {
        try
        {
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "Church.usp_GetStudyBySlug",
                new { Slug = slug },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await using var grid = await connection.QueryMultipleAsync(command);
            var study = await grid.ReadFirstOrDefaultAsync<StudyDetailDto>();
            if (study is null) return null;
            var faqs = (await grid.ReadAsync<StudyFaqDto>()).ToList();
            return new StudyDetailResult(study, faqs);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "usp_GetStudyBySlug failed for slug {Slug}.", slug);
            return null;
        }
    }
}

public sealed class GetStudiesQueries(
    IDbConnectionFactory connectionFactory, 
    ICongregationService congregationService,
    ILogger<GetStudiesQueries> logger)
{
    public async Task<IReadOnlyList<StudySummaryDto>> ListAsync(string? category = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await congregationService.GetCurrentAsync(cancellationToken);
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            var command = new CommandDefinition(
                "Church.usp_GetCongregationStudiesList",
                new { CongregationId = profile.Id, Category = category },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
            return (await connection.QueryAsync<StudySummaryDto>(command)).ToList();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "usp_GetCongregationStudiesList failed (category {Category}).", category);
            return [];
        }
    }

    public Task<StudyDetailResult?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        StudyDetailLoader.LoadAsync(connectionFactory, logger, slug, cancellationToken);
}
