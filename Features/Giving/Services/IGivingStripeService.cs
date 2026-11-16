using System.Threading.Tasks;
using Stripe.Checkout;
using CACI_Life.Features.Giving.Models;

namespace CACI_Life.Features.Giving.Services
{
    public interface IGivingStripeService
    {
        Task<Session> CreateGivingSessionAsync(OnlineGiving model, string successUrl, string cancelUrl, string? destinationStripeAccountId = null, string? churchSlug = null);
        Task<Session> GetSessionAsync(string sessionId);
    }
}
