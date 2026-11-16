using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace CACI_Life.Features.Discovery
{
    public class SpiritualGiftsRepository : ISpiritualGiftsRepository
    {
        private readonly string _connectionString;

        public SpiritualGiftsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("DefaultConnection string is not configured.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public Task InitializeDatabaseAsync() => Task.CompletedTask;

        public async Task<int> SaveEntryAsync(SpiritualGiftsEntry entry)
        {
            using var connection = CreateConnection();
            const string sql = @"
                INSERT INTO CACI.SpiritualGiftsEntries (
                    FirstName, LastName, Email, DateOfBirth, Gender, SubmissionDate,
                    Administration, Apostleship, Discernment, Evangelism, Exhortation,
                    Faith, Giving, Knowledge, Leadership, Mercy, Pastoring, Prophecy,
                    Serving, Teaching, Wisdom
                ) VALUES (
                    @FirstName, @LastName, @Email, @DateOfBirth, @Gender, @SubmissionDate,
                    @Administration, @Apostleship, @Discernment, @Evangelism, @Exhortation,
                    @Faith, @Giving, @Knowledge, @Leadership, @Mercy, @Pastoring, @Prophecy,
                    @Serving, @Teaching, @Wisdom
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await connection.ExecuteScalarAsync<int>(sql, entry);
        }

        public async Task<IEnumerable<SpiritualGiftsEntry>> GetAllEntriesAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<SpiritualGiftsEntry>(
                "SELECT * FROM CACI.SpiritualGiftsEntries ORDER BY SubmissionDate DESC"
            );
        }

        public async Task<SpiritualGiftsEntry?> GetEntryByIdAsync(int id)
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SpiritualGiftsEntry>(
                "SELECT * FROM CACI.SpiritualGiftsEntries WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}
