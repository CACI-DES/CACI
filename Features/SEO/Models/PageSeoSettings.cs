using System;

namespace CACI_Life.Features.SEO.Models
{
    public class PageSeoSettings
    {
        public int Id { get; set; }
        public string PagePath { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public string? ImageUrl { get; set; }
        public string MetaRobots { get; set; } = "index, follow";
        public string? JsonLdSchema { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
