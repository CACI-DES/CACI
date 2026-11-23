#nullable enable
using System;
using System.Collections.Generic;

namespace CACI_Life.Features.MinisterialApplication;

public class MinisterialApplicationData
{
    // ==========================================
    // STAGE 1: Personal & Contact Information
    // ==========================================
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string CityStateZip { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // ==========================================
    // STAGE 2: Credential Certificate Level
    // ==========================================
    public string PresentMinisterialStatus { get; set; } = string.Empty;
    public string CredentialLevelApplyingFor { get; set; } = string.Empty; // Bishop, Elder, Minister
    
    // Bishop-specific
    public string BishopOption { get; set; } = string.Empty;
    
    // Elder-specific
    public string ElderOption { get; set; } = string.Empty;
    public List<string> ElderQualifications { get; set; } = new();

    // Minister-specific
    public string MinisterOption { get; set; } = string.Empty;
    public string PresentMinistryDuration { get; set; } = string.Empty;
    public string PastorSupport { get; set; } = string.Empty;
    public string PastorName { get; set; } = string.Empty;

    // ==========================================
    // STAGE 3: Family Status
    // ==========================================
    public string MaritalStatus { get; set; } = string.Empty;
    public string AnniversaryDate { get; set; } = string.Empty;
    public string SpouseName { get; set; } = string.Empty;
    public string SpouseSupportive { get; set; } = string.Empty;

    // ==========================================
    // STAGE 4: Conversion Experience
    // ==========================================
    public string Acts238Standard { get; set; } = string.Empty;
    public string WaterBaptismJesusName { get; set; } = string.Empty;
    public string HolySpiritTonguesEvidence { get; set; } = string.Empty;

    // ==========================================
    // STAGE 5: Reputation & Background
    // ==========================================
    public string CriminalLitigation { get; set; } = string.Empty;
    public string SexualMisconduct { get; set; } = string.Empty;
    public string PayDebtsTimely { get; set; } = string.Empty;
    public string CountCostBeforeDebt { get; set; } = string.Empty;
    public string RefrainProfanitySocialMedia { get; set; } = string.Empty;
    public string SecretSocietiesRacistGroups { get; set; } = string.Empty;
    public string ConsentBackgroundCheck { get; set; } = string.Empty;

    // ==========================================
    // STAGE 6: The CACI Message - The 4 Keys of The Kingdom
    // ==========================================
    public int Key1Rating { get; set; } = 5;
    public string Key1Understanding { get; set; } = string.Empty;

    public int Key2Rating { get; set; } = 5;
    public string Key2Understanding { get; set; } = string.Empty;
    public string Observe7thDaySabbath { get; set; } = string.Empty;
    public string AttendWeeklySundayService { get; set; } = string.Empty;

    public int Key3Rating { get; set; } = 5;
    public string Key3Understanding { get; set; } = string.Empty;
    public string TorahSpiritualApplication { get; set; } = string.Empty;

    public int Key4Rating { get; set; } = 5;
    public string Key4Understanding { get; set; } = string.Empty;
    public string FullConversionStandard { get; set; } = string.Empty;
    public string ViewOfTheName { get; set; } = string.Empty;

    // ==========================================
    // STAGE 7: The CACI Mission - The 4 Pillars of The Apostolic Faith
    // ==========================================
    public int Pillar1Rating { get; set; } = 5;
    public string Pillar1Understanding { get; set; } = string.Empty;

    public int Pillar2Rating { get; set; } = 5;
    public string Pillar2Understanding { get; set; } = string.Empty;

    public int Pillar3Rating { get; set; } = 5;
    public string Pillar3Understanding { get; set; } = string.Empty;

    public int Pillar4Rating { get; set; } = 5;
    public string Pillar4Understanding { get; set; } = string.Empty;

    // ==========================================
    // STAGE 8: Your Covenant & Commitments
    // ==========================================
    public string FulfillObligations { get; set; } = string.Empty;
    public string DisputeResolutionBiblicalNoLawsuit { get; set; } = string.Empty;
    public string SurrenderCredentialsIfRevoked { get; set; } = string.Empty;
    public string AdhereCodeOfEthics { get; set; } = string.Empty;
    public string BiblicalSubmissionLeadership { get; set; } = string.Empty;
    public string UnityNonFundamentals { get; set; } = string.Empty;
    public string UnityRestoredMinisters { get; set; } = string.Empty;
    public string AnnualConvocationParticipation { get; set; } = string.Empty;
    public string StudyCaciTeachings { get; set; } = string.Empty;
    public string CounselBeforePublicDifferenceOrResignation { get; set; } = string.Empty;
    public string CounselBeforeAdoptingOutsideDoctrine { get; set; } = string.Empty;

    // ==========================================
    // STAGE 9: Additional Considerations
    // ==========================================
    public string WhyAffiliationWithCaci { get; set; } = string.Empty;
    public string AdditionalInformation { get; set; } = string.Empty;
}

public class MinisterialApplicationRecord
{
    public int Id { get; set; }
    public string DraftKey { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? StreetAddress { get; set; }
    public string? CityStateZip { get; set; }
    public string? CredentialLevel { get; set; }
    public string? PresentStatus { get; set; }
    public bool IsDraft { get; set; } = true;
    public int CompletionPercentage { get; set; }
    public int CurrentStep { get; set; } = 1;
    public string ApplicationDataJson { get; set; } = "{}";
    public string Status { get; set; } = "Draft";
    public string? AdminNotes { get; set; }
    public string? ReviewedBy { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public MinisterialApplicationData GetData()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ApplicationDataJson)) return new MinisterialApplicationData();
            return System.Text.Json.JsonSerializer.Deserialize<MinisterialApplicationData>(ApplicationDataJson) 
                   ?? new MinisterialApplicationData();
        }
        catch
        {
            return new MinisterialApplicationData();
        }
    }
}
