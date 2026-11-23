#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace CACI_Life.Features.MinisterialApplication;

public interface IMinisterialApplicationRepository
{
    Task<MinisterialApplicationRecord?> GetByDraftKeyAsync(string draftKey, CancellationToken cancellationToken = default);
    Task<MinisterialApplicationRecord?> GetLatestDraftByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<MinisterialApplicationRecord> SaveDraftAsync(MinisterialApplicationRecord record, CancellationToken cancellationToken = default);
    Task SubmitApplicationAsync(int id, MinisterialApplicationData data, CancellationToken cancellationToken = default);
}
