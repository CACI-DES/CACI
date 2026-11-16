using System.Collections.Generic;
using System.Threading.Tasks;
using CACI_Life.Features.Giving.Models;

namespace CACI_Life.Features.Giving.Services
{
    public interface IGivingRepository
    {
        Task<IEnumerable<GivingAccount>> GetActiveAccountsAsync(string? role = null);
        Task<IEnumerable<GivingAccount>> GetAllAccountsAsync();
        Task<int> CreateAccountAsync(string name, string accessLevel, int displayOrder);
        Task ToggleAccountActiveAsync(int id, bool isActive);
        Task<int> CreateGivingAsync(OnlineGiving giving);
        Task UpdateStripeSessionIdAsync(int id, string sessionId);
        Task<OnlineGiving?> GetGivingBySessionIdAsync(string sessionId);
        Task<int> CompleteGivingPaymentAsync(string sessionId, string paymentIntentId);
        Task<IEnumerable<OnlineGiving>> GetGivingHistoryByEmailAsync(string email);
    }
}
