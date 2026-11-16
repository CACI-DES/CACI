# CACI.Life â€” Strategic SEO & Market Share Growth Recommendations

**Document Version:** 1.0  
**Target Solution:** `c:\CACI_Sites\CACI_Life\CACI_Life.csproj` (`https://caci.life`)  
**Role in Network:** National & International Authority Hub / Episcopal Headquarters  
**Sister Assemblies:** [Kingdom Covenant Apostolic](https://kingdomcovenantapostolic.org) (Southwest Missouri / Greater Ozarks) & Affiliated Congregations  

---

## Executive Summary

This document outlines the architectural review and actionable roadmap to optimize **CACI.Life** for national search engine visibility (Google, Bing), rich snippet captures (Google Knowledge Panel, Courses, FAQs), and network syndication with regional daughter congregations.

The goal is to establish CACI.Life as the premier digital hub for Whole-Bible, First-Century Apostolic faith, driving organic search traffic and funneling seekers into regional fellowship and structured discipleship.

---

## 1. Architecture & Standards Compliance

| Category | Standard / Criteria | Current Implementation | Status |
| :--- | :--- | :--- | :---: |
| **Framework & Runtime** | .NET 10 / C# 14 | `net10.0` in `CACI_Life.csproj` with C# 14 syntax. | **100% Pass** |
| **Presentation Model** | Blazor Web App (SSR + Interactive Islands) | `AddInteractiveServerRenderMode()` with enhanced navigation (`blazor.web.js`). | **Pass** |
| **Data Access & Security** | Microsoft SQL Server with Dapper | Scoped repositories (`ISeoRepository`, `IAuthRepository`, `ICommunicationService`, etc.) using parameterized SQL. | **100% Pass** |
| **Architectural Pattern** | Vertical Slice Architecture | Organized into feature slices (`Features/Identity`, `Features/Communication`, `Features/Giving`, `Features/Events`, `Features/Theology`, `Features/SEO`). | **100% Pass** |
| **Dynamic SEO Engine** | Schema.org, OpenGraph, Canonical URLs | Implemented via `Components/Controls/SeoHead.razor` backed by `CACI.PageSeoSettings`. | **Pass** *(Optimizations Identified)* |
| **Crawl Architecture** | Valid `robots.txt` and `sitemap.xml` | Both exist in `wwwroot/`, but need updates for newly added discipleship courses. | **Pass** *(Needs Slugs Added)* |
| **Discipleship Pipeline** | Centralized 12 Foundations Engine | Open interactive courses at `/foundations/lesson/{1-3}` with referral tracking (`?ref=kca`) and elder routing. | **100% Pass** |

---

## 2. Priority SEO & Market Share Opportunities

### Opportunity 1: Upgrade Schema.org from `Organization` to `ReligiousOrganization` / `Church`
* **File:** `Components/Controls/SeoHead.razor`
* **Current Implementation:** Hardcodes `"@type": "Organization"`.
* **The Problem:** Generic `Organization` competes with commercial businesses and software companies rather than religious bodies and denominations.
* **The High-Impact Fix:** Upgrade to:
  ```json
  {
    "@context": "https://schema.org",
    "@type": ["ReligiousOrganization", "Church"],
    "name": "Covenant Apostolic Congregations International",
    "alternateName": "CACI",
    "url": "https://caci.life",
    "logo": "https://caci.life/images/logos/AllTheWord_NOTextLogo.png",
    "address": {
      "@type": "PostalAddress",
      "streetAddress": "9777 Betty's Way Suite C",
      "addressLocality": "Semmes",
      "addressRegion": "AL",
      "postalCode": "36575",
      "addressCountry": "US"
    },
    "sameAs": [
      "https://www.facebook.com/THECACI",
      "https://www.youtube.com/@THECACI",
      "https://www.youtube.com/@covenantapostoliccongregations"
    ]
  }
  ```
* **Result:** Qualifies CACI for an official **Google Knowledge Panel** in brand and doctrinal searches.

---

### Opportunity 2: Add `Course` & `LearningResource` Schema to `LessonReader.razor`
* **File:** `Features/Theology/LessonReader.razor` (`/foundations/lesson/{LessonNumber}`)
* **Current Implementation:** Has `<PageTitle>`, but does not invoke `<SeoHead>` and lacks structured course metadata.
* **The High-Impact Fix:**
  1. Add `<SeoHead>` to `LessonReader.razor` with unique titles and descriptions per lesson:
     - **Lesson 1:** "The Earth Created and Paradise Lost | 12 Foundations Lesson 1"
     - **Lesson 2:** "The Global Flood, The First Cleansing | 12 Foundations Lesson 2"
     - **Lesson 3:** "The Rise of Babylon, The Harlot System | 12 Foundations Lesson 3"
  2. Emit Google **`Course`** and **`LearningResource`** JSON-LD:
     ```json
     {
       "@context": "https://schema.org",
       "@type": "Course",
       "name": "12 Foundations: Lesson 1 â€“ The Blueprint & Paradise Lost",
       "description": "Understand your Bible from Genesis to Revelation without human creeds or seminaries.",
       "provider": {
         "@type": "ReligiousOrganization",
         "name": "Covenant Apostolic Congregations International",
         "sameAs": "https://caci.life"
       },
       "isAccessibleForFree": true,
       "educationalLevel": "Beginner to Intermediate Biblical Discipleship"
     }
     ```
* **Result:** Ranks inside Google's **Online Courses & Free Educational Resources** carousels.

---

### Opportunity 3: Update `sitemap.xml` with New Discipleship Landing Pages
* **File:** `wwwroot/sitemap.xml`
* **Current Implementation:** Does not include the new interactive lessons.
* **Action Items:**
  1. Add canonical URLs for each open foundation:
     - `https://caci.life/foundations/lesson/1` (priority: 0.9, weekly)
     - `https://caci.life/foundations/lesson/2` (priority: 0.8, weekly)
     - `https://caci.life/foundations/lesson/3` (priority: 0.8, weekly)
  2. Resolve subdomain canonical confusion:
     - Currently, `sitemap.xml` contains both `https://4keys.caci.life/` and `https://caci.life/4keys` / `https://caci.life/4KeysInfo`. Ensure all links point to the canonical URL structure to prevent keyword self-cannibalization.

---

### Opportunity 4: Embed FAQPage JSON-LD on Core Theological Pages
* **Files:** `Features/Theology/Sabbath.razor` & `Features/Theology/WhatWeBelieve.razor`
* **Current Implementation:** Deep theological texts are presented in tabs/accordions, but without structured Q&A schema.
* **The High-Impact Fix:** Provide JSON-LD for high-search topics:
  - "Why do Covenant Apostolics keep the 7th-day Sabbath?"
  - "Does Romans 14 or Colossians 2 abolish the Sabbath?"
  - "What is CACI's biblical view on Acts 2:38 baptism?"
  - "Why does CACI observe the biblical feast days in Leviticus 23?"
* **Result:** Triggers Google's "People Also Ask" SERP expandable answer cards for national queries.

---

### Opportunity 5: The Congregations & Assembly Directory (`/congregations`)
* **Proposed Route:** `/congregations` or `/fellowships`
* **Current Implementation:** There is currently no public directory of local assemblies on CACI.Life.
* **Strategic Value:**
  - Establishes a reciprocal backlink network between CACI.Life (national domain) and regional congregations (e.g. `kingdomcovenantapostolic.org`).
  - Converts national visitors into local in-person discipleship.
  - Featured listing:
    > **Kingdom Covenant Apostolic**  
    > *Serving Southwest Missouri & the Greater Ozarks (Springfield, Branson, Joplin)*  
    > Sabbath Convocations & 12 Foundations In-Person Discipleship  
    > Website: [kingdomcovenantapostolic.org](https://kingdomcovenantapostolic.org) | Pastoral Intake: [/connect](https://kingdomcovenantapostolic.org/connect)
* **Result:** Immediate surge in local search authority for daughter assemblies while cementing CACI as the mother organization.

---

## 3. Pre-Launch Implementation Checklist

When returning to CACI.Life development, execute these steps in order:

- [ ] **Step 1:** In `Components/Controls/SeoHead.razor`, update `"@type"` to `["ReligiousOrganization", "Church"]` and include the Semmes, AL headquarters address.
- [ ] **Step 2:** In `Features/Theology/LessonReader.razor`, add `<SeoHead>` and Course JSON-LD.
- [ ] **Step 3:** In `wwwroot/sitemap.xml`, add the 3 lesson URLs and verify canonical consistency.
- [ ] **Step 4:** In `Features/Theology/Sabbath.razor` and `WhatWeBelieve.razor`, wire `FAQPage` schema.
- [ ] **Step 5:** Create `Features/Identity/Congregations.razor` (`/congregations`) and link it in `NavBar.razor` and `Footer.razor`.
- [ ] **Step 6:** Run `dotnet build CACI_Life.csproj` to confirm zero compilation errors.