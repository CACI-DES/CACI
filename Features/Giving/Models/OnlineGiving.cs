using System;

namespace CACI_Life.Features.Giving.Models
{
    public class OnlineGiving
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string? DonorName { get; set; }
        public string? UserRole { get; set; }
        public decimal TitheAmount { get; set; }
        public decimal OfferingAmount { get; set; }
        public decimal DonationAmount { get; set; }
        public decimal MinistryDuesAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string? StripeSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public string? AllocationsJson { get; set; }
        public string? Memo { get; set; }
        public bool CoversFee { get; set; }
        public decimal FeeAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    public class GivingAllocation
    {
        public string AccountName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
