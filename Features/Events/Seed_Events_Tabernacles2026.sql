-- ============================================================================
-- SQL Script: Schema & Stored Procedures & Seed Data for Tabernacles 2026 Event
-- Database Schema: CACI
-- ============================================================================

-- 1. Ensure Schema CACI exists
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'CACI')
BEGIN
    EXEC('CREATE SCHEMA [CACI]');
END
GO

-- 2. Create CACI.Conferences Table if missing
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Conferences' AND schema_id = SCHEMA_ID('CACI'))
BEGIN
    CREATE TABLE [CACI].[Conferences] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [Title] NVARCHAR(255) NOT NULL,
        [StartDate] DATETIME2 NOT NULL,
        [EndDate] DATETIME2 NOT NULL,
        [Location] NVARCHAR(255) NULL,
        [Description] NVARCHAR(MAX) NULL,
        [ImageUrl] NVARCHAR(500) NULL,
        [YoutubeLink] NVARCHAR(500) NULL,
        [RegistrationOpen] BIT NOT NULL DEFAULT 1,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

-- 3. Create CACI.ConferenceSessions Table if missing
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConferenceSessions' AND schema_id = SCHEMA_ID('CACI'))
BEGIN
    CREATE TABLE [CACI].[ConferenceSessions] (
        [Id] INT IDENTITY(1,1) PRIMARY KEY,
        [ConferenceId] INT NOT NULL FOREIGN KEY REFERENCES [CACI].[Conferences]([Id]) ON DELETE CASCADE,
        [SessionTime] DATETIME2 NOT NULL,
        [EndTime] DATETIME2 NULL,
        [Title] NVARCHAR(255) NOT NULL,
        [Speaker] NVARCHAR(255) NULL,
        [Location] NVARCHAR(255) NULL,
        [Description] NVARCHAR(MAX) NULL,
        [SpecialNote] NVARCHAR(255) NULL,
        [SessionType] NVARCHAR(100) NULL
    );
END
GO

-- 4. Create or Alter Stored Procedure: CACI.sp_GetActiveConference
CREATE OR ALTER PROCEDURE [CACI].[sp_GetActiveConference]
    @ConferenceId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @ConferenceId IS NOT NULL AND @ConferenceId > 0
    BEGIN
        SELECT TOP 1 
            [Id], [Title], [StartDate], [EndDate], [Location], 
            [Description], [ImageUrl], [YoutubeLink], [RegistrationOpen], 
            [IsActive], [CreatedDate]
        FROM [CACI].[Conferences]
        WHERE [Id] = @ConferenceId;
    END
    ELSE
    BEGIN
        SELECT TOP 1 
            [Id], [Title], [StartDate], [EndDate], [Location], 
            [Description], [ImageUrl], [YoutubeLink], [RegistrationOpen], 
            [IsActive], [CreatedDate]
        FROM [CACI].[Conferences]
        WHERE [IsActive] = 1
        ORDER BY [StartDate] DESC;
    END
END
GO

-- 5. Create or Alter Stored Procedure: CACI.sp_GetConferenceSessions
CREATE OR ALTER PROCEDURE [CACI].[sp_GetConferenceSessions]
    @ConferenceId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        [Id], [ConferenceId], [SessionTime], [EndTime], [Title], 
        [Speaker], [Location], [Description], [SpecialNote], [SessionType]
    FROM [CACI].[ConferenceSessions]
    WHERE [ConferenceId] = @ConferenceId
    ORDER BY [SessionTime] ASC;
END
GO

-- 6. Seed Event and Schedule Data for Tabernacles 2026: Kingdom Culture
DECLARE @ConfId INT;

SELECT @ConfId = Id FROM [CACI].[Conferences] WHERE [Title] = 'Tabernacles 2026: Kingdom Culture';

IF @ConfId IS NULL
BEGIN
    INSERT INTO [CACI].[Conferences] 
        ([Title], [StartDate], [EndDate], [Location], [Description], [ImageUrl], [IsActive], [RegistrationOpen])
    VALUES 
        ('Tabernacles 2026: Kingdom Culture', '2026-09-27', '2026-10-04', 'Ramer, AL', 
         'September 27 – October 4, 2026 • Ramer, AL', '/images/images/Tabernacles 2026.jpg', 1, 1);
         
    SET @ConfId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    UPDATE [CACI].[Conferences] SET [IsActive] = 1 WHERE [Id] = @ConfId;
    DELETE FROM [CACI].[ConferenceSessions] WHERE [ConferenceId] = @ConfId;
