using System.Collections.Generic;
using System.Threading.Tasks;
using CACI_Life.Features.Events.Models;

namespace CACI_Life.Features.Events
{
    public interface IEventsRepository
    {
        string? LastError { get; }
        Task<ConferenceModel?> GetActiveConferenceAsync(int? conferenceId = null);
        Task<IEnumerable<ConferenceSession>> GetSessionsForConferenceAsync(int conferenceId);
        Task<IEnumerable<SponsorPackageModel>> GetSponsorPackagesAsync();
    }
}
