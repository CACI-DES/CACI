#nullable enable
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CACI_Life.Features.Email;
using Microsoft.Extensions.Logging;

namespace CACI_Life.Features.MinisterialApplication;

public sealed class MinisterialApplicationService(
    IMinisterialApplicationRepository repository,
    IEmailService emailService,
    ILogger<MinisterialApplicationService> logger)
{
    public Task<MinisterialApplicationRecord?> GetByDraftKeyAsync(string draftKey, CancellationToken ct = default)
        => repository.GetByDraftKeyAsync(draftKey, ct);

    public Task<MinisterialApplicationRecord?> GetLatestDraftByEmailAsync(string email, CancellationToken ct = default)
        => repository.GetLatestDraftByEmailAsync(email, ct);

    public async Task<MinisterialApplicationRecord> SaveDraftAsync(
        MinisterialApplicationData data,
        string? draftKey,
        int currentStep,
        CancellationToken ct = default)
    {
        var completion = CalculateCompletion(data);

        var record = new MinisterialApplicationRecord
        {
            DraftKey = !string.IsNullOrWhiteSpace(draftKey) ? draftKey.Trim().ToUpperInvariant() : string.Empty,
            ApplicantName = !string.IsNullOrWhiteSpace(data.FullName) ? data.FullName : "Draft Applicant",
            Email = data.Email,
            Phone = data.Phone,
            StreetAddress = data.StreetAddress,
            CityStateZip = data.CityStateZip,
            CredentialLevel = data.CredentialLevelApplyingFor,
            PresentStatus = data.PresentMinisterialStatus,
            IsDraft = true,
            CompletionPercentage = completion.Percentage,
            CurrentStep = currentStep,
            ApplicationDataJson = JsonSerializer.Serialize(data),
            Status = "Draft"
        };

        if (DateTime.TryParse(data.DateOfBirth, out var dob))
        {
            record.DateOfBirth = dob;
        }

        return await repository.SaveDraftAsync(record, ct);
    }

    public async Task SendResumeLinkEmailAsync(string email, string draftKey, string resumeUrl)
    {
        if (string.IsNullOrWhiteSpace(email)) return;

        var subject = $"Your CACI Ministerial Application Draft Code [{draftKey}]";
        var body = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; color: #1e293b;'>
    <div style='text-align: center; margin-bottom: 25px;'>
        <h2 style='color: #174477; margin: 0;'>CACI Ministerial Application</h2>
        <p style='color: #64748b; font-size: 14px; margin-top: 5px;'>Save &amp; Resume Portal</p>
    </div>
    <div style='background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 20px; margin-bottom: 25px;'>
        <p style='margin: 0 0 12px 0; font-size: 15px;'>Peace and blessings,</p>
        <p style='margin: 0 0 16px 0; font-size: 14px; line-height: 1.5;'>
            You have saved an in-progress draft of your <strong>CACI Ministerial Ordination &amp; Credential Application</strong>. You may resume and complete your application at any time using your unique resume code:
        </p>
        <div style='text-align: center; margin: 25px 0;'>
            <span style='display: inline-block; font-size: 26px; font-weight: bold; letter-spacing: 4px; color: #174477; background: #e0f2fe; padding: 12px 28px; border-radius: 6px; border: 1px solid #bae6fd;'>
                {draftKey}
            </span>
        </div>
        <div style='text-align: center; margin-top: 20px;'>
            <a href='{resumeUrl}' style='display: inline-block; background-color: #174477; color: #ffffff; text-decoration: none; padding: 12px 24px; font-weight: bold; font-size: 14px; border-radius: 6px;'>
                Click Here to Resume Application
            </a>
        </div>
    </div>
    <p style='font-size: 12px; color: #94a3b8; text-align: center;'>
        Covenant Apostolic Congregations International &bull; All the Word to All the World
    </p>
</div>";

        try
        {
            await emailService.SendEmailAsync(email, subject, body);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send resume code email to '{Email}'", email);
        }
    }

    public async Task SubmitApplicationAsync(int recordId, MinisterialApplicationData data, CancellationToken ct = default)
    {
        await repository.SubmitApplicationAsync(recordId, data, ct);

        // 1. Send confirmation to applicant
        if (!string.IsNullOrWhiteSpace(data.Email))
        {
            var applicantSubject = "CACI Ministerial Credential Application Received";
            var applicantBody = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; color: #1e293b;'>
    <div style='text-align: center; margin-bottom: 25px;'>
        <h2 style='color: #174477; margin: 0;'>Application Received</h2>
        <p style='color: #64748b; font-size: 14px; margin-top: 5px;'>Covenant Apostolic Congregations International</p>
    </div>
    <div style='background-color: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 20px; margin-bottom: 20px;'>
        <p style='margin: 0 0 12px 0; font-size: 15px;'>Dear {data.FullName},</p>
        <p style='margin: 0 0 14px 0; font-size: 14px; line-height: 1.6;'>
            We have received your completed <strong>CACI Ministerial Ordination &amp; Credential Application</strong>.
        </p>
        <table style='width: 100%; border-collapse: collapse; font-size: 13px; margin: 15px 0;'>
            <tr><td style='padding: 6px 0; color: #64748b;'><strong>Applicant Name:</strong></td><td>{data.FullName}</td></tr>
            <tr><td style='padding: 6px 0; color: #64748b;'><strong>Target Office:</strong></td><td>{data.CredentialLevelApplyingFor}</td></tr>
            <tr><td style='padding: 6px 0; color: #64748b;'><strong>Present Status:</strong></td><td>{data.PresentMinisterialStatus}</td></tr>
            <tr><td style='padding: 6px 0; color: #64748b;'><strong>Submission Date:</strong></td><td>{DateTime.UtcNow:MMMM dd, yyyy} UTC</td></tr>
        </table>
        <p style='margin: 14px 0 0 0; font-size: 14px; line-height: 1.6;'>
            Your application has been delivered to the CACI Apostolic Council and Credential Committee for formal examination. You will be contacted regarding the next steps in your credential review process.
        </p>
    </div>
    <p style='font-size: 12px; color: #94a3b8; text-align: center;'>
        Covenant Apostolic Congregations International &bull; All the Word to All the World
    </p>
</div>";
            try
            {
                await emailService.SendEmailAsync(data.Email, applicantSubject, applicantBody);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send confirmation email to applicant '{Email}'", data.Email);
            }
        }

        // 2. Send notification to HQ
        var hqSubject = $"NEW Ministerial Application: {data.FullName} ({data.CredentialLevelApplyingFor})";
        var hqBody = $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <h2 style='color: #174477;'>New Ministerial Application Submitted</h2>
    <p>A new ministerial credential application has been submitted and is ready for review in CACI Admin.</p>
    <ul>
        <li><strong>Name:</strong> {data.FullName}</li>
        <li><strong>Email:</strong> {data.Email}</li>
        <li><strong>Phone:</strong> {data.Phone}</li>
        <li><strong>Location:</strong> {data.CityStateZip}</li>
        <li><strong>Office Applying For:</strong> {data.CredentialLevelApplyingFor}</li>
        <li><strong>Status:</strong> {data.PresentMinisterialStatus}</li>
        <li><strong>Pastor Name / Support:</strong> {data.PastorName} ({data.PastorSupport})</li>
    </ul>
    <p>Please log in to <a href='https://admin.cacievents.com/admin/applications'>CACI Admin Ministerial Applications</a> to review the full submission.</p>
</div>";
        try
        {
            await emailService.SendEmailAsync("Contact@caci.life", hqSubject, hqBody);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send application notification to HQ");
        }
    }

    public (int Percentage, List<string> MissingFields) CalculateCompletion(MinisterialApplicationData d)
    {
        var required = new List<(string Name, bool IsFilled)>
        {
            ("Full Name", !string.IsNullOrWhiteSpace(d.FullName)),
            ("Phone Number", !string.IsNullOrWhiteSpace(d.Phone)),
            ("Date of Birth", !string.IsNullOrWhiteSpace(d.DateOfBirth)),
            ("Street Address", !string.IsNullOrWhiteSpace(d.StreetAddress)),
            ("City / State / Zip", !string.IsNullOrWhiteSpace(d.CityStateZip)),
            ("Email Address", !string.IsNullOrWhiteSpace(d.Email)),
            ("Present Ministerial Status", !string.IsNullOrWhiteSpace(d.PresentMinisterialStatus)),
            ("Credential Certificate Level", !string.IsNullOrWhiteSpace(d.CredentialLevelApplyingFor)),
            ("Time in Present Ministry", !string.IsNullOrWhiteSpace(d.PresentMinistryDuration)),
            ("Pastor's Support", !string.IsNullOrWhiteSpace(d.PastorSupport)),
            ("Pastor's Name", !string.IsNullOrWhiteSpace(d.PastorName)),
            ("Marital Status", !string.IsNullOrWhiteSpace(d.MaritalStatus)),
            ("Acts 2:38 Conversion Standard", !string.IsNullOrWhiteSpace(d.Acts238Standard)),
            ("Water Baptism in Jesus Name", !string.IsNullOrWhiteSpace(d.WaterBaptismJesusName)),
            ("Holy Spirit Infilling with Tongues", !string.IsNullOrWhiteSpace(d.HolySpiritTonguesEvidence)),
            ("Criminal Litigation Declaration", !string.IsNullOrWhiteSpace(d.CriminalLitigation)),
            ("Criminal Sexual Misconduct Declaration", !string.IsNullOrWhiteSpace(d.SexualMisconduct)),
            ("Timely Debt Payment", !string.IsNullOrWhiteSpace(d.PayDebtsTimely)),
            ("Counting Cost of Debt", !string.IsNullOrWhiteSpace(d.CountCostBeforeDebt)),
            ("Refraining from Profanity", !string.IsNullOrWhiteSpace(d.RefrainProfanitySocialMedia)),
            ("Secret Societies & Racist Groups", !string.IsNullOrWhiteSpace(d.SecretSocietiesRacistGroups)),
            ("Consent to Background Check", !string.IsNullOrWhiteSpace(d.ConsentBackgroundCheck)),
            ("Key #1 Understanding", !string.IsNullOrWhiteSpace(d.Key1Understanding)),
            ("Key #2 Understanding", !string.IsNullOrWhiteSpace(d.Key2Understanding)),
            ("7th-Day Sabbath Observance", !string.IsNullOrWhiteSpace(d.Observe7thDaySabbath)),
            ("Key #3 Understanding", !string.IsNullOrWhiteSpace(d.Key3Understanding)),
            ("Spiritual Application of Torah", !string.IsNullOrWhiteSpace(d.TorahSpiritualApplication)),
            ("Key #4 Understanding", !string.IsNullOrWhiteSpace(d.Key4Understanding)),
            ("Full Conversion Teaching Standard", !string.IsNullOrWhiteSpace(d.FullConversionStandard)),
            ("View of The Name", !string.IsNullOrWhiteSpace(d.ViewOfTheName)),
            ("Pillar #1 Understanding", !string.IsNullOrWhiteSpace(d.Pillar1Understanding)),
            ("Pillar #2 Understanding", !string.IsNullOrWhiteSpace(d.Pillar2Understanding)),
            ("Pillar #3 Understanding", !string.IsNullOrWhiteSpace(d.Pillar3Understanding)),
            ("Pillar #4 Understanding", !string.IsNullOrWhiteSpace(d.Pillar4Understanding)),
            ("Fulfill Ministerial Obligations", !string.IsNullOrWhiteSpace(d.FulfillObligations)),
            ("Dispute Resolution Without Lawsuit", !string.IsNullOrWhiteSpace(d.DisputeResolutionBiblicalNoLawsuit)),
            ("Surrender Credentials If Revoked", !string.IsNullOrWhiteSpace(d.SurrenderCredentialsIfRevoked)),
            ("Adhere to Ministerial Code of Ethics", !string.IsNullOrWhiteSpace(d.AdhereCodeOfEthics)),
            ("Biblical Submission to Leadership", !string.IsNullOrWhiteSpace(d.BiblicalSubmissionLeadership)),
            ("Unity on Non-Fundamentals", !string.IsNullOrWhiteSpace(d.UnityNonFundamentals)),
            ("Unity with Restored Ministers", !string.IsNullOrWhiteSpace(d.UnityRestoredMinisters)),
            ("Annual Convocation Participation", !string.IsNullOrWhiteSpace(d.AnnualConvocationParticipation)),
            ("Commitment to Study CACI Teachings", !string.IsNullOrWhiteSpace(d.StudyCaciTeachings)),
            ("Counsel Prior to Public Difference/Resignation", !string.IsNullOrWhiteSpace(d.CounselBeforePublicDifferenceOrResignation)),
            ("Counsel Prior to Outside Doctrine", !string.IsNullOrWhiteSpace(d.CounselBeforeAdoptingOutsideDoctrine)),
            ("Why Affiliation with CACI", !string.IsNullOrWhiteSpace(d.WhyAffiliationWithCaci))
        };

        var missing = new List<string>();
        int filledCount = 0;

        foreach (var req in required)
        {
            if (req.IsFilled)
            {
                filledCount++;
            }
            else
            {
                missing.Add(req.Name);
            }
        }

        int percentage = (int)Math.Round((double)filledCount / required.Count * 100);
        return (percentage, missing);
    }
}
