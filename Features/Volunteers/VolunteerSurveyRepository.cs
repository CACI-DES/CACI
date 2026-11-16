using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace CACI_Life.Features.Volunteers
{
    public class VolunteerSurveyRepository : IVolunteerSurveyRepository
    {
        private readonly string _connectionString;

        public VolunteerSurveyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection string is not configured.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public Task InitializeDatabaseAsync() => Task.CompletedTask;

        public async Task<int> SaveEntryAsync(VolunteerSurveyEntry entry)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO CACI.VolunteerSurveys (
                    Team, FullName, PhoneNumber, EmailAddress, PreferredContact, MembershipLength,
                    OtherMinistries, Testimony, FeastPreference, Availability, AvailabilityOther,
                    AgeGroupPreference, GiftsTalentsSkills, GiftsExperienceText, ExperienceText,
                    AreasOfInterest, ComfortLevelChoice, OpenToTraining, HeartVision1, HeartVision2,
                    Restrictions, SpecificRole, FinalStepChoice, PdfFilePath, SubmissionDate
                ) VALUES (
                    @Team, @FullName, @PhoneNumber, @EmailAddress, @PreferredContact, @MembershipLength,
                    @OtherMinistries, @Testimony, @FeastPreference, @Availability, @AvailabilityOther,
                    @AgeGroupPreference, @GiftsTalentsSkills, @GiftsExperienceText, @ExperienceText,
                    @AreasOfInterest, @ComfortLevelChoice, @OpenToTraining, @HeartVision1, @HeartVision2,
                    @Restrictions, @SpecificRole, @FinalStepChoice, @PdfFilePath, @SubmissionDate
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            return await connection.ExecuteScalarAsync<int>(sql, entry);
        }

        public async Task<VolunteerSurveyEntry?> GetEntryByIdAsync(int id)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<VolunteerSurveyEntry>(
                "SELECT * FROM CACI.VolunteerSurveys WHERE Id = @Id",
                new { Id = id }
            );
        }

        public async Task<IEnumerable<VolunteerSurveyEntry>> GetAllEntriesAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<VolunteerSurveyEntry>(
                "SELECT * FROM CACI.VolunteerSurveys ORDER BY SubmissionDate DESC"
            );
        }

        public async Task UpdateEntryAsync(VolunteerSurveyEntry entry)
        {
            using var connection = CreateConnection();
            const string sql = @"
                UPDATE CACI.VolunteerSurveys SET
                    Team = @Team, FullName = @FullName, PhoneNumber = @PhoneNumber, EmailAddress = @EmailAddress,
                    PreferredContact = @PreferredContact, MembershipLength = @MembershipLength, OtherMinistries = @OtherMinistries,
                    Testimony = @Testimony, FeastPreference = @FeastPreference, Availability = @Availability,
                    AvailabilityOther = @AvailabilityOther, AgeGroupPreference = @AgeGroupPreference,
                    GiftsTalentsSkills = @GiftsTalentsSkills, GiftsExperienceText = @GiftsExperienceText,
                    ExperienceText = @ExperienceText, AreasOfInterest = @AreasOfInterest, ComfortLevelChoice = @ComfortLevelChoice,
                    OpenToTraining = @OpenToTraining, HeartVision1 = @HeartVision1, HeartVision2 = @HeartVision2,
                    Restrictions = @Restrictions, SpecificRole = @SpecificRole, FinalStepChoice = @FinalStepChoice,
                    PdfFilePath = @PdfFilePath
                WHERE Id = @Id";
            
            await connection.ExecuteAsync(sql, entry);
        }

        public async Task<string?> GetTeamLeaderEmailAsync(string teamKey)
        {
            var members = await GetAllOrgMembersAsync();
            return members.FirstOrDefault(m => 
                m.TeamAssigned.Equals(teamKey, StringComparison.OrdinalIgnoreCase) && 
                m.RoleInTeam.Equals("Leader", StringComparison.OrdinalIgnoreCase))?.CACIEmail;
        }

        public async Task<IEnumerable<OrgMember>> GetAllOrgMembersAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<OrgMember>(
                "SELECT * FROM CACI.Organization ORDER BY TeamAssigned ASC, LastName ASC, FirstName ASC"
            );
        }

        public async Task<int> SaveOrgMemberAsync(OrgMember member)
        {
            using var connection = CreateConnection();
            if (member.Id > 0)
            {
                const string updateSql = @"
                    UPDATE CACI.Organization SET
                        Title = @Title, FirstName = @FirstName, LastName = @LastName,
                        TeamAssigned = @TeamAssigned, RoleInTeam = @RoleInTeam, CACIEmail = @CACIEmail
                    WHERE Id = @Id";
                await connection.ExecuteAsync(updateSql, member);
                return member.Id;
            }
            else
            {
                const string insertSql = @"
                    INSERT INTO CACI.Organization (Title, FirstName, LastName, TeamAssigned, RoleInTeam, CACIEmail)
                    VALUES (@Title, @FirstName, @LastName, @TeamAssigned, @RoleInTeam, @CACIEmail);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";
                return await connection.ExecuteScalarAsync<int>(insertSql, member);
            }
        }

        public async Task UpdateOrgMemberAsync(OrgMember member)
        {
            await SaveOrgMemberAsync(member);
        }

        public async Task DeleteOrgMemberAsync(int id)
        {
            using var connection = CreateConnection();
            await connection.ExecuteAsync("DELETE FROM CACI.Organization WHERE Id = @Id", new { Id = id });
        }
    }
}
