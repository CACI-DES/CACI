using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CACI_Life.Components;
using CACI_Life.Features.Identity.Services;
using CACI_Life.Features.Identity.Models;
using CACI_Life.Features.Communication.Services;
using CACI_Life.Features.Giving.Services;
using CACI_Life.Features.Volunteers;
using CACI_Life.Features.Discovery;
using CACI_Life.Features.Events;
using CACI_Life.Features.Email;
using CACI_Life.Features.SEO.Services;
using CACI_Life.Features.SiteMedia;
using CACI_Life.Features.MinisterialApplication;
using CACI.Core.Domain.Funnels;
using CACI_Life.Core.Domain.Congregations;
using CACI_Life.Features.Congregations.Commands;
using CACI_Life.Features.Congregations.Queries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Cascading authentication state allows us to use AuthorizeView and check ClaimsPrincipal
builder.Services.AddCascadingAuthenticationState();

// Shared Data Protection for Single Sign-On across caci.life, study.caci.life, and admin.caci.life
var keysPath = ResolveDataProtectionKeysPath(builder);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("CACI_Shared_Ecosystem");

// Add Cookie authentication (CACI_Shared_Auth) + optional Google OAuth 2.0
var authBuilder = builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.Name = "CACI_Shared_Auth";
        options.CookieManager = new CaciLifeDynamicCookieManager(new ChunkingCookieManager());
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    })
    .AddCookie("GoogleExternal");

var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (!string.IsNullOrWhiteSpace(googleClientId) && !string.IsNullOrWhiteSpace(googleClientSecret))
{
    authBuilder.AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SignInScheme = "GoogleExternal";
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });
}

// Password Hasher for internal Identity validation
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();

// Email Settings configuration
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Vertical Slice Architecture Services (Internal, Dapper + Stored Procedures)
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IVolunteerSurveyRepository, VolunteerSurveyRepository>();
builder.Services.AddScoped<ISpiritualGiftsRepository, SpiritualGiftsRepository>();
builder.Services.AddScoped<IEventsRepository, EventsRepository>();
builder.Services.AddScoped<IGivingRepository, GivingRepository>();
builder.Services.AddScoped<IGivingStripeService, GivingStripeService>();
builder.Services.AddScoped<GivingReportService>();
builder.Services.AddScoped<ISeoRepository, SeoRepository>();
builder.Services.AddScoped<ISiteMediaService, SiteMediaService>();
builder.Services.AddScoped<ISubsiteContext, SubsiteContextService>();
builder.Services.AddScoped<IMinisterialApplicationRepository, MinisterialApplicationRepository>();
builder.Services.AddScoped<MinisterialApplicationService>();

// CACI Multi-Site Congregation Services & Queries
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddSingleton<IMarkdownService, MarkdownService>();
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<ICongregationService, CongregationService>();
builder.Services.AddScoped<GetHomeDataQuery>();
builder.Services.AddScoped<GetFoundationsQueries>();
builder.Services.AddScoped<GetStudiesQueries>();
builder.Services.AddScoped<GetFeastsQuery>();
builder.Services.AddScoped<SubmitInquiryCommand>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

// Crawler HEAD request handling
app.Use(async (context, next) =>
{
    if (HttpMethods.IsHead(context.Request.Method))
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        return;
    }
    await next();
});

