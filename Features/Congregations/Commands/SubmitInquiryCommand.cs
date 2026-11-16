#nullable enable
using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using CACI_Life.Core.Domain.Congregations;

namespace CACI_Life.Features.Congregations.Commands;

public sealed class InquiryForm
{
    [Required(ErrorMessage = "Please enter your full name."), StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your email."), EmailAddress(ErrorMessage = "Please enter a valid email."), StringLength(250)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter a phone number."), StringLength(50)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your city."), StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter your state."), StringLength(50)]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please share a little of your spiritual background."), StringLength(4000)]
    public string SpiritualBackground { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string PreferredContact { get; set; } = "Phone";

    [StringLength(250)]
    public string? SourceScreen { get; set; }

    /// <summary>Honeypot. Real visitors never see or fill this; it is never stored.</summary>
    public string? Website { get; set; }
}

public sealed class SubmitInquiryCommand(
    IDbConnectionFactory connectionFactory, 
    ICongregationService congregationService,
    ILogger<SubmitInquiryCommand> logger)
{
    public async Task<int?> ExecuteAsync(InquiryForm form, CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await congregationService.GetCurrentAsync(cancellationToken);
            await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("@FullName", form.FullName.Trim(), DbType.String, size: 150);
            parameters.Add("@Email", form.Email.Trim(), DbType.String, size: 250);
            parameters.Add("@Phone", form.Phone.Trim(), DbType.String, size: 50);
            parameters.Add("@City", form.City.Trim(), DbType.String, size: 100);
            parameters.Add("@State", form.State.Trim(), DbType.String, size: 50);
            parameters.Add("@SpiritualBackground", form.SpiritualBackground.Trim(), DbType.String, size: -1);
            parameters.Add("@PreferredContact", form.PreferredContact, DbType.String, size: 50);
            parameters.Add("@SourceScreen", string.IsNullOrWhiteSpace(form.SourceScreen) ? null : form.SourceScreen.Trim(), DbType.String, size: 250);
            parameters.Add("@ChurchId", profile.Id, DbType.Int32);
            parameters.Add("@NewId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(new CommandDefinition(
                "CACI.usp_InsertFellowshipInquiry",
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

            var id = parameters.Get<int>("@NewId");
            logger.LogInformation("Fellowship inquiry {InquiryId} recorded for Church {ChurchId}.", id, profile.Id);
            return id;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "CACI.usp_InsertFellowshipInquiry failed.");
            return null;
        }
    }
}
