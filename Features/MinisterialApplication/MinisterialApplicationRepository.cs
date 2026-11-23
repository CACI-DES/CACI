#nullable enable
using System;
using System.Data;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CACI_Life.Core.Domain.Congregations;
using Dapper;
using Microsoft.Extensions.Logging;

namespace CACI_Life.Features.MinisterialApplication;

public sealed class MinisterialApplicationRepository(
    IDbConnectionFactory connectionFactory,
    ILogger<MinisterialApplicationRepository> logger) : IMinisterialApplicationRepository
{
    public async Task<MinisterialApplicationRecord?> GetByDraftKeyAsync(string draftKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(draftKey)) return null;

        try
        {
            await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            const string sql = @"
                SELECT TOP 1 *
                FROM CACI.MinisterialApplications
                WHERE DraftKey = @DraftKey;";
            return await conn.QueryFirstOrDefaultAsync<MinisterialApplicationRecord>(
                new CommandDefinition(sql, new { DraftKey = draftKey.Trim().ToUpperInvariant() }, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to fetch ministerial application by DraftKey '{DraftKey}'", draftKey);
            return null;
        }
    }

    public async Task<MinisterialApplicationRecord?> GetLatestDraftByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        try
        {
            await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            const string sql = @"
                SELECT TOP 1 *
                FROM CACI.MinisterialApplications
                WHERE Email = @Email AND IsDraft = 1
                ORDER BY UpdatedAtUtc DESC;";
            return await conn.QueryFirstOrDefaultAsync<MinisterialApplicationRecord>(
                new CommandDefinition(sql, new { Email = email.Trim() }, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to fetch latest draft ministerial application by email '{Email}'", email);
            return null;
        }
    }

    public async Task<MinisterialApplicationRecord> SaveDraftAsync(MinisterialApplicationRecord record, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(record.DraftKey))
        {
            record.DraftKey = GenerateDraftKey();
        }

        try
        {
            await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

            const string upsertSql = @"
                MERGE INTO CACI.MinisterialApplications AS target
                USING (SELECT @DraftKey AS DraftKey) AS source
                ON target.DraftKey = source.DraftKey
                WHEN MATCHED THEN
                    UPDATE SET
                        ApplicantName = @ApplicantName,
                        Email = @Email,
                        Phone = @Phone,
                        DateOfBirth = @DateOfBirth,
                        StreetAddress = @StreetAddress,
                        CityStateZip = @CityStateZip,
                        CredentialLevel = @CredentialLevel,
                        PresentStatus = @PresentStatus,
                        IsDraft = @IsDraft,
                        CompletionPercentage = @CompletionPercentage,
                        CurrentStep = @CurrentStep,
                        ApplicationDataJson = @ApplicationDataJson,
                        Status = @Status,
                        UpdatedAtUtc = SYSUTCDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (DraftKey, ApplicantName, Email, Phone, DateOfBirth, StreetAddress, CityStateZip, 
                            CredentialLevel, PresentStatus, IsDraft, CompletionPercentage, CurrentStep, 
                            ApplicationDataJson, Status, CreatedAtUtc, UpdatedAtUtc)
                    VALUES (@DraftKey, @ApplicantName, @Email, @Phone, @DateOfBirth, @StreetAddress, @CityStateZip, 
                            @CredentialLevel, @PresentStatus, @IsDraft, @CompletionPercentage, @CurrentStep, 
                            @ApplicationDataJson, @Status, SYSUTCDATETIME(), SYSUTCDATETIME())
                OUTPUT inserted.Id, inserted.CreatedAtUtc, inserted.UpdatedAtUtc;";

            var result = await conn.QuerySingleAsync<(int Id, DateTime CreatedAtUtc, DateTime UpdatedAtUtc)>(
                new CommandDefinition(upsertSql, record, cancellationToken: cancellationToken));

            record.Id = result.Id;
            record.CreatedAtUtc = result.CreatedAtUtc;
            record.UpdatedAtUtc = result.UpdatedAtUtc;

            return record;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to upsert draft ministerial application '{DraftKey}'", record.DraftKey);
            throw;
        }
    }

    public async Task SubmitApplicationAsync(int id, MinisterialApplicationData data, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var conn = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            const string sql = @"
                UPDATE CACI.MinisterialApplications
                SET
                    ApplicantName = @ApplicantName,
                    Email = @Email,
                    Phone = @Phone,
                    StreetAddress = @StreetAddress,
                    CityStateZip = @CityStateZip,
                    CredentialLevel = @CredentialLevel,
                    PresentStatus = @PresentStatus,
                    IsDraft = 0,
                    CompletionPercentage = 100,
                    ApplicationDataJson = @ApplicationDataJson,
                    Status = 'Submitted',
                    SubmittedAtUtc = SYSUTCDATETIME(),
                    UpdatedAtUtc = SYSUTCDATETIME()
                WHERE Id = @Id;";

            var json = JsonSerializer.Serialize(data);
            await conn.ExecuteAsync(new CommandDefinition(sql, new
            {
                Id = id,
                ApplicantName = data.FullName,
                Email = data.Email,
                Phone = data.Phone,
                StreetAddress = data.StreetAddress,
                CityStateZip = data.CityStateZip,
                CredentialLevel = data.CredentialLevelApplyingFor,
                PresentStatus = data.PresentMinisterialStatus,
                ApplicationDataJson = json
            }, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to submit ministerial application ID '{Id}'", id);
            throw;
        }
    }

    private static string GenerateDraftKey()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        var buffer = new char[8];
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = chars[random.Next(chars.Length)];
        }
        return new string(buffer);
    }
}
