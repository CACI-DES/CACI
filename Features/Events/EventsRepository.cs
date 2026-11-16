using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using CACI_Life.Features.Events.Models;

namespace CACI_Life.Features.Events
{
    public class EventsRepository : IEventsRepository
    {
        private readonly string _connectionString;

        public string? LastError { get; private set; }

        public EventsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? string.Empty;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<ConferenceModel?> GetActiveConferenceAsync(int? conferenceId = null)
        {
            LastError = null;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                LastError = "DefaultConnection string is empty or missing in appsettings.json.";
                Console.WriteLine($"[EventsRepository] {LastError}");
                return null;
            }

            try
            {
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();

                if (conferenceId.HasValue && conferenceId.Value > 0)
                {
                    parameters.Add("@ConferenceId", conferenceId.Value);
                }

                // 1. Try Stored Procedure [CACI].[sp_GetActiveConference]
                try
                {
                    var result = await connection.QueryFirstOrDefaultAsync<ConferenceModel>(
                        "[CACI].[sp_GetActiveConference]", 
                        parameters, 
                        commandType: CommandType.StoredProcedure
                    );

                    if (result != null) return result;
                }
                catch (Exception spEx)
                {
                    Console.WriteLine($"[EventsRepository] SP [CACI].[sp_GetActiveConference] note: {spEx.Message}");
                }

                // 2. Try Stored Procedure [dbo].[sp_GetActiveConference]
                try
                {
                    var resultDbo = await connection.QueryFirstOrDefaultAsync<ConferenceModel>(
                        "sp_GetActiveConference", 
                        parameters, 
                        commandType: CommandType.StoredProcedure
                    );

                    if (resultDbo != null) return resultDbo;
                }
                catch
                {
                    // Ignore SP fallback error
                }

                // 3. Direct SELECT Fallbacks on CACI.Conferences and dbo.Conferences
                string[] schemas = new[] { "[CACI]", "[dbo]" };
                foreach (var schema in schemas)
                {
                    try
                    {
                        if (conferenceId.HasValue && conferenceId.Value > 0)
                        {
                            var sqlById = $"SELECT TOP 1 * FROM {schema}.[Conferences] WHERE [Id] = @Id";
                            var confById = await connection.QueryFirstOrDefaultAsync<ConferenceModel>(sqlById, new { Id = conferenceId.Value });
                            if (confById != null) return confById;
                        }

                        var sqlActive = $"SELECT TOP 1 * FROM {schema}.[Conferences] WHERE [IsActive] = 1 ORDER BY [StartDate] DESC";
                        var confActive = await connection.QueryFirstOrDefaultAsync<ConferenceModel>(sqlActive);
                        if (confActive != null) return confActive;

                        var sqlLatest = $"SELECT TOP 1 * FROM {schema}.[Conferences] ORDER BY [StartDate] DESC";
                        var confLatest = await connection.QueryFirstOrDefaultAsync<ConferenceModel>(sqlLatest);
                        if (confLatest != null) return confLatest;
                    }
                    catch (Exception queryEx)
                    {
                        LastError = $"Direct query fallback on {schema}.Conferences: {queryEx.Message}";
                        Console.WriteLine($"[EventsRepository] {LastError}");
                    }
                }

                if (string.IsNullOrEmpty(LastError))
                {
                    LastError = "No active conference record was found in the database.";
                }

                return null;
            }
            catch (Exception ex)
            {
                LastError = $"Connection/Query exception: {ex.Message}";
                Console.WriteLine($"[EventsRepository] {LastError}");
                return null;
            }
        }

        public async Task<IEnumerable<ConferenceSession>> GetSessionsForConferenceAsync(int conferenceId)
        {
            if (string.IsNullOrWhiteSpace(_connectionString) || conferenceId <= 0) 
            {
                return Array.Empty<ConferenceSession>();
            }

            try
            {
                using var connection = CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@ConferenceId", conferenceId);

                // 1. Try SP [CACI].[sp_GetConferenceSessions]
                try
                {
                    var spSessions = await connection.QueryAsync<ConferenceSession>(
                        "[CACI].[sp_GetConferenceSessions]", 
                        parameters, 
                        commandType: CommandType.StoredProcedure
                    );

                    if (spSessions != null && spSessions.Any())
                    {
                        return spSessions;
                    }
                }
                catch (Exception spEx)
                {
                    Console.WriteLine($"[EventsRepository] SP [CACI].[sp_GetConferenceSessions] note: {spEx.Message}");
                }

                // 2. Try SP [dbo].[sp_GetConferenceSessions]
                try
                {
                    var spSessionsDbo = await connection.QueryAsync<ConferenceSession>(
                        "sp_GetConferenceSessions", 
                        parameters, 
                        commandType: CommandType.StoredProcedure
                    );

                    if (spSessionsDbo != null && spSessionsDbo.Any())
                    {
                        return spSessionsDbo;
                    }
                }
                catch
                {
                    // Ignore SP fallback error
                }

                // 3. Direct SELECT Fallbacks on CACI.ConferenceSessions and dbo.ConferenceSessions
                string[] schemas = new[] { "[CACI]", "[dbo]" };
                foreach (var schema in schemas)
                {
                    try
                    {
                        var sql = $"SELECT * FROM {schema}.[ConferenceSessions] WHERE [ConferenceId] = @ConferenceId ORDER BY [SessionTime] ASC";
                        var sessions = await connection.QueryAsync<ConferenceSession>(sql, new { ConferenceId = conferenceId });
                        if (sessions != null && sessions.Any())
                        {
                            return sessions;
                        }
                    }
                    catch (Exception queryEx)
                    {
                        Console.WriteLine($"[EventsRepository] Direct query fallback on {schema}.ConferenceSessions: {queryEx.Message}");
                    }
                }

                return Array.Empty<ConferenceSession>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EventsRepository] Error in GetSessionsForConferenceAsync: {ex.Message}");
                return Array.Empty<ConferenceSession>();
            }
        }

        public async Task<IEnumerable<SponsorPackageModel>> GetSponsorPackagesAsync()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                return Array.Empty<SponsorPackageModel>();
            }

            try
            {
                using var connection = CreateConnection();

                // 1. Try SP [EVENTS].[usp_GetSponsorPackages]
                try
                {
                    var packages = await connection.QueryAsync<SponsorPackageModel>(
                        "EVENTS.usp_GetSponsorPackages",
                        commandType: CommandType.StoredProcedure
                    );

                    if (packages != null && packages.Any())
                    {
                        return packages;
                    }
                }
                catch (Exception spEx)
                {
                    Console.WriteLine($"[EventsRepository] SP [EVENTS].[usp_GetSponsorPackages] note: {spEx.Message}");
                }

                // 2. Direct Query Fallback
                string[] schemas = new[] { "[EVENTS]", "[CACI]", "[dbo]" };
                foreach (var schema in schemas)
                {
                    try
                    {
                        var sql = $"SELECT PackageName, PackagePrice, PackageDescription, IsActive FROM {schema}.[SponsorPackages] WHERE [IsActive] = 1 ORDER BY [PackagePrice] ASC";
                        var packages = await connection.QueryAsync<SponsorPackageModel>(sql);
                        if (packages != null && packages.Any())
                        {
                            return packages;
                        }
                    }
                    catch { }
                }

                return Array.Empty<SponsorPackageModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EventsRepository] Error in GetSponsorPackagesAsync: {ex.Message}");
                return Array.Empty<SponsorPackageModel>();
            }
        }
    }
}
