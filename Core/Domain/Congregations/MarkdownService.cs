#nullable enable
using Markdig;

namespace CACI_Life.Core.Domain.Congregations;

public interface IMarkdownService
{
    string ToHtml(string? markdown);
}

public sealed class MarkdownService : IMarkdownService
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    public string ToHtml(string? markdown) =>
        string.IsNullOrWhiteSpace(markdown) ? string.Empty : Markdown.ToHtml(markdown, _pipeline);
}
