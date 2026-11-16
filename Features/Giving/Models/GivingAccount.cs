using System;

namespace CACI_Life.Features.Giving.Models
{
    public class GivingAccount
    {
        public int Id { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string AccessLevel { get; set; } = "All";
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
    }
}
