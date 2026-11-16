using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using CACI_Life.Features.Giving.Models;

namespace CACI_Life.Features.Giving.Services
{
    public class GivingRepository : IGivingRepository
    {
        private readonly string _connectionString;

        public GivingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException("DefaultConnection string is not configured.");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<GivingAccount>> GetActiveAccountsAsync(string? role = null)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserRole", role);
            return await connection.QueryAsync<GivingAccount>(
                "dbo.Giving_usp_GetActiveAccounts",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<GivingAccount>> GetAllAccountsAsync()
        {
            using var connection = CreateConnection();
            return await connection.QueryAsync<GivingAccount>(
                "dbo.Giving_usp_GetAllAccounts",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> CreateAccountAsync(string name, string accessLevel, int displayOrder)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@AccountName", name);
            parameters.Add("@AccessLevel", accessLevel);
            parameters.Add("@DisplayOrder", displayOrder);

            return await connection.QuerySingleAsync<int>(
                "dbo.Giving_usp_CreateAccount",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task ToggleAccountActiveAsync(int id, bool isActive)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@IsActive", isActive);

            await connection.ExecuteAsync(
                "dbo.Giving_usp_ToggleAccountActive",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> CreateGivingAsync(OnlineGiving giving)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserEmail", giving.UserEmail);
            parameters.Add("@DonorName", giving.DonorName);
            parameters.Add("@UserRole", giving.UserRole);
            parameters.Add("@TitheAmount", giving.TitheAmount);
            parameters.Add("@OfferingAmount", giving.OfferingAmount);
            parameters.Add("@DonationAmount", giving.DonationAmount);
            parameters.Add("@MinistryDuesAmount", giving.MinistryDuesAmount);
            parameters.Add("@TotalAmount", giving.TotalAmount);
            parameters.Add("@PaymentStatus", giving.PaymentStatus);
            parameters.Add("@AllocationsJson", giving.AllocationsJson);
            parameters.Add("@Memo", giving.Memo);
            parameters.Add("@CoversFee", giving.CoversFee);
            parameters.Add("@FeeAmount", giving.FeeAmount);
            parameters.Add("@ChurchId", giving.ChurchId);
            parameters.Add("@ChurchSlug", giving.ChurchSlug);

            return await connection.QuerySingleAsync<int>(
                "dbo.Giving_usp_CreateGiving",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task UpdateStripeSessionIdAsync(int id, string sessionId)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@StripeSessionId", sessionId);

            await connection.ExecuteAsync(
                "dbo.Giving_usp_UpdateStripeSessionId",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<OnlineGiving?> GetGivingBySessionIdAsync(string sessionId)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SessionId", sessionId);

            return await connection.QueryFirstOrDefaultAsync<OnlineGiving>(
                "dbo.Giving_usp_GetBySessionId",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> CompleteGivingPaymentAsync(string sessionId, string paymentIntentId)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@SessionId", sessionId);
            parameters.Add("@PaymentIntentId", paymentIntentId);

            return await connection.QuerySingleAsync<int>(
                "dbo.Giving_usp_CompletePayment",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<OnlineGiving>> GetGivingHistoryByEmailAsync(string email)
        {
            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);

            return await connection.QueryAsync<OnlineGiving>(
                "dbo.Giving_usp_GetHistoryByEmail",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