// Short church slug 301 redirect: /clcmobile -> /congregation/clcmobile
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.Trim('/');
    if (!string.IsNullOrEmpty(path) && !path.Contains('/'))
    {
        var reserved = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "login", "logout", "register", "contact", "give", "giving", "events", 
            "sabbath", "ministry", "pillars", "foundations", "4keys", "4keysinfo", 
            "4pillarsinfo", "12foundations", "study-with-us", "request-bible-study",
            "feast-dates", "feastevent", "meet-the-caci", "what-we-believe", "our-history",
            "discovery", "churches", "congregations", "sitemap.xml", "robots.txt", "not-found", "error"
        };

        if (!reserved.Contains(path))
        {
            var config = context.RequestServices.GetRequiredService<IConfiguration>();
            var connStr = config.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(connStr))
            {
                try
                {
                    using var conn = new Microsoft.Data.SqlClient.SqlConnection(connStr);
                    var matchedSlug = await Dapper.SqlMapper.ExecuteScalarAsync<string>(
                        conn,
                        "SELECT TOP 1 Slug FROM CACI.Churches WHERE IsActive = 1 AND Slug = @slug",
                        new { slug = path });

                    if (!string.IsNullOrWhiteSpace(matchedSlug))
                    {
                        context.Response.Redirect($"/congregation/{matchedSlug}", permanent: true);
                        return;
                    }
                }
                catch
                {
                    // Fall through to regular pipeline
                }
            }
        }
    }
    await next();
});

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Local Authentication POST Handlers for Cookie Signing (Internal DB Validation)
app.MapPost("/login-handler", async (
    [FromForm] string email, 
    [FromForm] string password, 
    [FromQuery] string? returnUrl,
    IAuthRepository authRepo, 
    HttpContext context) =>
{
    if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
    {
        return Results.Redirect($"/login?error=Email and password are required&returnUrl={Uri.EscapeDataString(returnUrl ?? "")}");
    }

    try
    {
        var authResult = await authRepo.ValidateUserAsync(email, password);

        if (authResult.Succeeded)
        {
            var principal = BuildAuthPrincipal(authResult, email);
            await context.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            return Results.Redirect(NormalizeLocalReturnUrl(returnUrl));
        }
        else
        {
            var errorMsg = authResult.ErrorMessage ?? "Invalid email or password.";
            return Results.Redirect($"/login?error={Uri.EscapeDataString(errorMsg)}&returnUrl={Uri.EscapeDataString(returnUrl ?? "")}");
        }
    }
    catch (Exception ex)
    {
        return Results.Redirect($"/login?error={Uri.EscapeDataString(ex.Message)}&returnUrl={Uri.EscapeDataString(returnUrl ?? "")}");
    }
}).DisableAntiforgery();

