using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using CACI_Life.Features.SEO.Models;

namespace CACI_Life.Features.SEO.Services
{
    public class SeoRepository : ISeoRepository
    {
        private readonly string _connectionString;

        public SeoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("DefaultConnection string is not configured.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<PageSeoSettings?> GetSeoByPathAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) path = "/";

            try
            {
                using var connection = CreateConnection();

                // 1. Direct path lookup
                var seo = await connection.QueryFirstOrDefaultAsync<PageSeoSettings>(
                    "SELECT TOP 1 * FROM CACI.PageSeoSettings WHERE PagePath = @PagePath",
                    new { PagePath = path }
                );

                if (seo != null) return seo;

                // 2. Fallback to root "/" if different
                if (path != "/")
                {
                    seo = await connection.QueryFirstOrDefaultAsync<PageSeoSettings>(
                        "SELECT TOP 1 * FROM CACI.PageSeoSettings WHERE PagePath = '/'",
                        new { PagePath = "/" }
                    );
                }

                return seo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeoRepository] Error querying SEO for path '{path}': {ex.Message}");
                return null;
            }
        }
    }
}
