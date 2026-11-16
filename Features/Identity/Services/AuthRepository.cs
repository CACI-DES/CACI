using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using CACI_Life.Features.Identity.Models;
using CACI_Life.Features.Email;

namespace CACI_Life.Features.Identity.Services;

public interface IAuthRepository
{
    Task<AuthResponse> ValidateUserAsync(string email, string password);
    Task<MinistryProfileDto?> GetProfileByEmailAsync(string email);
    Task<RequestAccessResult> RequestAccessAsync(RequestAccessModel model);
}

public class AuthRepository : IAuthRepository
{
    private readonly string _connectionString;
    private readonly IPasswordHasher<object> _passwordHasher;
    private readonly IEmailService? _emailService;

    public AuthRepository(
        IConfiguration configuration, 
        IPasswordHasher<object> passwordHasher,
        IEmailService? emailService = null)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("DefaultConnection string is not configured.");
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<AuthResponse> ValidateUserAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return new AuthResponse { Succeeded = false, ErrorMessage = "Email and password are required." };
        }

        try
        {
            using var connection = CreateConnection();
            var user = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "dbo.Auth_usp_GetUserSecurityInfo",
                new { Email = email.Trim() },
                commandType: CommandType.StoredProcedure
            );

            if (user == null)
            {
                return new AuthResponse { Succeeded = false, ErrorMessage = "Invalid email or password." };
            }

            string passwordHash = user.PasswordHash ?? string.Empty;
            bool verified = false;

            // 1. Try ASP.NET Identity PasswordHasher
            try
            {
                var result = _passwordHasher.VerifyHashedPassword(new object(), passwordHash, password);
                if (result != PasswordVerificationResult.Failed)
                {
                    verified = true;
                }
            }
            catch
            {
                // Fallback to BCrypt
            }

            // 2. Fallback to BCrypt check
            if (!verified)
            {
                try
                {
                    if (BCrypt.Net.BCrypt.Verify(password, passwordHash))
                    {
                        verified = true;
                    }
                }
                catch
                {
                    // Ignore format errors
                }
            }

            if (!verified)
            {
                return new AuthResponse { Succeeded = false, ErrorMessage = "Invalid email or password." };
            }

            // 3. Verify user approval status & ChurchId
            int? userChurchId = null;
            try
            {
                var meta = await connection.QueryFirstOrDefaultAsync<(bool? IsApproved, int? ChurchId)>(
                    "SELECT IsApproved, ChurchId FROM CACI.AspNetUsers WHERE Id = @Id",
                    new { Id = (string)user.UserId }
                );

                if (meta.IsApproved == false)
                {
                    return new AuthResponse 
                    { 
                        Succeeded = false, 
                        ErrorMessage = "Your account is pending administrator approval. Please check back shortly." 
                    };
                }
                userChurchId = meta.ChurchId;
            }
            catch
            {
                // If column doesn't exist or error, continue
            }

            string rolesList = user.RolesList ?? "";
            var roles = rolesList.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(r => r.Trim())
                                 .ToList();

            return new AuthResponse
            {
                Succeeded = true,
                UserId = user.UserId?.ToString(),
                Email = user.Email,
                ChurchId = userChurchId,
                Roles = roles
            };
        }
        catch (Exception ex)
        {
            return new AuthResponse { Succeeded = false, ErrorMessage = $"Authentication error: {ex.Message}" };
        }
    }

    public async Task<MinistryProfileDto?> GetProfileByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;

        try
        {
            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<MinistryProfileDto>(
                "SELECT FirstName, LastName, UserId FROM CACI.Ministry_Profiles WHERE Email = @Email",
                new { Email = email.Trim() }
            );
        }
        catch
        {
            return null;
        }
    }

    public async Task<RequestAccessResult> RequestAccessAsync(RequestAccessModel model)
    {
        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            return new RequestAccessResult { Succeeded = false, ErrorMessage = "Full name is required." };
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return new RequestAccessResult { Succeeded = false, ErrorMessage = "A valid email address is required." };
        }

        if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
        {
            return new RequestAccessResult { Succeeded = false, ErrorMessage = "Password must be at least 6 characters long." };
        }

        try
        {
            using var connection = CreateConnection();
            var emailClean = model.Email.Trim();

            var userExists = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM CACI.AspNetUsers WHERE Email = @Email",
                new { Email = emailClean }
            );

            if (userExists > 0)
            {
                return new RequestAccessResult 
                { 
                    Succeeded = false, 
                    ErrorMessage = "An account or pending request already exists for this email address." 
                };
            }

            var userId = Guid.NewGuid().ToString();
            var passwordHash = _passwordHasher.HashPassword(new object(), model.Password);

            var newUser = new
            {
                Id = userId,
                UserName = emailClean,
                NormalizedUserName = emailClean.ToUpperInvariant(),
                Email = emailClean,
                NormalizedEmail = emailClean.ToUpperInvariant(),
                EmailConfirmed = true,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                FullName = model.FullName.Trim(),
                Congregation = model.Congregation?.Trim(),
                Notes = model.Notes?.Trim(),
                IsApproved = false
            };

            const string insertUserSql = @"
                INSERT INTO CACI.AspNetUsers 
                (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, FullName, IsApproved, Congregation, Notes)
                VALUES 
                (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnabled, @AccessFailedCount, @FullName, @IsApproved, @Congregation, @Notes)";

            await connection.ExecuteAsync(insertUserSql, newUser);

            // Assign 'Member' role
            var memberRoleId = await connection.QueryFirstOrDefaultAsync<string>(
                "SELECT Id FROM CACI.AspNetRoles WHERE Name = 'Member'"
            );

            if (string.IsNullOrEmpty(memberRoleId))
            {
                memberRoleId = Guid.NewGuid().ToString();
                await connection.ExecuteAsync(
                    "INSERT INTO CACI.AspNetRoles (Id, Name, NormalizedName) VALUES (@Id, 'Member', 'MEMBER')",
                    new { Id = memberRoleId }
                );
            }

            await connection.ExecuteAsync(
                "INSERT INTO CACI.AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)",
                new { UserId = userId, RoleId = memberRoleId }
            );

            // Send notification emails
            if (_emailService != null)
            {
                try
                {
                    // Admin notification
                    var adminSubject = $"New CACI Account Request: {model.FullName}";
                    var adminBody = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                            <h2 style='color: #174477; margin-top: 0;'>New CACI Life Account Request</h2>
                            <p>A new member has submitted a registration request on CACI Life:</p>
                            <table style='width: 100%; border-collapse: collapse; margin-top: 15px;'>
                                <tr><td style='padding: 6px; font-weight: bold; width: 140px;'>Full Name:</td><td style='padding: 6px;'>{model.FullName}</td></tr>
                                <tr><td style='padding: 6px; font-weight: bold;'>Email:</td><td style='padding: 6px;'><a href='mailto:{emailClean}'>{emailClean}</a></td></tr>
                                <tr><td style='padding: 6px; font-weight: bold;'>Phone:</td><td style='padding: 6px;'>{(string.IsNullOrWhiteSpace(model.PhoneNumber) ? "Not provided" : model.PhoneNumber)}</td></tr>
                                <tr><td style='padding: 6px; font-weight: bold;'>Congregation:</td><td style='padding: 6px;'>{(string.IsNullOrWhiteSpace(model.Congregation) ? "Not provided" : model.Congregation)}</td></tr>
                                <tr><td style='padding: 6px; font-weight: bold;'>Notes:</td><td style='padding: 6px;'>{(string.IsNullOrWhiteSpace(model.Notes) ? "None" : model.Notes)}</td></tr>
                            </table>
                            <p style='margin-top: 20px; font-size: 13px; color: #64748b;'>
                                Status: <strong>Pending Administrator Approval</strong> (IsApproved = 0).
                            </p>
                        </div>";

                    await _emailService.SendEmailAsync("Contact@caci.life", adminSubject, adminBody);

                    // User confirmation
                    var userSubject = "CACI Life - Account Request Received";
                    var userBody = $@"
                        <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e2e8f0; border-radius: 8px;'>
                            <div style='text-align: center; margin-bottom: 20px;'>
                                <h2 style='color: #174477; margin: 0;'>Covenant Apostolic Congregations Intl.</h2>
                                <p style='color: #64748b; font-size: 13px; margin: 5px 0 0 0;'>Official Account Registration</p>
                            </div>
                            <p>Dear {model.FullName},</p>
                            <p>Thank you for submitting your account request for <strong>CACI Life</strong>. We have successfully logged your details into our directory.</p>
                            <div style='background-color: #f8fafc; border-left: 4px solid #174477; padding: 12px 16px; margin: 20px 0;'>
                                <p style='margin: 0; font-size: 14px;'><strong>Status:</strong> Pending Leadership Approval</p>
                                <p style='margin: 4px 0 0 0; font-size: 13px; color: #64748b;'>Your account is currently under review by our administrative team. Once approved, you will be able to log in securely with your credentials.</p>
                            </div>
                            <p>If you have any questions or urgent ministerial needs, feel free to reply directly to this email or contact us at <a href='mailto:Contact@caci.life' style='color: #174477;'>Contact@caci.life</a>.</p>
                            <p style='margin-top: 30px; border-top: 1px solid #e2e8f0; padding-top: 15px; font-size: 12px; color: #94a3b8;'>
                                Covenant Apostolic Congregations International &bull; <a href='https://caci.life' style='color: #174477;'>caci.life</a>
                            </p>
                        </div>";

                    await _emailService.SendEmailAsync(emailClean, userSubject, userBody);
                }
                catch (Exception mailEx)
                {
                    Console.WriteLine($"[AuthRepository] Notice: Email notification could not be dispatched: {mailEx.Message}");
                }
            }

            return new RequestAccessResult { Succeeded = true };
        }
        catch (Exception ex)
        {
            return new RequestAccessResult { Succeeded = false, ErrorMessage = $"Failed to submit account request: {ex.Message}" };
        }
    }
}