app.MapGet("/api/auth/google", (HttpContext context, IConfiguration configuration) =>
{
    var clientId = configuration["Authentication:Google:ClientId"];
    var clientSecret = configuration["Authentication:Google:ClientSecret"];
    var returnUrl = NormalizeLocalReturnUrl(context.Request.Query["returnUrl"].ToString());

    if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
    {
        return Results.Redirect($"/login?error={Uri.EscapeDataString("Google sign-in is not yet configured.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var props = new AuthenticationProperties
    {
        RedirectUri = $"/api/auth/google-callback?returnUrl={Uri.EscapeDataString(returnUrl)}"
    };
    return Results.Challenge(props, [GoogleDefaults.AuthenticationScheme]);
}).AllowAnonymous();

app.MapGet("/api/auth/google-callback", async (
    HttpContext context,
    IAuthRepository authRepo) =>
{
    var returnUrl = NormalizeLocalReturnUrl(context.Request.Query["returnUrl"].ToString());
    var externalAuth = await context.AuthenticateAsync("GoogleExternal");
    if (!externalAuth.Succeeded || externalAuth.Principal is null)
    {
        return Results.Redirect($"/login?error={Uri.EscapeDataString("Google authentication failed. Please try again.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var email = externalAuth.Principal.FindFirstValue(ClaimTypes.Email);
    await context.SignOutAsync("GoogleExternal");

    if (string.IsNullOrWhiteSpace(email))
    {
        return Results.Redirect($"/login?error={Uri.EscapeDataString("Your Google account did not provide an email address.")}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var authResult = await authRepo.ValidateExternalUserByEmailAsync(email);
    if (!authResult.Succeeded)
    {
        var err = authResult.ErrorMessage ?? "No approved CACI account matches that Google email.";
        return Results.Redirect($"/login?error={Uri.EscapeDataString(err)}&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }

    var principal = BuildAuthPrincipal(authResult, email);
    await context.SignInAsync(IdentityConstants.ApplicationScheme, principal);
    return Results.Redirect(returnUrl);
}).AllowAnonymous();

static ClaimsPrincipal BuildAuthPrincipal(AuthResponse authResult, string fallbackEmail)
{
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, authResult.Email ?? fallbackEmail),
        new Claim(ClaimTypes.NameIdentifier, authResult.UserId ?? Guid.NewGuid().ToString())
    };

    foreach (var role in authResult.Roles)
    {
        claims.Add(new Claim(ClaimTypes.Role, role));
    }

    if (authResult.ChurchId.HasValue)
    {
        claims.Add(new Claim("church_id", authResult.ChurchId.Value.ToString()));
    }

    var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
    return new ClaimsPrincipal(identity);
}

static string NormalizeLocalReturnUrl(string? returnUrl)
{
    if (string.IsNullOrWhiteSpace(returnUrl))
    {
        return "/";
    }

    return Uri.IsWellFormedUriString(returnUrl, UriKind.Relative)
           && returnUrl.StartsWith('/')
           && !returnUrl.StartsWith("//")
        ? returnUrl
        : "/";
}

app.MapPost("/Account/Logout", async (HttpContext context) =>
{
    await context.SignOutAsync(IdentityConstants.ApplicationScheme);
    return Results.Redirect("/");
});

// Dynamic SEO Sitemap Endpoint (core caci.life routes + active congregations)
app.MapGet("/sitemap.xml", async (IConfiguration configuration) =>
{
    var coreRoutes = new (string Loc, string ChangeFreq, string Priority)[]
    {
        ("https://caci.life/", "weekly", "1.0"),
        ("https://caci.life/meet-the-caci", "monthly", "0.8"),
        ("https://caci.life/what-we-believe", "monthly", "0.8"),
        ("https://caci.life/our-history", "monthly", "0.7"),
        ("https://caci.life/ministry", "monthly", "0.8"),
        ("https://caci.life/4keys", "weekly", "0.9"),
        ("https://caci.life/4KeysInfo", "weekly", "0.8"),
        ("https://caci.life/4PillarsInfo", "weekly", "0.8"),
        ("https://caci.life/pillars", "weekly", "0.8"),
        ("https://caci.life/12Foundations", "weekly", "0.8"),
        ("https://caci.life/foundations", "weekly", "0.8"),
        ("https://caci.life/study-with-us", "weekly", "0.8"),
        ("https://caci.life/request-bible-study", "weekly", "0.9"),
        ("https://caci.life/sabbath", "weekly", "0.8"),
        ("https://caci.life/feast-dates", "monthly", "0.7"),
        ("https://caci.life/events", "weekly", "0.8"),
        ("https://caci.life/contact", "monthly", "0.7")
    };

    var activeSlugs = new List<string>();
    var connString = configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(connString))
    {
        try
        {
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(connString);
            var slugs = await Dapper.SqlMapper.QueryAsync<string>(
                conn,
                "SELECT Slug FROM CACI.Churches WHERE IsActive = 1 ORDER BY Slug ASC");
            activeSlugs = slugs.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        }
        catch
        {
            // Fallback to core routes if table is not yet initialized
        }
    }

    var sb = new System.Text.StringBuilder();
    sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
    sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");
    foreach (var route in coreRoutes)
    {
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape(route.Loc)}</loc>");
        sb.AppendLine($"    <changefreq>{route.ChangeFreq}</changefreq>");
        sb.AppendLine($"    <priority>{route.Priority}</priority>");
        sb.AppendLine("  </url>");
    }

    foreach (var rawSlug in activeSlugs)
    {
        var slug = rawSlug.Trim().Trim('/');
        // Canonical congregation subfolder routes (https://caci.life/congregation/{slug})
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape($"https://caci.life/congregation/{slug}")}</loc>");
        sb.AppendLine("    <changefreq>weekly</changefreq>");
        sb.AppendLine("    <priority>0.9</priority>");
        sb.AppendLine("  </url>");
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape($"https://caci.life/congregation/{slug}/what-we-believe")}</loc>");
        sb.AppendLine("    <changefreq>monthly</changefreq>");
        sb.AppendLine("    <priority>0.8</priority>");
        sb.AppendLine("  </url>");
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape($"https://caci.life/congregation/{slug}/sabbath")}</loc>");
        sb.AppendLine("    <changefreq>monthly</changefreq>");
        sb.AppendLine("    <priority>0.8</priority>");
        sb.AppendLine("  </url>");
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape($"https://caci.life/congregation/{slug}/bible-studies")}</loc>");
        sb.AppendLine("    <changefreq>weekly</changefreq>");
        sb.AppendLine("    <priority>0.8</priority>");
        sb.AppendLine("  </url>");
    }
    sb.AppendLine("</urlset>");

    return Results.Content(sb.ToString(), "application/xml; charset=utf-8");
}).AllowAnonymous();