END

-- Insert Day 1: Sunday, September 27th — Arrival Day
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Description], [SessionType])
VALUES 
(@ConfId, '2026-09-27 00:00:00', NULL, 'Build Sukkahs & Setup', 'Build Sukkahs • Set up A/V Equipment • Set up Merch Store', 'Setup'),
(@ConfId, '2026-09-27 19:00:00', NULL, 'OPENING (Ranch Guidelines)', NULL, 'Session');

-- Insert Day 2: Monday, September 28th
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Description], [SessionType])
VALUES 
(@ConfId, '2026-09-28 10:00:00', '2026-09-28 12:00:00', 'Registration / Merch Store Open', NULL, 'Registration'),
(@ConfId, '2026-09-28 12:00:00', '2026-09-28 13:00:00', 'Lunch', NULL, 'Meal'),
(@ConfId, '2026-09-28 13:00:00', NULL, 'Fellowship Time', NULL, 'Fellowship'),
(@ConfId, '2026-09-28 17:00:00', '2026-09-28 18:00:00', 'Dinner', NULL, 'Meal'),
(@ConfId, '2026-09-28 19:00:00', NULL, 'Sound Check Worship', NULL, 'Worship');

-- Insert Day 3: Tuesday, September 29th
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Speaker], [Description], [SessionType])
VALUES 
(@ConfId, '2026-09-29 07:00:00', '2026-09-29 09:00:00', 'Team Fishing Competition', NULL, NULL, 'Activity'),
(@ConfId, '2026-09-29 09:30:00', NULL, 'Morning Devotion by the Lake', 'Elder Robbie Darnell', NULL, 'Worship'),
(@ConfId, '2026-09-29 10:00:00', '2026-09-29 11:30:00', 'Tribal Challenge Opening', NULL, NULL, 'Youth'),
(@ConfId, '2026-09-29 12:00:00', '2026-09-29 13:00:00', 'Lunch', NULL, NULL, 'Meal'),
(@ConfId, '2026-09-29 13:00:00', NULL, 'Sukkah Discussions – Kingdom Culture', 'Elder Stephen Velie', NULL, 'Session'),
(@ConfId, '2026-09-29 14:00:00', '2026-09-29 15:00:00', 'Youth Games', NULL, NULL, 'Youth'),
(@ConfId, '2026-09-29 17:00:00', '2026-09-29 18:00:00', 'Dinner', NULL, NULL, 'Meal'),
(@ConfId, '2026-09-29 18:30:00', NULL, 'Prayer', NULL, NULL, 'Worship'),
(@ConfId, '2026-09-29 19:00:00', NULL, 'Worship Service', 'Presiding Bishop Jesse White', NULL, 'Worship');

-- Insert Day 4: Wednesday, September 30th
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Speaker], [Description], [SessionType])
VALUES 
(@ConfId, '2026-09-30 10:00:00', '2026-09-30 11:30:00', 'Tribal Challenge #2', NULL, NULL, 'Youth'),
(@ConfId, '2026-09-30 12:00:00', '2026-09-30 13:00:00', 'Lunch', NULL, NULL, 'Meal'),
(@ConfId, '2026-09-30 13:00:00', NULL, 'Sukkah Discussions – Kingdom Culture', 'Elder Dave Seibold', NULL, 'Session'),
(@ConfId, '2026-09-30 14:00:00', '2026-09-30 15:00:00', 'Youth Games', NULL, NULL, 'Youth'),
(@ConfId, '2026-09-30 17:00:00', '2026-09-30 18:00:00', 'Dinner', NULL, NULL, 'Meal'),
(@ConfId, '2026-09-30 18:30:00', NULL, 'Prayer', NULL, NULL, 'Worship'),
(@ConfId, '2026-09-30 19:00:00', NULL, 'Worship Service', 'Elder Brent Price', NULL, 'Worship');

