	CREATE FUNCTION [dbo].[GetTimeZoneId](@DisplayName NVARCHAR(100))
RETURNS NVARCHAR(100)
AS
BEGIN
    DECLARE @SqlServerId NVARCHAR(100)
    
    SELECT @SqlServerId = [SqlServerIdentifier] 
    FROM [dbo].[tblTimeZoneMapping] 
    WHERE [DisplayName] = @DisplayName 
        AND [IsActive] = 1
    
    -- If not found, return the original name (for standard timezones)
    IF @SqlServerId IS NULL
        SET @SqlServerId = @DisplayName
        
    RETURN @SqlServerId
END