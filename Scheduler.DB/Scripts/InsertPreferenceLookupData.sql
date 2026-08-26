-- =============================================
-- Script to insert lookup data for Preference System
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Insert lookup types and items for client preferences and service provider attributes
-- =============================================

-- Insert Preference/Attribute Types into tblLookups if not exists
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookups] WHERE [Name] = 'PreferenceTypes')
BEGIN
    INSERT INTO [dbo].[tblLookups] ([Name], [Description], [DisplayName], [IsActive], [IsVisible])
    VALUES ('PreferenceTypes', 'Types of preferences that can be set for clients and attributes for service providers', 'Preference Types', 1, 1);
END

-- Get the PreferenceTypes lookup ID
DECLARE @PreferenceTypesLookupId INT;
SELECT @PreferenceTypesLookupId = Id FROM [dbo].[tblLookups] WHERE [Name] = 'PreferenceTypes';

-- Insert Gender Preference Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Gender' AND [Name] = 'Male')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Gender', 'Male', 'Male gender preference/attribute', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Gender' AND [Name] = 'Female')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Gender', 'Female', 'Female gender preference/attribute', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Gender' AND [Name] = 'Non-Binary')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Gender', 'Non-Binary', 'Non-binary gender preference/attribute', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Gender' AND [Name] = 'No Preference')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Gender', 'No Preference', 'No gender preference', 1, GETDATE());
END

-- Insert Smoking Status Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'SmokingStatus' AND [Name] = 'Non-Smoker')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('SmokingStatus', 'Non-Smoker', 'Does not smoke', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'SmokingStatus' AND [Name] = 'Smoker')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('SmokingStatus', 'Smoker', 'Smokes regularly', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'SmokingStatus' AND [Name] = 'No Preference')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('SmokingStatus', 'No Preference', 'No smoking preference', 1, GETDATE());
END

-- Insert Language Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Language' AND [Name] = 'English')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Language', 'English', 'English language', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Language' AND [Name] = 'Spanish')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Language', 'Spanish', 'Spanish language', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Language' AND [Name] = 'French')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Language', 'French', 'French language', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Language' AND [Name] = 'German')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Language', 'German', 'German language', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Language' AND [Name] = 'Mandarin')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Language', 'Mandarin', 'Mandarin Chinese language', 1, GETDATE());
END

-- Insert Age Range Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '18-30')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '18-30', 'Age range 18 to 30 years', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '31-45')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '31-45', 'Age range 31 to 45 years', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '46-60')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '46-60', 'Age range 46 to 60 years', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'AgeRange' AND [Name] = '60+')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('AgeRange', '60+', 'Age 60 years and above', 1, GETDATE());
END

-- Insert Experience Level Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Entry Level (0-2 years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Entry Level (0-2 years)', '0-2 years of experience', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Mid Level (3-5 years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Mid Level (3-5 years)', '3-5 years of experience', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Senior Level (6-10 years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Senior Level (6-10 years)', '6-10 years of experience', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Experience' AND [Name] = 'Expert Level (10+ years)')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Experience', 'Expert Level (10+ years)', '10 or more years of experience', 1, GETDATE());
END

-- Insert Pet Friendly Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'PetFriendly' AND [Name] = 'Yes')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('PetFriendly', 'Yes', 'Comfortable working with pets', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'PetFriendly' AND [Name] = 'No')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('PetFriendly', 'No', 'Not comfortable working with pets', 1, GETDATE());
END

-- Insert Transportation Mode Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'TransportationMode' AND [Name] = 'Own Vehicle')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('TransportationMode', 'Own Vehicle', 'Has own vehicle for transportation', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'TransportationMode' AND [Name] = 'Public Transport')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('TransportationMode', 'Public Transport', 'Uses public transportation', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'TransportationMode' AND [Name] = 'Bicycle')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('TransportationMode', 'Bicycle', 'Uses bicycle for transportation', 1, GETDATE());
END

-- Insert Certification Items
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'First Aid Certified')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'First Aid Certified', 'Has first aid certification', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'CPR Certified')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'CPR Certified', 'Has CPR certification', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'Licensed Professional')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'Licensed Professional', 'Has professional license', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblLookupItems] WHERE [LookupType] = 'Certification' AND [Name] = 'Background Check Cleared')
BEGIN
    INSERT INTO [dbo].[tblLookupItems] ([LookupType], [Name], [Description], [IsActive], [InsertedAt])
    VALUES ('Certification', 'Background Check Cleared', 'Passed background verification', 1, GETDATE());
END

PRINT 'Preference lookup data inserted successfully!';

