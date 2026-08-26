
CREATE PROCEDURE [dbo].[uspGetClientPreferences]
    @ClientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CP.[Id],
        CP.[ClientId],
        CP.[PreferenceType],
        CP.[PreferenceValue],
        CP.[PreferenceItemId],
        CP.[IsRequired],
        CP.[CreatedDate],
        CP.[UpdatedDate],
        CP.[IsActive],
        LI.[Name] AS PreferenceItemName,
        LI.[Description] AS PreferenceItemDescription
    FROM [dbo].[tblClientPreferences] CP
    LEFT JOIN [dbo].[tblLookupItems] LI ON CP.PreferenceItemId = LI.Id
    WHERE CP.[ClientId] = @ClientId 
        AND CP.[IsActive] = 1
    ORDER BY CP.[PreferenceType], CP.[CreatedDate];
END