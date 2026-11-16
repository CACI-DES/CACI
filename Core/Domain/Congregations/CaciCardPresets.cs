namespace CACI_Life.Core.Domain.Congregations;

public record CaciPillarItem(
    string Title,
    string Description,
    string IconClass,
    string BorderClass
);

public record CaciKeyItem(
    string KeyWord,
    string KeySubtitle,
    string IconClass,
    string LinkUrl,
    string Description = ""
);

public static class CaciCardPresets
{
    public static readonly IReadOnlyList<CaciPillarItem> Pillars = new List<CaciPillarItem>
    {
        new(
            Title: "Apostles Doctrine",
            Description: "The foundation of all faith, restoring the original Apostolic teachings.",
            IconClass: "fas fa-book-open",
            BorderClass: "border-t-caci-blue"
        ),
        new(
            Title: "Apostolic Fellowship",
            Description: "Building strong, spiritual bonds through ordination and maintaining fellowship one with another.",
            IconClass: "fas fa-handshake",
            BorderClass: "border-t-caci-purple"
        ),
        new(
            Title: "Breaking of Bread",
            Description: "Recognizing and participating in the sacred ordinance of communion, remembering Christ's sacrifice.",
            IconClass: "fas fa-bread-slice",
            BorderClass: "border-t-amber-500"
        ),
        new(
            Title: "Apostolic Prayer",
            Description: "Engaging in fervent, authoritative prayer to advance the Kingdom and see spiritual transformation.",
            IconClass: "fas fa-hands-praying",
            BorderClass: "border-t-[#C8102E]"
        )
    };

    public static readonly IReadOnlyList<CaciKeyItem> Keys = new List<CaciKeyItem>
    {
        new(
            KeyWord: "The Coming",
            KeySubtitle: "of \u00a0The Kingdom",
            IconClass: "fas fa-crown",
            LinkUrl: "/4keys#keys-grid",
            Description: "God’s ultimate promise is not an escape from creation, but the literal return of Jesus Christ to restore His righteous Kingdom across the entire earth."
        ),
        new(
            KeyWord: "The Law",
            KeySubtitle: "of \u00a0The Kingdom",
            IconClass: "fas fa-book-open",
            LinkUrl: "/4keys#keys-grid",
            Description: "The unchanging covenant God spoke aloud at Sinai is now written on the hearts of believers—not to earn salvation, but as the eternal standard of His reign."
        ),
        new(
            KeyWord: "The Culture",
            KeySubtitle: "of \u00a0The Kingdom",
            IconClass: "fas fa-users",
            LinkUrl: "/4keys#keys-grid",
            Description: "Through the seventh-day Sabbath, biblical feasts, and God’s instructions for daily living, believers reflect the identity and rhythm of their true homeland."
        ),
        new(
            KeyWord: "The Gospel",
            KeySubtitle: "of \u00a0The Kingdom",
            IconClass: "fas fa-bullhorn",
            LinkUrl: "/4keys#keys-grid",
            Description: "Through repentance, water baptism in Jesus’s name, and the infilling of the Holy Spirit, strangers are rescued from sin and grafted into the covenant family of Israel."
        )
    };
}
