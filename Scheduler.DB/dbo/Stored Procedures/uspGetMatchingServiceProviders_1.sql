
CREATE PROCEDURE [dbo].[uspGetMatchingServiceProviders]
    @ClientId UNIQUEIDENTIFIER,
    @FranchiseId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT
        U.[Id] AS UserId,
        U.[FirstName],
        U.[SurName],
        U.[LastName],
        U.[Email],
        U.[MobileNo],
        U.[PhoneNo],
        U.[Gender] AS GenderId,
        U.[FranchiseId],
        (
            SELECT COUNT(*)
            FROM [dbo].[tblClientPreferences] CP
            INNER JOIN [dbo].[tblServiceProviderAttributes] SPA 
                ON CP.PreferenceType = SPA.AttributeType
                AND (
                    (CP.PreferenceValue = SPA.AttributeValue) 
                    OR 
                    (CP.PreferenceItemId = SPA.AttributeItemId AND CP.PreferenceItemId IS NOT NULL)
                )
            WHERE CP.ClientId = @ClientId 
                AND CP.IsActive = 1
                AND SPA.ServiceProviderId = U.Id
                AND SPA.IsActive = 1
        ) AS MatchScore,
        CASE 
            WHEN EXISTS (
                SELECT 1
                FROM [dbo].[tblClientPreferences] CP
                WHERE CP.ClientId = @ClientId 
                    AND CP.IsRequired = 1 
                    AND CP.IsActive = 1
                    AND NOT EXISTS (
                        SELECT 1
                        FROM [dbo].[tblServiceProviderAttributes] SPA
                        WHERE SPA.ServiceProviderId = U.Id
                            AND SPA.AttributeType = CP.PreferenceType
                            AND SPA.IsActive = 1
                            AND (
                                (CP.PreferenceValue = SPA.AttributeValue) 
                                OR 
                                (CP.PreferenceItemId = SPA.AttributeItemId AND CP.PreferenceItemId IS NOT NULL)
                            )
                    )
            ) THEN 0
            ELSE 1
        END AS MeetsRequiredPreferences
    FROM [dbo].[tblUser] U
    INNER JOIN [dbo].[tblServiceProvider] SP ON SP.UserId = U.Id
    WHERE U.IsActive = 1
        AND SP.IsActive = 1
        AND (@FranchiseId IS NULL OR U.FranchiseId = @FranchiseId)
    ORDER BY MeetsRequiredPreferences DESC, MatchScore DESC, U.FirstName;
END