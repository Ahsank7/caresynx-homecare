-- =============================================
-- Diagnostic Script for Preference Lookup Issues
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Check if lookup data exists and identify any mismatches
-- =============================================

PRINT '=== Checking Lookup Data Status ==='
PRINT ''

-- Check if preference lookup types exist
PRINT '1. Checking Lookups Table:'
SELECT 
    Id,
    [Name],
    [Description],
    IsActive
FROM [dbo].[tblLookups]
WHERE [Name] IN ('Gender', 'SmokingStatus', 'Language', 'AgeRange', 'Experience', 'PetFriendly', 'TransportationMode', 'Certification', 'PreferenceTypes')
ORDER BY [Name];

PRINT ''
PRINT '2. Checking Lookup Items by Type:'
SELECT 
    LookupType,
    COUNT(*) AS ItemCount
FROM [dbo].[tblLookupItems]
WHERE LookupType IN ('Gender', 'SmokingStatus', 'Language', 'AgeRange', 'Experience', 'PetFriendly', 'TransportationMode', 'Certification')
GROUP BY LookupType
ORDER BY LookupType;

PRINT ''
PRINT '3. All Lookup Items:'
SELECT 
    Id,
    LookupType,
    [Name],
    [Description],
    IsActive
FROM [dbo].[tblLookupItems]
WHERE LookupType IN ('Gender', 'SmokingStatus', 'Language', 'AgeRange', 'Experience', 'PetFriendly', 'TransportationMode', 'Certification')
ORDER BY LookupType, [Name];

PRINT ''
PRINT '4. Current Client Preferences with Lookup Info:'
SELECT 
    CP.[Id],
    CP.[ClientId],
    CP.[PreferenceType],
    CP.[PreferenceItemId],
    CP.[PreferenceValue],
    CP.[IsRequired],
    LI.[Name] AS LookupItemName,
    LI.[LookupType] AS LookupItemType,
    CASE 
        WHEN LI.Id IS NULL THEN 'MISSING - Lookup item does not exist'
        WHEN CP.PreferenceType != LI.LookupType THEN 'MISMATCH - Type does not match'
        ELSE 'OK'
    END AS Status
FROM [dbo].[tblClientPreferences] CP
LEFT JOIN [dbo].[tblLookupItems] LI ON CP.PreferenceItemId = LI.Id
WHERE CP.IsActive = 1
ORDER BY CP.[PreferenceType];

PRINT ''
PRINT '5. Current Service Provider Attributes with Lookup Info:'
SELECT 
    SPA.[Id],
    SPA.[ServiceProviderId],
    SPA.[AttributeType],
    SPA.[AttributeItemId],
    SPA.[AttributeValue],
    LI.[Name] AS LookupItemName,
    LI.[LookupType] AS LookupItemType,
    CASE 
        WHEN LI.Id IS NULL THEN 'MISSING - Lookup item does not exist'
        WHEN SPA.AttributeType != LI.LookupType THEN 'MISMATCH - Type does not match'
        ELSE 'OK'
    END AS Status
FROM [dbo].[tblServiceProviderAttributes] SPA
LEFT JOIN [dbo].[tblLookupItems] LI ON SPA.AttributeItemId = LI.Id
WHERE SPA.IsActive = 1
ORDER BY SPA.[AttributeType];

PRINT ''
PRINT '=== Diagnostic Complete ==='