-- Insert Day 5: Thursday, October 1st
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Speaker], [Location], [Description], [SessionType])
VALUES 
(@ConfId, '2026-10-01 10:00:00', '2026-10-01 11:00:00', 'Tribal Challenge #3', NULL, NULL, NULL, 'Youth'),
(@ConfId, '2026-10-01 11:00:00', '2026-10-01 12:00:00', 'Registration / Merch Store Open', NULL, NULL, NULL, 'Registration'),
(@ConfId, '2026-10-01 12:00:00', '2026-10-01 13:00:00', 'Lunch', NULL, NULL, NULL, 'Meal'),
(@ConfId, '2026-10-01 13:00:00', NULL, 'Sukkah Discussions – Kingdom Culture', 'Elder Bradley Wallace', NULL, NULL, 'Session'),
(@ConfId, '2026-10-01 14:00:00', '2026-10-01 15:00:00', 'Kick Back Ranch Games and Activities at the Pavilion', NULL, 'Pavilion', NULL, 'Youth'),
(@ConfId, '2026-10-01 17:00:00', '2026-10-01 18:00:00', 'Dinner', NULL, NULL, NULL, 'Meal'),
(@ConfId, '2026-10-01 18:30:00', NULL, 'Prayer at the Lake', NULL, 'By the Lake', NULL, 'Worship'),
(@ConfId, '2026-10-01 19:00:00', NULL, 'Worship Service at the Lake', 'Bishop Mark Jackson', 'By the Lake', NULL, 'Worship'),
(@ConfId, '2026-10-01 20:30:00', NULL, 'Shake the Lake', NULL, NULL, NULL, 'Fellowship'),
(@ConfId, '2026-10-01 22:00:00', '2026-10-02 00:00:00', 'Youth Night – S’mores and Slide at the Water Slide', NULL, 'Water Slide', NULL, 'Youth');

-- Insert Day 6: Friday, October 2nd
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Speaker], [Description], [SessionType])
VALUES 
(@ConfId, '2026-10-02 10:00:00', '2026-10-02 11:30:00', 'Tribal Challenge #4', NULL, NULL, 'Youth'),
(@ConfId, '2026-10-02 12:00:00', '2026-10-02 13:00:00', 'Lunch', NULL, NULL, 'Meal'),
(@ConfId, '2026-10-02 12:00:00', '2026-10-02 14:00:00', 'CACI Ministry Luncheon', NULL, NULL, 'Meal'),
(@ConfId, '2026-10-02 14:30:00', '2026-10-02 16:00:00', 'Youth Color Wars', NULL, NULL, 'Youth'),
(@ConfId, '2026-10-02 17:00:00', '2026-10-02 18:00:00', 'Dinner', NULL, NULL, 'Meal'),
(@ConfId, '2026-10-02 18:30:00', NULL, 'Prayer', NULL, NULL, 'Worship'),
(@ConfId, '2026-10-02 19:00:00', NULL, 'Worship Service', 'Bishop Chad Sullivan', NULL, 'Worship');

-- Insert Day 7: Saturday, October 3rd
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Speaker], [Location], [Description], [SessionType])
VALUES 
(@ConfId, '2026-10-03 10:00:00', NULL, 'Sukkah Discussions – Kingdom Culture', 'Elder Scott Clifton', NULL, NULL, 'Session'),
(@ConfId, '2026-10-03 12:00:00', '2026-10-03 13:00:00', 'Lunch', NULL, NULL, NULL, 'Meal'),
(@ConfId, '2026-10-03 13:30:00', NULL, 'Communion', NULL, NULL, NULL, 'Worship'),
(@ConfId, '2026-10-03 13:30:00', NULL, 'Sabbath Worship Service', 'Presiding Bishop Jesse White', NULL, NULL, 'Worship'),
(@ConfId, '2026-10-03 16:00:00', NULL, 'Sukkah and Ranch Cleanup', NULL, NULL, NULL, 'Setup'),
(@ConfId, '2026-10-03 18:00:00', '2026-10-03 19:00:00', 'Unity Brisket Dinner at Pavilion', NULL, 'Pavilion', NULL, 'Meal'),
(@ConfId, '2026-10-03 19:00:00', NULL, 'Last Night of Fellowship', NULL, NULL, NULL, 'Fellowship');

-- Insert Day 8: Sunday, October 4th — Departure Day
INSERT INTO [CACI].[ConferenceSessions] ([ConferenceId], [SessionTime], [EndTime], [Title], [Description], [SessionType])
VALUES 
(@ConfId, '2026-10-04 00:00:00', NULL, 'Clean Up Day / Travel Home Day', NULL, 'Departure');

PRINT 'Successfully created/verified CACI schema, stored procedures [CACI].[sp_GetActiveConference] and [CACI].[sp_GetConferenceSessions], and seeded Tabernacles 2026 event.';
GO
