using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Stripe;
using Stripe.Checkout;
using CACI_Life.Features.Giving.Models;

namespace CACI_Life.Features.Giving.Services
{
    public class GivingStripeService : IGivingStripeService
    {
        private readonly IConfiguration _configuration;

        public GivingStripeService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<string> GetSettingValueAsync(string key, string defaultValue)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var val = await connection.QueryFirstOrDefaultAsync<string>(
                    "dbo.Giving_usp_GetSiteSetting",
                    new { Key = key },
                    commandType: CommandType.StoredProcedure
                );
                return val ?? defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public async Task<Session> CreateGivingSessionAsync(OnlineGiving model, string successUrl, string cancelUrl)
        {
            var apiKey = await GetSettingValueAsync("StripeSecretKey", _configuration["Stripe:SecretKey"] ?? string.Empty);
            var requestOptions = new RequestOptions { ApiKey = apiKey };

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = successUrl + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = cancelUrl,
                CustomerEmail = model.UserEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "giving_id", model.Id.ToString() }
                }
            };

            if (!string.IsNullOrEmpty(model.AllocationsJson))
            {
                try
                {
                    var allocations = System.Text.Json.JsonSerializer.Deserialize<List<GivingAllocation>>(model.AllocationsJson);
                    if (allocations != null)
                    {
                        foreach (var alloc in allocations)
                        {
                            if (alloc.Amount > 0)
                            {
                                options.LineItems.Add(CreateLineItem(alloc.AccountName, (long)(alloc.Amount * 100), 1));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing allocations JSON in Stripe Service: {ex.Message}");
                }
            }

            // Fallback to legacy fields if no line items were created
            if (options.LineItems.Count == 0)
            {
                if (model.TitheAmount > 0)
                    options.LineItems.Add(CreateLineItem("Tithing", (long)(model.TitheAmount * 100), 1));
                
                if (model.OfferingAmount > 0)
                    options.LineItems.Add(CreateLineItem("Offering", (long)(model.OfferingAmount * 100), 1));
                
                if (model.DonationAmount > 0)
                    options.LineItems.Add(CreateLineItem("Generous Donation", (long)(model.DonationAmount * 100), 1));
                
                if (model.MinistryDuesAmount > 0)
                    options.LineItems.Add(CreateLineItem("Ministry Dues (Ordination Card)", (long)(model.MinistryDuesAmount * 100), 1));
            }

            // Add Processing Fee if covered
            if (model.FeeAmount > 0)
            {
                options.LineItems.Add(CreateLineItem("Card Processing Fee (6%)", (long)(model.FeeAmount * 100), 1));
            }

            var service = new SessionService();
            return await service.CreateAsync(options, requestOptions);
        }

        public async Task<Session> GetSessionAsync(string sessionId)
        {
            var apiKey = await GetSettingValueAsync("StripeSecretKey", _configuration["Stripe:SecretKey"] ?? string.Empty);
            var requestOptions = new RequestOptions { ApiKey = apiKey };

            var service = new SessionService();
            return await service.GetAsync(sessionId, requestOptions: requestOptions);
        }

        private SessionLineItemOptions CreateLineItem(string name, long unitAmount, int quantity)
        {
            return new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = unitAmount,
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = name,
                    },
                },
                Quantity = quantity,
            };
        }
    }
}
