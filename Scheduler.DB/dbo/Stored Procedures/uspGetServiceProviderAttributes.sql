-- =============================================
-- Author:		Scheduler System
-- Create date: 2026-01-21
-- Description:	Get all attributes for a service provider
-- =============================================
CREATE PROCEDURE [dbo].[uspGetServiceProviderAttributes]
    @ServiceProviderId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        SPA.[Id],
        SPA.[ServiceProviderId],
        SPA.[AttributeType],
        SPA.[AttributeValue],
        SPA.[AttributeItemId],
        SPA.[CreatedDate],
        SPA.[UpdatedDate],
        SPA.[IsActive],
        LI.[Name] AS AttributeItemName,
        LI.[Description] AS AttributeItemDescription
    FROM [dbo].[tblServiceProviderAttributes] SPA
    LEFT JOIN [dbo].[tblLookupItems] LI ON SPA.AttributeItemId = LI.Id
    WHERE SPA.[ServiceProviderId] = @ServiceProviderId 
        AND SPA.[IsActive] = 1
    ORDER BY SPA.[AttributeType], SPA.[CreatedDate];
END

