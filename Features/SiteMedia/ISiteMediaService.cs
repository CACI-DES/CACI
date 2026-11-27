namespace CACI_Life.Features.SiteMedia;

public interface ISiteMediaService
{
    Task<IReadOnlyDictionary<string, SiteMediaItem>> GetAllAsync(CancellationToken cancellationToken = default);
}
