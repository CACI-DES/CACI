using System;
using System.Collections.Generic;

namespace CACI.Core.Domain.Funnels;

public record ScriptureAnchor(string Ref, string Desc);

public record FunnelItem(
    int Order,
    string Badge,             // e.g. "Key 01", "Pillar 02", "Foundation 07"
    string Title,             // e.g. "Kingdom Law", "Apostolic Order"
    string Slug,              // e.g. "kingdom-law", "apostolic-order"
    string Subtitle,          // Short thematic tag
    string ShortSummary,      // Card view text (~20 words)
    string HeaderLead,        // Expanded drawer headline
    string FullExposition,    // Deep doctrinal exposition for SEO & seekers
    string TargetStudyKey,    // Passed to caci.life/request-bible-study?key=...
    List<ScriptureAnchor> Scriptures
);

public record FunnelFaq(string Question, string Answer);

public record FunnelConfiguration(
    string FunnelId,          // "4keys", "4pillars", "12foundations"
    string CanonicalUrl,      // "https://4keys.caci.life/" or "https://caci.life/foundations"
    string CategoryBadge,     // "Official Kingdom Orientation"
    string MainTitle,         // "Unlock Your Path to The Kingdom"
    string SubtitleLead,      // "Step beyond religion and tradition..."
    string GridColumnsClass,  // "grid-cols-1 md:grid-cols-2" vs "grid-cols-1 md:grid-cols-3"
    List<FunnelItem> Items,
    List<FunnelFaq> Faqs,
    string StudyRequestBaseUrl = "/request-bible-study",
    string OgImageUrl = "https://caci.life/images/brand-standards/AllTheWord_TextLogo.png",
    string Keywords = ""
);
