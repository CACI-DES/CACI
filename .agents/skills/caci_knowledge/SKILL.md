---
name: caci_knowledge
description: Architectural specification, schema configurations, cross-project synchronization rules, and integration details for the CACI ecosystem (CACI_Life, CACIEvents, CACI_Admin).
---

# CACI Ecosystem Architecture & Synchronization Guide

This skill serves as the central memory and specifications reference for Antigravity when working across the Covenant Apostolic Congregations International (CACI) online infrastructure.

## System Topology & Roles

> **Important Architecture Rule:** `CACI_API` is **deprecated and retired**. All data operations and processes are self-contained and execute internally within each respective application using Dapper and SQL Server.

1. **`CACI_Life` (Public Ministry & Multi-Site Microsites - `caci.life`)**
   - Blazor Server (`.AddInteractiveServerComponents()`) running ASP.NET Core 10.
   - Public-facing ministry portal and multi-tenant congregation site engine (`Features\Congregations\`).
   - Dynamic per-congregation branding: `LogoUrl`, `HeroTagline`, `Subtitle`, `RegionDescription`, `FacebookUrl`, `YouTubeUrl`, `ContactEmail`, `GoogleAnalyticsId`.
   - Data Access: Direct Dapper connections via `SqlConnectionFactory` with memory caching (`IMemoryCache`).

2. **`CACIEvents` (Event Registration & Ticketing - `cacievents.com`)**
   - Blazor Server project for conference registration, banquet tickets, meal plan selection (full week, half week), and lodging bookings.
   - Stripe payment processing and local webhook callback controller (`api/stripe/webhook`).
   - Automated attendee confirmation emails.

3. **`CACI_Admin` (Administrative Command Center - `admin.caci.life`)**
   - Operations dashboard for HQ organization administrators (`OrgAdmin`), district leaders, and pastors/church administrators (`ChurchAdmin`, `PastorAdmin`).
   - Modules:
     - **Congregations / Churches**: Org-level management (`ChurchesAdmin.razor`) and local pastor profile editor (`MyChurchAdmin.razor`) supporting all dynamic branding variables (Logo, Tagline, Subtitle, Region, Social, Analytics).
     - **Events & Lodging**: Registrations review, banquet balance, room assignments (`LodgingManagement.razor`), and QuestPDF badge generation (`Badges.razor`).
     - **Finances & Minister Dues**: Annual minister dues, Elect Lady dues, donation allocations, and accounting balance checks.
     - **District Governance**: Apostolic districts, super districts, and state assignments.
     - **SEO & Discovery**: Dynamic page SEO settings and Bible study / volunteer application reviews.

---

## Shared Database Specifications

- **Server**: `SQL8005.site4now.net`
- **Database**: `db_a92200_caci`
- **Key Schemas**:
  - `CACI`: `Churches`, `AspNetUsers`, `AspNetRoles`, `FellowshipInquiries`, `Ministry_Profiles`, `Ministry_Payments`, `PageSeoSettings`, `Districts`.
  - `EVENTS`: `Registrations`, `Attendees`, `PaymentTransactions`, `LodgingBookings`, `FoodPlans`.
  - `dbo`: `Online_Givings`, `Online_Giving_Accounts`, `Conferences`, `SiteContent`.

---

## Cross-Project Synchronization Protocol (Golden Rules)

Whenever a feature, field, or variable is introduced or changed in `CACI_Life` or `CACIEvents`:
1. **Database Migration Script**: Add an idempotent `.sql` script in `CACI_Admin\Database\Scripts\0XX_*.sql` and update `Master_Deployment_Script.sql`.
2. **Stored Procedures**: Update corresponding stored procedures (e.g., `CACI.usp_GetChurch`, `EVENTS.usp_GetRegistrationFinanceSummary`).
3. **CACI_Admin Models**: Update the corresponding DTO / Model in `CACI_Admin` (e.g. `ChurchModels.cs` or `RegistrationReportModel.cs`).
4. **CACI_Admin Repositories**: Update queries and parameters in the repository (e.g. `ChurchRepository.cs`, `EventsRepository.cs`).
5. **CACI_Admin UI**: Ensure both OrgAdmin and local ChurchAdmin views provide form controls to view and update the new variables.

---

## Shared Cookie SSO
- **Application Name**: `CACI_Shared_Ecosystem`
- **Cookie Name**: `CACI_Shared_Auth`
- **Cookie Domain**: `.caci.life`
- **Keys Path**: Shared directory under `%USERPROFILE%\.caci\keys` for decrypting credentials across subdomains.
