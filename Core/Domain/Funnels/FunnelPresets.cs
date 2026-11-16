using System.Collections.Generic;

namespace CACI.Core.Domain.Funnels;

public static class FunnelPresets
{
    public static FunnelConfiguration Get4KeysConfig(bool isSubsite = false)
    {
        return new FunnelConfiguration(
            FunnelId: "4keys",
            CanonicalUrl: isSubsite ? "https://4keys.caci.life/" : "https://caci.life/4KeysInfo",
            CategoryBadge: "Official Kingdom Orientation",
            MainTitle: "Unlock Your Path to The Kingdom",
            SubtitleLead: "Step beyond religion and human tradition into the original Apostolic understanding of God's coming earthly reign, His righteous law, sacred culture, and the true Gospel.",
            GridColumnsClass: "grid-cols-1 md:grid-cols-2",
            Items: new List<FunnelItem>
            {
                new FunnelItem(
                    Order: 1,
                    Badge: "Key 01",
                    Title: "The Coming of the Kingdom",
                    Slug: "the-coming-kingdom",
                    Subtitle: "Restoration of the Earth",
                    ShortSummary: "The scriptures never declare eternity in heaven to be the reward of the saints. God's Kingdom will be restored over all the earth.",
                    HeaderLead: "The Prophetic Blueprint of Earthly Restoration",
                    FullExposition: "From Genesis to Revelation, the Holy Scriptures consistently proclaim the physical renewal of creation and the literal earthly reign of the Messiah. Christ taught His disciples to pray 'Thy Kingdom come, Thy will be done in earth, as it is in heaven.' At the return of the Messiah, the righteous dead will be resurrected into incorruptible, glorified bodies to govern the nations alongside Christ in an everlasting, righteous dominion.",
                    TargetStudyKey: "4keys-coming-kingdom",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Matthew 5:5", "Blessed are the meek: for they shall inherit the earth."),
                        new ScriptureAnchor("Revelation 5:10", "And hast made us unto our God kings and priests: and we shall reign on the earth."),
                        new ScriptureAnchor("Daniel 7:27", "And the kingdom and dominion, and the greatness of the kingdom under the whole heaven, shall be given to the people of the saints of the most High.")
                    }
                ),
                new FunnelItem(
                    Order: 2,
                    Badge: "Key 02",
                    Title: "The Law of the Kingdom",
                    Slug: "the-law-of-the-kingdom",
                    Subtitle: "Torah Written on Hearts",
                    ShortSummary: "Messiah is the Lawgiver who writes God's commandments upon our hearts, establishing righteousness in the New Covenant.",
                    HeaderLead: "The Eternal Moral Constitution of Zion",
                    FullExposition: "Modern religious tradition often paints Jesus as a lawless King who came to abolish God's holy standards. In contrast, the New Covenant explicitly fulfills Jeremiah's prophecy: the Law written into the minds and hearts of believers by the Holy Spirit. Messiah did not come to destroy the Law or the Prophets, but to demonstrate their true spiritual intent. The Ten Commandments, including the 7th-day Sabbath, remain the eternal standard of Kingdom righteousness.",
                    TargetStudyKey: "4keys-kingdom-law",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Jeremiah 31:33", "I will put my law in their inward parts, and write it in their hearts; and will be their God, and they shall be my people."),
                        new ScriptureAnchor("Matthew 5:17-19", "Think not that I am come to destroy the law, or the prophets: I am not come to destroy, but to fulfil."),
                        new ScriptureAnchor("Isaiah 2:3", "For out of Zion shall go forth the law, and the word of the LORD from Jerusalem.")
                    }
                ),
                new FunnelItem(
                    Order: 3,
                    Badge: "Key 03",
                    Title: "The Culture of the Kingdom",
                    Slug: "the-culture-of-the-kingdom",
                    Subtitle: "Covenant Lifestyle & Feasts",
                    ShortSummary: "Living as sanctified citizens through Biblical appointed feasts, dietary holiness, and a consecrated household order.",
                    HeaderLead: "Consecrated Life in a Fallen Age",
                    FullExposition: "Kingdom citizenship produces an identifiable, sanctified way of life distinct from worldly culture and pagan syncretism. Believers are called out to honor the Lord's appointed times (moedim)—such as Passover, Pentecost, and Tabernacles—and to maintain holiness in speech, dietary discernment, and family structure. As a royal priesthood, our households function as active embassies representing King Jesus.",
                    TargetStudyKey: "4keys-kingdom-culture",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Leviticus 23:2", "Concerning the feasts of the LORD, which ye shall proclaim to be holy convocations, even these are my feasts."),
                        new ScriptureAnchor("1 Peter 2:9", "But ye are a chosen generation, a royal priesthood, an holy nation, a peculiar people."),
                        new ScriptureAnchor("Romans 12:2", "And be not conformed to this world: but be ye transformed by the renewing of your mind.")
                    }
                ),
                new FunnelItem(
                    Order: 4,
                    Badge: "Key 04",
                    Title: "The Gospel of the Kingdom",
                    Slug: "the-gospel-of-the-kingdom",
                    Subtitle: "The Good News of His Reign",
                    ShortSummary: "The royal proclamation preached by Yeshua and the Apostles: the government of God has drawn near to redeem mankind.",
                    HeaderLead: "The Imperial Announcement of King Messiah",
                    FullExposition: "The Gospel is far more than an individual escape ticket to the clouds. It is the royal, historic announcement that God's sovereign King has arrived to break the powers of darkness, redeem fallen humanity through His shed blood, and re-establish the government of God upon earth. Repentance and baptism into the Name of Jesus Christ mark our allegiance to His crown and citizenship in His realm.",
                    TargetStudyKey: "4keys-kingdom-gospel",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Mark 1:14-15", "Jesus came into Galilee, preaching the gospel of the kingdom of God, and saying, The time is fulfilled, and the kingdom of God is at hand."),
                        new ScriptureAnchor("Matthew 24:14", "And this gospel of the kingdom shall be preached in all the world for a witness unto all nations; and then shall the end come."),
                        new ScriptureAnchor("Acts 8:12", "When they believed Philip preaching the things concerning the kingdom of God, and the name of Jesus Christ, they were baptized.")
                    }
                )
            },
            Faqs: new List<FunnelFaq>
            {
                new FunnelFaq(
                    Question: "What are the 4 Keys of the Kingdom?",
                    Answer: "The 4 Keys represent foundational biblical doctrines restored in Apostolic understanding: The Coming Kingdom (earthly resurrection and reign), The Law of the Kingdom (Torah written on hearts), The Culture of the Kingdom (biblical feasts and holy living), and The Gospel of the Kingdom (the royal proclamation of Messiah's sovereign government)."
                ),
                new FunnelFaq(
                    Question: "Does Scripture teach believers spend eternity in heaven?",
                    Answer: "No. From the Genesis creation mandate through Revelation 21-22, God's ultimate plan is the redemption of this physical earth. Christ taught that 'the meek shall inherit the earth' (Matt 5:5), and Revelation 5:10 confirms the saints will 'reign on the earth.' Heaven is the present throne of God, but the New Jerusalem descends to earth."
                ),
                new FunnelFaq(
                    Question: "How do I participate in a guided 4 Keys Bible study?",
                    Answer: "Click the 'Request Guided Study' button on any key card or visit our Bible Study Request page. A CACI study coordinator will reach out to connect you with an online small group or personal mentor."
                )
            },
            StudyRequestBaseUrl: isSubsite ? "https://caci.life/request-bible-study" : "/request-bible-study",
            Keywords: "4 Keys of the Kingdom, Kingdom of God, Apostolic Faith, Law of God, Sabbath, Kingdom Culture, Hebrew Roots, Biblical Feasts, CACI, Gospel of the Kingdom, Earthly Reign, Messiah, Covenant Apostolic Congregations International"
        );
    }

    public static FunnelConfiguration Get4PillarsConfig(bool isSubsite = false)
    {
        return new FunnelConfiguration(
            FunnelId: "4pillars",
            CanonicalUrl: isSubsite ? "https://pillars.caci.life/" : "https://caci.life/4PillarsInfo",
            CategoryBadge: "Apostolic Order & Church Government",
            MainTitle: "The 4 Pillars of the Apostolic Faith",
            SubtitleLead: "Restoring the original first-century assembly pattern, holy orders, and sacramental life as established by Christ and His Apostles in Acts 2:42.",
            GridColumnsClass: "grid-cols-1 md:grid-cols-2",
            Items: new List<FunnelItem>
            {
                new FunnelItem(
                    Order: 1,
                    Badge: "Pillar 01",
                    Title: "Apostolic Government & Holy Orders",
                    Slug: "apostolic-government",
                    Subtitle: "Presbyteries, Bishops & Deacons",
                    ShortSummary: "The restoring of biblical Presbyteries, Bishops, Elders, and Deacons operating without denominational bureaucracy.",
                    HeaderLead: "Divine Blueprint for Order and Spiritual Authority",
                    FullExposition: "The New Testament church was not governed by corporate committees or autonomous individualism, but by a recognized presbytery of Bishops, Elders, and Deacons in covenant fellowship. Apostolic government provides biblical oversight, protects assemblies against false doctrine, and empowers leaders without oppressive denominational bureaucracy. Titus was specifically charged to 'set in order the things that are wanting, and ordain elders in every city.'",
                    TargetStudyKey: "4pillars-government",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Titus 1:5", "For this cause left I thee in Crete, that thou shouldest set in order the things that are wanting, and ordain elders in every city, as I had appointed thee."),
                        new ScriptureAnchor("1 Timothy 3:1-13", "Qualifications and biblical holy orders for Bishops and Deacons in the house of God."),
                        new ScriptureAnchor("Acts 14:23", "And when they had ordained them elders in every church, and had prayed with fasting, they commended them to the Lord.")
                    }
                ),
                new FunnelItem(
                    Order: 2,
                    Badge: "Pillar 02",
                    Title: "The Apostles' Doctrine",
                    Slug: "apostles-doctrine",
                    Subtitle: "Hebraic Context & Holiness",
                    ShortSummary: "Continual steadfastness in the unadulterated biblical canon, original Hebraic covenant context, and holy living.",
                    HeaderLead: "Unswerving Fidelity to the Faith Once Delivered",
                    FullExposition: "The Apostles were steeped in the Hebrew Scriptures and interpreted the revelation of Yeshua through covenant continuity, not human philosophy. Steadfastness in the Apostles' Doctrine requires unreserved submission to the whole counsel of God, rejection of syncretic traditions, and an uncompromised pursuit of holiness. As Jude exhorted, the church must earnestly contend for the faith once delivered to the saints.",
                    TargetStudyKey: "4pillars-doctrine",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Acts 2:42", "And they continued stedfastly in the apostles' doctrine and fellowship, and in breaking of bread, and in prayers."),
                        new ScriptureAnchor("Jude 1:3", "Earnestly contend for the faith which was once delivered unto the saints."),
                        new ScriptureAnchor("2 Timothy 3:16-17", "All scripture is given by inspiration of God, and is profitable for doctrine, for reproof, for correction, for instruction in righteousness.")
                    }
                ),
                new FunnelItem(
                    Order: 3,
                    Badge: "Pillar 03",
                    Title: "The Covenant Altar & Sacraments",
                    Slug: "covenant-altar-sacraments",
                    Subtitle: "Immersion, Hands & Covenant Cup",
                    ShortSummary: "Proper administration of immersion in water, the laying on of hands, and the covenant bread and cup.",
                    HeaderLead: "Sacramental Reality and Divine Power",
                    FullExposition: "Sacraments are not empty symbolic rituals; they are sacred covenantal acts instituted by Christ with spiritual reality and power. We administer full water immersion in the Name of Jesus Christ for the remission of sins, the laying on of hands for the impartation of the Holy Spirit and ordination, and regular participation in the Lord's Table with solemn reverence and self-examination.",
                    TargetStudyKey: "4pillars-sacraments",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Hebrews 6:1-2", "Of the doctrine of baptisms, and of laying on of hands, and of resurrection of the dead, and of eternal judgment."),
                        new ScriptureAnchor("1 Corinthians 11:23-26", "For as often as ye eat this bread, and drink this cup, ye do shew the Lord's death till he come."),
                        new ScriptureAnchor("Acts 2:38", "Repent, and be baptized every one of you in the name of Jesus Christ for the remission of sins, and ye shall receive the gift of the Holy Ghost.")
                    }
                ),
                new FunnelItem(
                    Order: 4,
                    Badge: "Pillar 04",
                    Title: "The Mobilized Body (Community & Laborers)",
                    Slug: "mobilized-body",
                    Subtitle: "Every Believer a Priest",
                    ShortSummary: "Transitioning from passive spectators to an active, equipped priesthood where every household functions as an embassy of the King.",
                    HeaderLead: "Equipping the Saints for Supernatural Kingdom Work",
                    FullExposition: "The early church rejected spectator religion. Five-fold ministry gifts are given not to do all the ministry, but to equip the saints for the work of service. Every believer is ordained as a priest before God, called to minister within their household, workplace, and community. Discipleship operates relationally from house to house, multiplying disciples and advancing the Kingdom into every realm of life.",
                    TargetStudyKey: "4pillars-mobilized-body",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Ephesians 4:11-16", "For the perfecting of the saints, for the work of the ministry, for the edifying of the body of Christ."),
                        new ScriptureAnchor("1 Peter 2:5", "Ye also, as lively stones, are built up a spiritual house, an holy priesthood, to offer up spiritual sacrifices."),
                        new ScriptureAnchor("Acts 2:46", "And breaking bread from house to house, did eat their meat with gladness and singleness of heart.")
                    }
                )
            },
            Faqs: new List<FunnelFaq>
            {
                new FunnelFaq(
                    Question: "Why are these 4 Pillars essential for the church today?",
                    Answer: "Acts 2:42 provides the foundational architecture of the New Testament assembly. By anchoring in Apostolic Government, Pure Doctrine, Sacramental Reverence, and a Mobilized Priesthood, local assemblies are guarded against cultural compromise, legalism, and bureaucratic decay."
                ),
                new FunnelFaq(
                    Question: "What is the difference between Apostolic government and denominational hierarchy?",
                    Answer: "Denominational hierarchies often depend on political elections, secular management methods, and centralized control. Biblical Apostolic government relies on recognized spiritual fathers, elders, and presbyters who walk in mutual accountability, scriptural qualifications, and pastoral care for the flock."
                ),
                new FunnelFaq(
                    Question: "How can our household or fellowship align with these 4 Pillars?",
                    Answer: "We offer mentoring, regional fellowship support, ordination pathways, and online small groups. Click 'Request Guided Study' to start a dialogue with our leadership team."
                )
            },
            StudyRequestBaseUrl: isSubsite ? "https://caci.life/request-bible-study" : "/request-bible-study",
            Keywords: "4 Pillars of Apostolic Faith, Acts 2:42, Apostles Doctrine, Apostolic Government, Presbytery, Biblical Elders, Holy Orders, Sacraments, Lord's Table, Water Baptism, Priesthood of Believers, CACI"
        );
    }

    public static FunnelConfiguration Get12FoundationsConfig(bool isSubsite = false)
    {
        return new FunnelConfiguration(
            FunnelId: "12foundations",
            CanonicalUrl: isSubsite ? "https://foundations.caci.life/" : "https://caci.life/12Foundations",
            CategoryBadge: "Hermeneutical & Doctrinal Mastery",
            MainTitle: "Understanding Your Bible: The 12 Foundations",
            SubtitleLead: "Unlocking Textual, Historical, and Covenant Hermeneutics to understand the Scriptures from Genesis to Revelation without contradiction.",
            GridColumnsClass: "grid-cols-1 md:grid-cols-2 lg:grid-cols-3",
            Items: new List<FunnelItem>
            {
                // Block I: The Text & Translation
                new FunnelItem(
                    Order: 1,
                    Badge: "Foundation 01",
                    Title: "Inspiration & Preservation",
                    Slug: "inspiration-and-preservation",
                    Subtitle: "Block I: The Text & Translation",
                    ShortSummary: "How God communicated His word through holy prophets and preserved His manuscripts without loss through centuries.",
                    HeaderLead: "The Authority and Integrity of the Written Word",
                    FullExposition: "Examines verbal plenary inspiration, autographa vs. manuscript transmission (Masoretic Text, Septuagint, Dead Sea Scrolls), and how God providentially safeguarded His sacred text so believers can trust the Bible as God's infallible revelation.",
                    TargetStudyKey: "12foundations-inspiration",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("2 Timothy 3:16", "All scripture is given by inspiration of God, and is profitable for doctrine, for reproof, for correction."),
                        new ScriptureAnchor("Psalm 12:6-7", "The words of the LORD are pure words... Thou shalt keep them, O LORD, thou shalt preserve them.")
                    }
                ),
                new FunnelItem(
                    Order: 2,
                    Badge: "Foundation 02",
                    Title: "Textual Trajectories",
                    Slug: "textual-trajectories",
                    Subtitle: "Block I: The Text & Translation",
                    ShortSummary: "Evaluating translation philosophies, textual variants, and discovering the original Semitic thought patterns beneath the text.",
                    HeaderLead: "Navigating Translations and Ancient Manuscripts",
                    FullExposition: "Equipping disciples to analyze formal vs. dynamic equivalence in translation, recognize theological translator bias, cross-reference ancient witnesses, and uncover the original Semitic idioms that unlock difficult biblical passages.",
                    TargetStudyKey: "12foundations-textual-trajectories",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Proverbs 30:5-6", "Every word of God is pure: he is a shield unto them that put their trust in him. Add thou not unto his words."),
                        new ScriptureAnchor("Isaiah 28:10", "For precept must be upon precept, precept upon precept; line upon line, line upon line; here a little, and there a little.")
                    }
                ),
                new FunnelItem(
                    Order: 3,
                    Badge: "Foundation 03",
                    Title: "The Divine Name in Scripture",
                    Slug: "divine-name",
                    Subtitle: "Block I: The Text & Translation",
                    ShortSummary: "Restoring the Tetragrammaton (YHWH), the name of Messiah Yeshua, and understanding covenant identity.",
                    HeaderLead: "The Revelation and Restoration of the Sacred Name",
                    FullExposition: "Investigating why the Memorial Name (YHWH) was substituted over 6,800 times with generic titles like 'the LORD' in English Bibles, and examining how Messiah came in the Father's Name to reveal the fullness of His covenant character.",
                    TargetStudyKey: "12foundations-divine-name",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Exodus 3:15", "The LORD God of your fathers... this is my name for ever, and this is my memorial unto all generations."),
                        new ScriptureAnchor("John 17:6", "I have manifested thy name unto the men which thou gavest me out of the world.")
                    }
                ),

                // Block II: The Covenant Framework
                new FunnelItem(
                    Order: 4,
                    Badge: "Foundation 04",
                    Title: "Unconditional Covenants vs. Conditional Promises",
                    Slug: "covenants-vs-promises",
                    Subtitle: "Block II: Covenant Framework",
                    ShortSummary: "Distinguishing eternal blood-sealed covenants with Abraham and David from conditional performance blessings under Sinai.",
                    HeaderLead: "The Immutable Foundations of Divine Oath",
                    FullExposition: "Understanding how God's unilateral covenants with Abraham and David irrevocably guarantee redemption and land promises, while individual blessings and chastisements hinge on covenant loyalty and obedience.",
                    TargetStudyKey: "12foundations-covenants",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Genesis 15:17-18", "The LORD made a covenant with Abram, saying, Unto thy seed have I given this land."),
                        new ScriptureAnchor("Hebrews 6:17-18", "Wherein God, willing more abundantly to shew unto the heirs of promise the immutability of his counsel, confirmed it by an oath.")
                    }
                ),
                new FunnelItem(
                    Order: 5,
                    Badge: "Foundation 05",
                    Title: "The Commonwealth of Israel",
                    Slug: "commonwealth-of-israel",
                    Subtitle: "Block II: Covenant Framework",
                    ShortSummary: "Gentile believers grafted into the Olive Tree of Israel, co-heirs with the saints without replacement theology.",
                    HeaderLead: "One New Man in the Olive Tree",
                    FullExposition: "Dismantling supersessionism (replacement theology) through Ephesians 2 and Romans 11. Believers from the nations are not a separate entity replacing Israel, but grafted in by Messiah's blood as full citizens of the Commonwealth of Israel.",
                    TargetStudyKey: "12foundations-commonwealth",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Ephesians 2:12-13", "Being aliens from the commonwealth of Israel, and strangers from the covenants of promise... are made nigh by the blood of Christ."),
                        new ScriptureAnchor("Romans 11:17", "And thou, being a wild olive tree, wert grafted in among them, and with them partakest of the root and fatness of the olive tree.")
                    }
                ),
                new FunnelItem(
                    Order: 6,
                    Badge: "Foundation 06",
                    Title: "The Heart of the Torah",
                    Slug: "heart-of-the-torah",
                    Subtitle: "Block II: Covenant Framework",
                    ShortSummary: "The eternal law of God as loving instruction, moral righteousness, and the living constitution of the Kingdom.",
                    HeaderLead: "Instruction in Righteousness, Not Dead Legalism",
                    FullExposition: "Torah does not mean condemnation; it signifies fatherly instruction and teaching. Yeshua confirmed that the entire Law hangs on love for God and love for neighbor, revealing it as the enduring standard of holy conduct for all covenant citizens.",
                    TargetStudyKey: "12foundations-torah",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Psalm 119:97", "O how love I thy law! it is my meditation all the day."),
                        new ScriptureAnchor("Matthew 22:37-40", "On these two commandments hang all the law and the prophets.")
                    }
                ),

                // Block III: The Prophetic Rhythms
                new FunnelItem(
                    Order: 7,
                    Badge: "Foundation 07",
                    Title: "The Sabbath Rest",
                    Slug: "sabbath-rest",
                    Subtitle: "Block III: Prophetic Rhythms",
                    ShortSummary: "The Seventh-Day Sabbath from Creation to Eternity as an everlasting sign between God and His covenant people.",
                    HeaderLead: "The Memorial of Creation and Prophetic Millennial Rehearsal",
                    FullExposition: "Instituted and blessed at Creation before any human division, the Seventh-Day Sabbath stands as an perpetual sign of sanctification. It offers physical rest, sacred community assembly, and a weekly rehearsal of the coming 1,000-year Millennial reign of Messiah.",
                    TargetStudyKey: "12foundations-sabbath",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Genesis 2:2-3", "And God blessed the seventh day, and sanctified it: because that in it he had rested from all his work."),
                        new ScriptureAnchor("Hebrews 4:9", "There remaineth therefore a rest [sabbatismos] to the people of God."),
                        new ScriptureAnchor("Isaiah 66:23", "From one sabbath to another, shall all flesh come to worship before me, saith the LORD.")
                    }
                ),
                new FunnelItem(
                    Order: 8,
                    Badge: "Foundation 08",
                    Title: "The Appointed Feasts as God's Clock",
                    Slug: "appointed-feasts",
                    Subtitle: "Block III: Prophetic Rhythms",
                    ShortSummary: "Passover, Pentecost, Trumpets, Atonement, and Tabernacles as prophetic master plans of redemption.",
                    HeaderLead: "The Moedim: Divine Appointments with Destiny",
                    FullExposition: "The biblical feasts (moedim) are not merely ancient Jewish traditions; they are the prophetic calendar of redemption. The Spring Feasts precisely foreshadowed Messiah's first advent, while the Fall Feasts detail His second coming, the final judgment, and the wedding feast of the Lamb.",
                    TargetStudyKey: "12foundations-feasts",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Leviticus 23:4", "These are the feasts of the LORD, even holy convocations, which ye shall proclaim in their seasons."),
                        new ScriptureAnchor("Colossians 2:16-17", "Which are a shadow of things to come; but the body is of Christ.")
                    }
                ),
                new FunnelItem(
                    Order: 9,
                    Badge: "Foundation 09",
                    Title: "The Millennial Kingdom Vision",
                    Slug: "millennial-kingdom",
                    Subtitle: "Block III: Prophetic Rhythms",
                    ShortSummary: "The literal 1,000-year reign of Messiah in Jerusalem with resurrected saints governing the nations in peace.",
                    HeaderLead: "The Physical Restoration of All Creation",
                    FullExposition: "The climax of biblical prophecy: Messiah's physical return to Mount Zion to depose rebellious world systems, end war, restore ecological harmony, and establish universal justice governed with a rod of iron alongside His resurrected saints.",
                    TargetStudyKey: "12foundations-millennial-kingdom",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Revelation 20:4-6", "And they lived and reigned with Christ a thousand years."),
                        new ScriptureAnchor("Micah 4:1-3", "The mountain of the house of the LORD shall be established in the top of the mountains... and nation shall not lift up a sword against nation.")
                    }
                ),

                // Block IV: The Living Assembly
                new FunnelItem(
                    Order: 10,
                    Badge: "Foundation 10",
                    Title: "Biblical Repentance & Faith",
                    Slug: "repentance-and-faith",
                    Subtitle: "Block IV: The Living Assembly",
                    ShortSummary: "Teshuvah—a wholehearted turning from sin and dead works into unwavering trust in God's Messiah.",
                    HeaderLead: "Teshuvah: Returning to the Ancient Paths",
                    FullExposition: "Biblical repentance (Teshuvah) is not mere emotional remorse; it is a full 180-degree return of heart, mind, and behavior to God's commandments. Joined with active faith in Messiah's atoning blood, repentance produces genuine fruit worthy of the Kingdom.",
                    TargetStudyKey: "12foundations-repentance",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Acts 3:19", "Repent ye therefore, and be converted, that your sins may be blotted out, when the times of refreshing shall come."),
                        new ScriptureAnchor("Hebrews 6:1", "Leaving the principles of the doctrine of Christ... the foundation of repentance from dead works, and of faith toward God.")
                    }
                ),
                new FunnelItem(
                    Order: 11,
                    Badge: "Foundation 11",
                    Title: "The Role of the Ruach / Holy Spirit",
                    Slug: "role-of-the-spirit",
                    Subtitle: "Block IV: The Living Assembly",
                    ShortSummary: "The infilling, baptism, and supernatural power of the Holy Spirit to walk in Torah obedience and witness.",
                    HeaderLead: "Supernatural Power for Holiness and Ministry",
                    FullExposition: "The primary purpose of the Spirit as promised in Ezekiel 36:27 is to cause us to walk in God's statutes. In the New Testament, the baptism of the Holy Spirit empowers believers with spiritual gifts, bold witness, and divine authority to overcome the enemy.",
                    TargetStudyKey: "12foundations-ruach",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Ezekiel 36:27", "And I will put my spirit within you, and cause you to walk in my statutes, and ye shall keep my judgments, and do them."),
                        new ScriptureAnchor("Acts 1:8", "But ye shall receive power, after that the Holy Ghost is come upon you: and ye shall be witnesses unto me.")
                    }
                ),
                new FunnelItem(
                    Order: 12,
                    Badge: "Foundation 12",
                    Title: "Apostolic Commission to the Nations",
                    Slug: "apostolic-commission",
                    Subtitle: "Block IV: The Living Assembly",
                    ShortSummary: "Discipling the nations, establishing kingdom colonies, and preparing the Bride for the return of the King.",
                    HeaderLead: "The Mandate to Preach the Kingdom to the Ends of the Earth",
                    FullExposition: "The Great Commission commissions every local assembly to disciple nations, immerse converts into the covenant Name of Jesus, and teach obedience to all that Messiah commanded. We are ambassadors commissioned to establish outposts of the Kingdom across the globe.",
                    TargetStudyKey: "12foundations-commission",
                    Scriptures: new List<ScriptureAnchor>
                    {
                        new ScriptureAnchor("Matthew 28:19-20", "Go ye therefore, and teach all nations, baptizing them... Teaching them to observe all things whatsoever I have commanded you."),
                        new ScriptureAnchor("Acts 1:8", "And ye shall be witnesses unto me both in Jerusalem, and in all Judaea, and in Samaria, and unto the uttermost part of the earth.")
                    }
                )
            },
            Faqs: new List<FunnelFaq>
            {
                new FunnelFaq(
                    Question: "What are the 12 Foundations?",
                    Answer: "The 12 Foundations are a structured hermeneutical and doctrinal curriculum divided into 4 key blocks: The Text & Translation (Foundations 1-3), The Covenant Framework (Foundations 4-6), The Prophetic Rhythms (Foundations 7-9), and The Living Assembly (Foundations 10-12)."
                ),
                new FunnelFaq(
                    Question: "Who is this curriculum designed for?",
                    Answer: "Whether you are a new believer seeking grounded scripture knowledge or a seasoned leader looking for deep Hebrew and covenant context, the 12 Foundations provide systematic clarity without contradictions."
                ),
                new FunnelFaq(
                    Question: "How do I take this course or obtain study notes?",
                    Answer: "You can study with a CACI small group online or locally. Click any 'Request Guided Study' button to get registered with study materials, video lessons, and interactive discussions."
                )
            },
            StudyRequestBaseUrl: isSubsite ? "https://caci.life/request-bible-study" : "/request-bible-study",
            Keywords: "12 Foundations, Understanding Your Bible, Biblical Hermeneutics, Scripture Preservation, Divine Name YHWH, Covenant Theology, Commonwealth of Israel, Sabbath Rest, Moedim Feasts, Millennial Kingdom, Repentance, Ruach Holy Spirit, Great Commission, CACI"
        );
    }
}
