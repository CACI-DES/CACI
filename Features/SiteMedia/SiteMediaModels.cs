namespace CACI_Life.Features.SiteMedia;

public sealed class SiteMediaItem
{
    public string SlotKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PageGroup { get; set; } = string.Empty;
    public string MediaType { get; set; } = "Image";
    public string MediaUrl { get; set; } = string.Empty;
    public string DefaultUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public static class SiteMediaKeys
{
    public const string HomeHeroVideo = "home.hero_video";
    public const string HomeHeroVideoPoster = "home.hero_video_poster";
    public const string HomeLeadershipImage = "home.leadership_image";
    public const string HomeInvolvedBibleStudy = "home.involved_bible_study";
    public const string HomeInvolvedSabbath = "home.involved_sabbath";
    public const string HomeCountdownBackground = "home.countdown_background";
    public const string MinistryEldersImage = "ministry.elders_image";
    public const string MinistryBishopsImage = "ministry.bishops_image";
    public const string MinistryMinistersImage = "ministry.ministers_image";
    public const string EventsCalendarGlanceImage = "events.calendar_glance_image";
    public const string StudyBibleStudyImage = "study.bible_study_image";
    public const string DocumentsFestivalLetterThumb = "documents.festival_letter_thumb";
    public const string DocumentsSabbathLetterThumb = "documents.sabbath_letter_thumb";
    public const string FooterBackgroundImage = "footer.background_image";
}

public static class SiteMediaHelper
{
    private static readonly IReadOnlyDictionary<string, (string Url, string Alt)> Defaults =
        new Dictionary<string, (string Url, string Alt)>(StringComparer.OrdinalIgnoreCase)
        {
            [SiteMediaKeys.HomeHeroVideo] = ("/images/images/banner1.mp4", "CACI Home Header Video"),
            [SiteMediaKeys.HomeHeroVideoPoster] = ("/images/images/placeholder_video.jpg", "CACI Header Poster"),
            [SiteMediaKeys.HomeLeadershipImage] = ("/images/images/BishopWhite_EFC2025.jpg", "Presiding Bishop Image"),
            [SiteMediaKeys.HomeInvolvedBibleStudy] = ("/images/images/onlinebiblestudy.jpg", "Online Bible Study"),
            [SiteMediaKeys.HomeInvolvedSabbath] = ("/images/images/ElderClifton_Preaching2.jpg", "Weekly Sabbath Celebration"),
            [SiteMediaKeys.HomeCountdownBackground] = ("/images/images/Sabbath.png", "Feast Countdown Background"),
            [SiteMediaKeys.MinistryEldersImage] = ("/images/images/CACI_Elders.jpg", "CACI Elders"),
            [SiteMediaKeys.MinistryBishopsImage] = ("/images/images/CACI_Bishops.jpg", "CACI Bishops"),
            [SiteMediaKeys.MinistryMinistersImage] = ("/images/images/CACI_Ministers.jpg", "CACI Ministers"),
            [SiteMediaKeys.EventsCalendarGlanceImage] = ("/images/2026_Calendar_Glance.png", "CACI 2026 Calendar At A Glance"),
            [SiteMediaKeys.StudyBibleStudyImage] = ("/images/images/OnlineBibleStudy.jpg", "Bible Study"),
            [SiteMediaKeys.DocumentsFestivalLetterThumb] = ("/images/FestivalLetter_Thumb.png", "Festival Letter"),
            [SiteMediaKeys.DocumentsSabbathLetterThumb] = ("/images/SabbathLetter.avif", "Sabbath Letter"),
            [SiteMediaKeys.FooterBackgroundImage] = ("/images/images/pexels-pixabay-41949.jpg", "Footer Background")
        };

    public static string GetDefaultUrl(string slotKey) =>
        Defaults.TryGetValue(slotKey, out var def) ? def.Url : string.Empty;

    public static string GetImageOnError(string slotKey)
    {
        var fallback = GetDefaultUrl(slotKey).Replace("'", "\\'", StringComparison.Ordinal);
        return $"this.onerror=null;this.src='{fallback}';";
    }

    public static string GetUrl(IReadOnlyDictionary<string, SiteMediaItem> map, string slotKey)
    {
        if (map.TryGetValue(slotKey, out var item) && !string.IsNullOrWhiteSpace(item.MediaUrl))
        {
            return item.MediaUrl.Trim();
        }

        return Defaults.TryGetValue(slotKey, out var def) ? def.Url : string.Empty;
    }

    public static string GetAlt(IReadOnlyDictionary<string, SiteMediaItem> map, string slotKey)
    {
        if (map.TryGetValue(slotKey, out var item) && !string.IsNullOrWhiteSpace(item.AltText))
        {
            return item.AltText.Trim();
        }

        return Defaults.TryGetValue(slotKey, out var def) ? def.Alt : string.Empty;
    }

    public static string GetVideoMimeType(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return "video/mp4";
        var pathOnly = url.Split('?')[0];
        if (pathOnly.EndsWith(".webm", StringComparison.OrdinalIgnoreCase)) return "video/webm";
        if (pathOnly.EndsWith(".mov", StringComparison.OrdinalIgnoreCase)) return "video/quicktime";
        return "video/mp4";
    }

    public static string SanitizeCssUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        return url.Trim()
            .Replace("'", "%27", StringComparison.Ordinal)
            .Replace("(", "%28", StringComparison.Ordinal)
            .Replace(")", "%29", StringComparison.Ordinal)
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Replace("\n", string.Empty, StringComparison.Ordinal);
    }
}
