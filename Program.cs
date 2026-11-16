using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
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

// Add Cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

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
builder.Services.AddScoped<ISubsiteContext, SubsiteContextService>();

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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, authResult.Email ?? email),
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

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
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

app.MapPost("/Account/Logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
        ("https://4keys.caci.life/", "weekly", "0.9"),
        ("https://caci.life/4KeysInfo", "weekly", "0.8"),
        ("https://caci.life/4keys", "weekly", "0.8"),
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
        // Consolidated subfolder route (https://caci.life/{Slug})
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape($"https://caci.life/{slug}")}</loc>");
        sb.AppendLine("    <changefreq>weekly</changefreq>");
        sb.AppendLine("    <priority>0.9</priority>");
        sb.AppendLine("  </url>");
        // Subdomain route (https://{Slug}.caci.life/)
        sb.AppendLine("  <url>");
        sb.AppendLine($"    <loc>{System.Security.SecurityElement.Escape($"https://{slug}.caci.life/")}</loc>");
        sb.AppendLine("    <changefreq>weekly</changefreq>");
        sb.AppendLine("    <priority>0.9</priority>");
        sb.AppendLine("  </url>");
    }
    sb.AppendLine("</urlset>");

    return Results.Content(sb.ToString(), "application/xml; charset=utf-8");
}).AllowAnonymous();

app.Run();
