using System.Collections.Generic;

namespace CACI_Life.Features.Volunteers
{
    public class InterestGroup
    {
        public string Name { get; set; } = string.Empty;
        public List<string> Items { get; set; } = new();
    }

    public class SurveyDefinition
    {
        public string Key { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string IntroParagraph { get; set; } = string.Empty;
        public string ClosingText { get; set; } = string.Empty;
        
        public bool IsYouthMinistry => Key == "youth_ministry";
        public List<string> FeastOptions { get; set; } = new() { "EFC", "Tabernacles", "Both", "Any Feast/Event" };
        
        // Section 4 options
        public string GiftsLabel { get; set; } = "GIFTS, TALENTS & SKILLS";
        public string GiftsSubtext { get; set; } = "Please select the areas where you feel God has given you skills or experience that could serve in our team. (Check all that apply)";
        public List<string> GiftsOptions { get; set; } = new();

        // Section 5 labels
        public string GiftsExperienceLabel { get; set; } = string.Empty; // For youth ministry section 5
        public string ExperienceQuestion { get; set; } = "Do you have any experience in this area? If yes, please describe.";

        // Section 6 options
        public List<InterestGroup> InterestGroups { get; set; } = new();

        // Section 7 options
        public List<string> ComfortOptions { get; set; } = new();

        // Section 8 options
        public string HeartVisionQuestion1 { get; set; } = "Why do you feel called or interested in serving on this team?";
        public string HeartVisionQuestion2 { get; set; } = "How do you believe this team/ministry supports the mission of God's Kingdom?";

        public static SurveyDefinition Get(string key)
        {
            var normalizedKey = key?.ToLowerInvariant().Replace("-", "_").Replace(" ", "_") ?? string.Empty;
            return Definitions.TryGetValue(normalizedKey, out var def) ? def : throw new System.ArgumentException($"Survey type '{key}' not found.");
        }

        public static bool Exists(string key)
        {
            var normalizedKey = key?.ToLowerInvariant().Replace("-", "_").Replace(" ", "_") ?? string.Empty;
            return Definitions.ContainsKey(normalizedKey);
        }

        public static readonly Dictionary<string, SurveyDefinition> Definitions = new()
        {
            {
                "tech", new SurveyDefinition
                {
                    Key = "tech",
                    Title = "TECH TEAM",
                    IntroParagraph = "Thank you for your willingness to serve on the Tech Team! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and help build and maintain our online presence through excellent web tools and solutions.",
                    ClosingText = "Together we can build excellent tools that strengthen the ministry and reach the world for His Kingdom.",
                    GiftsOptions = new()
                    {
                        "Website Development (Front-End)", "Website Development (Back-End)", "Web Design / UI / UX",
                        "Content Management Systems (WordPress, etc.)", "Web Tools / Automation (Forms, Workflows, Integrations)",
                        "API / Integrations", "Database Management", "SEO / Analytics",
                        "Troubleshooting / Technical Support", "Writing / Content Creation",
                        "Graphic Design for Web", "Project Management", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience with website development or web tools? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "WEBSITE DEVELOPMENT", Items = new() { "Building New Websites", "Website Updates / Maintenance", "Plugin / Theme Management", "Performance / Security Optimization" } },
                        new InterestGroup { Name = "WEB TOOLS & SOLUTIONS", Items = new() { "Form Building (Surveys, Registrations, etc.)", "Email Marketing / Automation", "CRM / Member Management Systems", "Event Registration / Payment Systems", "Data Analytics / Reporting", "Workflow Automation / Integrations", "Third-Party Integrations (Social Media, Giving, etc.)", "Other" } }
                    },
                    ComfortOptions = new() { "Working independently", "Collaborating on projects", "Working behind the scenes", "Learning new technologies", "Solving technical problems" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Tech Team?",
                    HeartVisionQuestion2 = "How do you believe our website and web tools can help support the ministry and spread the Word?"
                }
            },
            {
                "marketing", new SurveyDefinition
                {
                    Key = "marketing",
                    Title = "MARKETING TEAM",
                    IntroParagraph = "Thank you for your willingness to serve! Please take a few minutes to complete this survey so we can connect you with marketing opportunities that help spread the word, engage our community, and support the success of our Feast events.",
                    ClosingText = "Together we can help share God's message, connect people to His feasts, and create meaningful experiences that glorify Him.",
                    FeastOptions = new() { "EFC", "Tabernacles", "Both", "Any Feast/Event" },
                    GiftsOptions = new()
                    {
                        "Graphic Design", "Photography", "Videography", "Social Media Management",
                        "Writing / Copywriting", "Public Relations", "Event Promotion", "Content Creation",
                        "Canva Experience", "Website Management", "Communication & Outreach",
                        "Creative Planning", "Team Leadership", "Other"
                    },
                    ExperienceQuestion = "Do you have experience in marketing, communications, social media, photography, design, or related areas? Please describe:",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "CONTENT CREATION", Items = new() { "Graphic Design", "Social Media Posts", "Video Creation & Editing", "Writing Captions & Announcements", "Testimony Stories" } },
                        new InterestGroup { Name = "PHOTOGRAPHY & MEDIA", Items = new() { "Event Photography", "Behind-the-Scenes Content", "Video Recording", "Livestream Support" } },
                        new InterestGroup { Name = "PROMOTION & OUTREACH", Items = new() { "Community Outreach", "Flyer Distribution", "Invitation Campaigns", "Email Communications", "Social Media Engagement" } },
                        new InterestGroup { Name = "EVENT MARKETING SUPPORT", Items = new() { "Booth Displays", "Signage & Banners", "Welcome Materials", "Marketing Table Support", "All of the Above" } }
                    },
                    ComfortOptions = new() { "Working behind the scenes", "Creative design work", "Taking photos/videos", "Writing content", "Communicating with people", "Leading a team", "Planning projects", "Learning new skills" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Marketing Team?",
                    HeartVisionQuestion2 = "How do you believe marketing and communication support the mission of God's Kingdom?"
                }
            },
            {
                "audio_visual", new SurveyDefinition
                {
                    Key = "audio_visual",
                    Title = "AUDIO/VISUAL TEAM",
                    IntroParagraph = "Thank you for your willingness to serve on the Audio/Visual Team! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and help create excellent sound and visual experiences for our church services at the feasts.",
                    ClosingText = "Together we can create powerful, distraction-free environments that glorify God and edify His people.",
                    GiftsSubtext = "Please select the areas where you feel God has given you skills or experience that could serve in our Audio/Visual Ministry. (Check all that apply)",
                    GiftsOptions = new()
                    {
                        "Audio: Mixing (Live Sound)", "Audio: Monitor / Stage Sound", "Audio: Microphone Setup & Placement",
                        "Audio: Audio Systems Setup", "Audio: Recording / Playback", "Audio: Troubleshooting / Technical Support",
                        "Video: Camera Operation", "Video: Video Switching / Directing", "Video: Projection / LED Screens",
                        "Video: Graphics / Presentations (ProPresenter, etc.)", "Video: Video Recording / Streaming",
                        "Video: Lighting Operation *(Basic stage lighting)", "Other: Equipment Setup / Tear Down",
                        "Other: Cable Management / Inventory", "Other: General Technical Support", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience setting up, running, or tearing down audio/visual equipment or programs for events or services? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "SERVICE ROLES", Items = new() { "Setup (Before Service/Events)", "Running / Operating (During Service/Events)", "Tear Down (After Service/Events)", "All of the Above" } },
                        new InterestGroup { Name = "AUDIO", Items = new() { "Front of House (Live Sound)", "Monitor / Stage Sound", "Audio Recording / Playback", "Audio Systems Maintenance" } },
                        new InterestGroup { Name = "VISUAL", Items = new() { "Camera Operation", "Video Switching / Directing", "Projection / Screens", "Graphics / Presentations", "Lighting Operation" } },
                        new InterestGroup { Name = "BROADCAST / STREAMING", Items = new() { "Live Streaming / Online Broadcast", "Recording / Editing", "Media Management" } },
                        new InterestGroup { Name = "OTHER", Items = new() { "Training / Teaching Others", "Inventory / Equipment Management", "Other" } }
                    },
                    ComfortOptions = new() { "Working behind the scenes", "Working on camera (visual)", "Setting up equipment", "Operating during service", "Solving technical problems", "Leading a team" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Audio/Visual Team?",
                    HeartVisionQuestion2 = "How do you believe excellent audio and visual helps people worship, learn, and connect with God's Word?"
                }
            },
            {
                "creative_arts", new SurveyDefinition
                {
                    Key = "creative_arts",
                    Title = "CREATIVE ARTS",
                    IntroParagraph = "Thank you for your willingness to serve in Creative Arts! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and help create beautiful, welcoming environments that honor God and enhance every ministry experience.",
                    ClosingText = "Together we can create beautiful spaces that glorify God and uplift His people.",
                    GiftsOptions = new()
                    {
                        "Decor Design / Styling", "Floral Arrangements / Greenery", "Stage Decor / Backdrops",
                        "Set Design / Fabrication", "Event Setup / Breakdown", "Table Design / Centerpieces",
                        "General Decorating / Detail Work", "Creative Planning / Vision Casting",
                        "Project Management / Coordination", "Organization / Logistics", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience in decor, event planning, or setup for events? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "STAGE / PLATFORM", Items = new() { "Stage Decor / Backdrops", "Platform Styling", "Prop Building / Set Construction", "Lighting Accents / Ambience" } },
                        new InterestGroup { Name = "EVENTS & GATHERINGS", Items = new() { "Ministers Luncheon", "Banquet", "Feast Services", "Special Events / Celebrations", "Retreats / Conferences", "Other Associated Events" } },
                        new InterestGroup { Name = "SUPPORT ROLES", Items = new() { "Setup / Installation", "Breakdown / Cleanup", "Inventory / Supplies", "Shopping / Vendor Coordination", "Transportation / Delivery", "Other" } }
                    },
                    ComfortOptions = new() { "Designing / Creating", "Building / Hands-On Work", "Setting up / Installing", "Organizing / Planning", "Working behind the scenes", "Leading a team" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving in Creative Arts?",
                    HeartVisionQuestion2 = "How do you believe beautiful, well-prepared environments enhance worship and ministry?"
                }
            },
            {
                "media_team", new SurveyDefinition
                {
                    Key = "media_team",
                    Title = "MEDIA TEAM",
                    IntroParagraph = "Thank you for your willingness to serve on the Media Team! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and help share the Word through excellence and creativity.",
                    ClosingText = "Together we can use media to proclaim the Truth and reach the world for His Kingdom.",
                    GiftsOptions = new()
                    {
                        "Camera Operation / Videography", "Photography", "Video Editing", "Live Streaming / Switching",
                        "Audio / Sound Engineering", "Graphic Design", "Social Media / Content Creation",
                        "Web / Technical Support", "Writing / Scriptwriting", "Lighting", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience serving on a media team or in related areas? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "VIDEO PRODUCTION", Items = new() { "Camera Operator", "Video Editor", "Director / Producer", "Live Streaming / Switcher", "Teleprompter Operator" } },
                        new InterestGroup { Name = "PHOTOGRAPHY", Items = new() { "Event Photography", "Portrait / Ministry Photography", "Photo Editing" } },
                        new InterestGroup { Name = "AUDIO", Items = new() { "Sound Mixing", "Microphone / Audio Setup", "Audio Recording" } },
                        new InterestGroup { Name = "DIGITAL & CREATIVE", Items = new() { "Graphic Design", "Social Media Management", "Content Creation", "Website / Technical Support", "Other" } }
                    },
                    ComfortOptions = new() { "Working behind the scenes", "Working on camera", "Both equally", "Assisting a leader", "Leading a team" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Media Team?",
                    HeartVisionQuestion2 = "How do you believe media can help share the Gospel and edify the body of Christ?"
                }
            },
            {
                "merchandise", new SurveyDefinition
                {
                    Key = "merchandise",
                    Title = "MERCHANDISE DEPARTMENT",
                    IntroParagraph = "Thank you for your willingness to serve in the Merchandise Department! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and serve the Kingdom through excellence and service.",
                    ClosingText = "Together we can make a lasting impact for the Kingdom.",
                    GiftsOptions = new()
                    {
                        "Customer Service / Hospitality", "Sales / Retail", "Inventory Management", "Receiving / Stocking",
                        "Merchandise Display / Visual Merchandising", "Cash Handling / POS Systems",
                        "Graphic Design / Creativity", "Photography / Content Creation",
                        "Online Store / E-commerce", "Organization / Administration", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience working in retail, inventory, or customer service? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "SALES & OPERATIONS", Items = new() { "Sales Associate / Customer Service", "Cashier / POS Operation", "Merchandise Display / Set-Up", "Inventory / Stock Management", "Receiving / Unpacking", "Shipping / Fulfillment" } },
                        new InterestGroup { Name = "ADMINISTRATIVE / BACK-END", Items = new() { "Inventory Tracking / Reporting", "Order Processing / Vendor Communication", "Pricing / Tagging", "Data Entry / Records" } },
                        new InterestGroup { Name = "CREATIVE / MARKETING", Items = new() { "Product Photography", "Social Media / Promotions", "Graphic Design", "Event Merchandise / Pop-Up Sales", "Other" } }
                    },
                    ComfortOptions = new() { "Serving customers", "Working behind the scenes", "Leading a team", "Assisting a leader" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving in the Merchandise Department?",
                    HeartVisionQuestion2 = "How do you believe merchandise can help support the ministry and spread the Word?"
                }
            },
            {
                "music_ministry", new SurveyDefinition
                {
                    Key = "music_ministry",
                    Title = "MUSIC MINISTRY",
                    IntroParagraph = "Thank you for your willingness to serve in Music Ministry! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and help lead God's people into worship and exaltation through music.",
                    ClosingText = "Together we can lead others into God's presence and make a lasting impact for His Kingdom.",
                    GiftsOptions = new()
                    {
                        "Vocal: Lead Vocals", "Vocal: Background Vocals / Vocal Harmony", "Vocal: Choir / Ensemble",
                        "Instrumental: Piano / Keyboard", "Instrumental: Electric Guitar", "Instrumental: Bass Guitar",
                        "Instrumental: Drums / Percussion", "Instrumental: Acoustic Guitar", "Instrumental: Violin / Strings",
                        "Instrumental: Brass (Trumpet, Trombone, etc.)", "Instrumental: Woodwinds (Saxophone, Flute, etc.)",
                        "Instrumental: Other Instrument",
                        "Other Music Roles: Worship Leader", "Other Music Roles: Music Arranging / Composing",
                        "Other Music Roles: Choir Director", "Other Music Roles: Vocal Coaching / Training",
                        "Other Music Roles: Audio / In-Ear Monitoring", "Other Music Roles: Music Librarian / Preparation",
                        "Other Music Roles: Stage Setup / Equipment Support", "Other Music Roles: Other"
                    },
                    ExperienceQuestion = "Do you have any experience serving in music ministry or in related areas? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "WORSHIP & LEADING", Items = new() { "Leading Congregational Worship", "Leading a Worship Team", "Special Music / Solo / Ensemble", "Choir / Ensemble Ministry" } },
                        new InterestGroup { Name = "MUSIC MINISTRY AREAS", Items = new() { "Feast / Service Music", "Choir Rehearsals", "Youth / Young Adult Music", "Children's Music Ministry", "Music Ministry Administration / Planning", "Retreats / Special Events", "Other" } }
                    },
                    ComfortOptions = new() { "Leading from the front", "Singing (lead or background)", "Playing an instrument", "Working behind the scenes", "Both front and behind", "Learning / Developing" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving in Music Ministry?",
                    HeartVisionQuestion2 = "What does worship mean to you, and how do you believe music can help draw people closer to God?"
                }
            },
            {
                "operations", new SurveyDefinition
                {
                    Key = "operations",
                    Title = "OPERATIONS TEAM",
                    IntroParagraph = "Thank you for your willingness to serve on the Operations Team! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and help ensure our events and services run smoothly, safely, and efficiently.",
                    ClosingText = "Together we can create smooth, safe, and well-prepared environments that glorify God and uplift His people.",
                    GiftsOptions = new()
                    {
                        "Set Up (Before Events / Services)", "Tear Down (After Events / Services)", "Moving / Transporting Elements",
                        "General Labor / Hands-On Work", "Equipment & Tool Handling", "Security / Crowd Management",
                        "Logistics & Coordination", "Problem Solving", "Leadership / Team Supervision",
                        "Forklift / Heavy Equipment Operation", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience in operations, event setup, security, or related areas? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "SET UP (BEFORE EVENTS / SERVICES)", Items = new() { "Stage / Platform Setup", "Seating / Tables / Chairs", "Decor / Backdrops / Signage", "Equipment / Supplies Preparation" } },
                        new InterestGroup { Name = "DURING EVENTS / SERVICES (OPERATIONS SUPPORT)", Items = new() { "General Support / Running Errands", "Moving / Adjusting Elements", "Monitoring / Safety Support", "Crowd Flow / Guest Assistance" } },
                        new InterestGroup { Name = "TEAR DOWN (AFTER EVENTS / SERVICES)", Items = new() { "Breakdown of Stage / Platform", "Packing / Loading Equipment", "Cleaning / Resetting Areas", "All of the Above" } },
                        new InterestGroup { Name = "SECURITY", Items = new() { "Access Control / Entry Points", "Patrol / Monitoring", "Crowd Safety / Emergency Response", "Parking / Traffic Direction", "Other" } }
                    },
                    ComfortOptions = new() { "Lifting / Physical Work", "Working behind the scenes", "Following instructions", "Leading a team", "Handling challenges / Problem solving" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Operations Team?",
                    HeartVisionQuestion2 = "How does helping with set up, tear down, and security support the mission of God's Kingdom?"
                }
            },
            {
                "youth_ministry", new SurveyDefinition
                {
                    Key = "youth_ministry",
                    Title = "YOUTH MINISTRY",
                    IntroParagraph = "Thank you for your willingness to serve in youth ministry! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts and serve the next generation.",
                    ClosingText = "Together we can make a lasting impact for the Kingdom.",
                    GiftsLabel = "AGE GROUP PREFERENCE",
                    GiftsSubtext = "Please select the age group you prefer to serve. (Check all that apply)",
                    GiftsOptions = new() { "Elementary (K-5th grade)", "Middle School (6th-8th grade)", "High School (9th-12th grade)", "No preference / open to where needed" },
                    GiftsExperienceLabel = "What strengths or talents do you feel God has given you that could serve youth?",
                    ExperienceQuestion = "Do you have any experience working with youth? Please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "TEACHING & DISCIPLESHIP", Items = new() { "Lead Teacher (lesson planning & teaching)", "Teaching Assistant (help support lesson & discussion)", "Small Group Leader (discussion, mentoring students)" } },
                        new InterestGroup { Name = "ACTIVITIES & ENGAGEMENT", Items = new() { "Games / Activity Leader", "Worship Team (music, singing, tech)", "Event Planning & Coordination" } },
                        new InterestGroup { Name = "SUPPORT ROLES", Items = new() { "Chaperone (events, trips, camps)", "Hospitality (snacks, welcoming students)", "Check-in / Registration", "Safety / Hall Monitor" } },
                        new InterestGroup { Name = "BEHIND-THE-SCENES", Items = new() { "Administrative / Organization", "Communication (emails, social media, reminders)", "Setup / Cleanup Team" } }
                    },
                    ComfortOptions = new() { "Leading a group", "Assisting a leader", "Behind the scenes" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving in youth ministry?",
                    HeartVisionQuestion2 = "What do you hope to see God do in the lives of students?"
                }
            },
            {
                "ladies_ministry", new SurveyDefinition
                {
                    Key = "ladies_ministry",
                    Title = "LADIES MINISTRY",
                    IntroParagraph = "Thank you for your willingness to serve in the Ladies Ministry! Please take a few minutes to complete this survey so we can connect you with the right opportunities to use your gifts to support, encourage, and uplift the women of our congregation.",
                    ClosingText = "Together we can build a strong sisterhood that honors God and strengthens our families and community.",
                    GiftsOptions = new()
                    {
                        "Event Planning & Coordinating", "Hospitality & Welcoming", "Small Group Leading / Discipleship",
                        "Public Speaking / Teaching", "Decorating & Creative Design", "Worship / Music",
                        "Intercessory Prayer", "Administrative / Organization", "Cooking / Baking / Food Prep",
                        "Mentoring / Counseling", "Outreach / Community Service", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience serving in women's ministry, event planning, or mentoring? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "EVENTS & GATHERINGS", Items = new() { "Ladies Luncheons / Teas", "Conferences / Retreats", "Fellowship Nights", "Event Setup & Decor" } },
                        new InterestGroup { Name = "DISCIPLESHIP & FELLOWSHIP", Items = new() { "Women's Small Groups / Bible Studies", "Mentoring Younger Women", "New Member Outreach", "Visitation & Care Support" } },
                        new InterestGroup { Name = "OPERATIONS & HELPING HANDS", Items = new() { "Hospitality & Welcoming", "Food Service / Catering", "Administrative Support", "Transportation Assistance", "Other" } }
                    },
                    ComfortOptions = new() { "Leading small groups", "Planning events", "Working behind the scenes", "One-on-one mentoring", "Welcoming guests", "Intercessory prayer" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving in the Ladies Ministry?",
                    HeartVisionQuestion2 = "How do you believe a strong women's ministry supports the overall health of the congregation?"
                }
            },
            {
                "mens_ministry", new SurveyDefinition
                {
                    Key = "mens_ministry",
                    Title = "MEN'S MINISTRY",
                    IntroParagraph = "Thank you for your willingness to serve in the Men's Ministry! Please take a few minutes to complete this survey so we can connect you with opportunities to help disciple, equip, and encourage the men of our congregation to lead and serve with integrity.",
                    ClosingText = "As iron sharpens iron, so one man sharpens another. Together we can stand strong in faith and service.",
                    GiftsOptions = new()
                    {
                        "Event Planning & Coordination", "Small Group Facilitation / Mentoring", "Public Speaking / Devotionals",
                        "Sound & Technical Support", "Set Up / Tear Down", "Outdoor / Sports Coordination",
                        "Intercessory Prayer", "Community Outreach / Service Projects", "Cooking / Grilling / Food Prep",
                        "Administration & Logistics", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience serving in men's ministry, mentoring, or coordinating service projects? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "FELLOWSHIP & EVENTS", Items = new() { "Men's Breakfasts", "Retreats & Conferences", "Sports / Outdoor Events", "Community Service Projects" } },
                        new InterestGroup { Name = "DISCIPLESHIP", Items = new() { "Men's Small Groups / Bible Studies", "Mentoring & Discipleship", "Outreach to New Members" } },
                        new InterestGroup { Name = "LOGISTICS & SUPPORT", Items = new() { "Event Setup & Tear Down", "Cooking & Grilling Team", "Security & Safety Support", "Transportation Support", "Other" } }
                    },
                    ComfortOptions = new() { "Leading small groups", "Coordinating events", "Hands-on labor / Setup", "Cooking / Food prep", "One-on-one mentoring", "Behind-the-scenes support" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving in the Men's Ministry?",
                    HeartVisionQuestion2 = "What is your vision for the men of CACI, and how can this ministry help achieve it?"
                }
            },
            {
                "prayer_ministry", new SurveyDefinition
                {
                    Key = "prayer_ministry",
                    Title = "PRAYER MINISTRY",
                    IntroParagraph = "Thank you for your heart and willingness to serve in the Prayer Ministry! Please take a few minutes to complete this survey so we can coordinate our intercessors to cover the church, its leaders, the feasts, and the specific needs of our congregation in prayer.",
                    ClosingText = "The effective, fervent prayer of a righteous man avails much. Together let us stand in the gap.",
                    GiftsOptions = new()
                    {
                        "Intercessory Prayer", "Altar Call Ministry", "Prophetic Prayer / Discernment",
                        "Deliverance Ministry", "Teaching on Prayer / Spiritual Warfare", "Encouragement & Pastoral Care",
                        "Coordination & Administration", "Writing Devotionals / Prayer Guides", "Other"
                    },
                    ExperienceQuestion = "Do you have experience serving on a prayer team, altar team, or in intercessory prayer groups? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "PRAYER ROLES", Items = new() { "Intercessory Prayer (Behind the scenes)", "Altar / Service Prayer Team", "Emergency Prayer Chain / Request Coverage", "Feast / Event Prayer Coverage" } },
                        new InterestGroup { Name = "SPECIALIZED MINISTRY", Items = new() { "Inner Healing / Deliverance Prayer", "Hospital / Home Visitations", "Prophetic / Discernment Ministry" } },
                        new InterestGroup { Name = "COORDINATION & DISSEMINATION", Items = new() { "Writing Prayer Guides / Points", "Organizing Prayer Meetings / Vigils", "Administrative Support", "Other" } }
                    },
                    ComfortOptions = new() { "Praying silently / alone", "Praying aloud in groups", "Praying one-on-one with individuals", "Altar ministry", "Teaching others how to pray", "Behind-the-scenes intercession" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Prayer Team?",
                    HeartVisionQuestion2 = "What does a lifestyle of prayer mean to you, and how do you see it impacting CACI?"
                }
            },
            {
                "sign_language", new SurveyDefinition
                {
                    Key = "sign_language",
                    Title = "SIGN LANGUAGE TEAM",
                    IntroParagraph = "Thank you for your willingness to serve on the Sign Language Team! Please take a few minutes to complete this survey so we can connect you with opportunities to use your signing gifts to interpret our worship services, sermons, and events, making the Word accessible to all.",
                    ClosingText = "Together we can ensure that every person hears and understands the message of the Kingdom.",
                    GiftsOptions = new()
                    {
                        "ASL Fluency (Conversational)", "ASL Fluency (Professional/Interpreter Level)", "Linguistic Interpretation (Transliteration)",
                        "Signing in Song / Artistic Expression", "Teaching ASL / Deaf Culture Awareness", "Deaf Outreach & Advocacy",
                        "Stage / Platform Presence", "Technical Support (Lighting, Stage Placement)", "Administrative Support", "Other"
                    },
                    ExperienceQuestion = "Do you have any experience interpreting or signing in ASL, or working within the Deaf community? If yes, please describe.",
                    InterestGroups = new()
                    {
                        new InterestGroup { Name = "INTERPRETATION ROLES", Items = new() { "Live Sermon Interpretation", "Live Praise & Worship Interpretation", "Special Events / Luncheons Interpretation", "Video Content / Pre-recorded Translation" } },
                        new InterestGroup { Name = "OUTREACH & EDUCATION", Items = new() { "Deaf Community Outreach", "Teaching ASL to Church Members", "Deaf Fellowship Events", "Creating Deaf-friendly Materials" } },
                        new InterestGroup { Name = "SUPPORT & STAGE", Items = new() { "Platform Coordination", "Lighting / Placement Optimization", "Administrative Assistance", "Other" } }
                    },
                    ComfortOptions = new() { "Signing on stage (front of sanctuary)", "Signing for smaller groups", "Translating pre-recorded videos", "Teaching/Instructing", "Working behind the scenes", "Learning/Developing signing skills" },
                    HeartVisionQuestion1 = "Why do you feel called or interested in serving on the Sign Language Team?",
                    HeartVisionQuestion2 = "How do you believe sign language and deaf inclusion support the mission of God's Kingdom at CACI?"
                }
            }
        };
    }
}
