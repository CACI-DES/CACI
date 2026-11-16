using System.Threading.Tasks;
using System.Collections.Generic;

namespace CACI_Life.Features.Volunteers
{
    public interface IVolunteerSurveyRepository
    {
        Task InitializeDatabaseAsync();
        Task<int> SaveEntryAsync(VolunteerSurveyEntry entry);
        Task<VolunteerSurveyEntry?> GetEntryByIdAsync(int id);
        Task<IEnumerable<VolunteerSurveyEntry>> GetAllEntriesAsync();
        Task UpdateEntryAsync(VolunteerSurveyEntry entry);
        Task<string?> GetTeamLeaderEmailAsync(string teamKey);

        // Organization members CRUD
        Task<IEnumerable<OrgMember>> GetAllOrgMembersAsync();
        Task<int> SaveOrgMemberAsync(OrgMember member);
        Task UpdateOrgMemberAsync(OrgMember member);
        Task DeleteOrgMemberAsync(int id);
    }
}