// Explicit permissive robots.txt endpoint
app.MapGet("/robots.txt", () =>
    Results.Content("User-agent: *\nAllow: /\n\nSitemap: https://caci.life/sitemap.xml\n", "text/plain; charset=utf-8")
).AllowAnonymous();

app.Run();

static string ResolveDataProtectionKeysPath(WebApplicationBuilder builder)
{
    var keysPath = builder.Configuration["DataProtectionKeysPath"];
    if (string.IsNullOrWhiteSpace(keysPath))
    {
        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        if (string.IsNullOrEmpty(userProfile) ||
            userProfile.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.System), StringComparison.OrdinalIgnoreCase) ||
            userProfile.Contains("systemprofile", StringComparison.OrdinalIgnoreCase))
        {
            keysPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys");
        }
        else
        {
            try
            {
                keysPath = Path.Combine(userProfile, ".caci", "keys");
                if (!Directory.Exists(keysPath))
                    Directory.CreateDirectory(keysPath);
            }
            catch
            {
                keysPath = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys");
            }
        }
    }

    if (!Directory.Exists(keysPath))
    {
        try
        {
            Directory.CreateDirectory(keysPath);
        }
        catch
        {
            keysPath = Path.Combine(builder.Environment.ContentRootPath, "keys");
            if (!Directory.Exists(keysPath))
                Directory.CreateDirectory(keysPath);
        }
    }

    return keysPath;
}

public class CaciLifeDynamicCookieManager(ICookieManager concreteManager) : ICookieManager
{
    public string? GetRequestCookie(HttpContext context, string key)
        => concreteManager.GetRequestCookie(context, key);

    public void AppendResponseCookie(HttpContext context, string key, string? value, CookieOptions options)
    {
        var host = context.Request.Host.Host;
        if (IsCaciDomain(host))
        {
            options.Domain = ".caci.life";
        }
        concreteManager.AppendResponseCookie(context, key, value, options);
    }

    public void DeleteCookie(HttpContext context, string key, CookieOptions options)
    {
        var hostOptions = new CookieOptions
        {
            Path = options.Path,
            Secure = options.Secure,
            SameSite = options.SameSite,
            HttpOnly = options.HttpOnly
        };
        concreteManager.DeleteCookie(context, key, hostOptions);

        var host = context.Request.Host.Host;
        if (IsCaciDomain(host))
        {
            var domainOptions = new CookieOptions
            {
                Path = options.Path,
                Domain = ".caci.life",
                Secure = options.Secure,
                SameSite = options.SameSite,
                HttpOnly = options.HttpOnly
            };
            concreteManager.DeleteCookie(context, key, domainOptions);
        }
    }

    private static bool IsCaciDomain(string host)
        => host.EndsWith(".caci.life", StringComparison.OrdinalIgnoreCase)
           || host.Equals("caci.life", StringComparison.OrdinalIgnoreCase);
}
