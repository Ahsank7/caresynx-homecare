-- Get all time-based rates for an organization
CREATE OR ALTER PROCEDURE [dbo].[uspGetOrganizationTimeBasedRates]
    @pOrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        otbr.Id,
        otbr.OrganizationId,
        otbr.ServiceTypeId,
        st.Name AS ServiceTypeName,
        otbr.ServiceId,
        s.Name AS ServiceName,
        otbr.DayOfWeek,
        CASE otbr.DayOfWeek
            WHEN 0 THEN 'Sunday'
            WHEN 1 THEN 'Monday'
            WHEN 2 THEN 'Tuesday'
            WHEN 3 THEN 'Wednesday'
            WHEN 4 THEN 'Thursday'
            WHEN 5 THEN 'Friday'
            WHEN 6 THEN 'Saturday'
        END AS DayName,
        otbr.StartTime,
        otbr.EndTime,
        otbr.ClientRate,
        otbr.WageRate,
        otbr.IsActive,
        otbr.CreatedAt,
        otbr.UpdatedAt
    FROM [dbo].[tblOrganizationTimeBasedRates] otbr
    LEFT JOIN [dbo].[tblServicesType] st ON otbr.ServiceTypeId = st.Id
    LEFT JOIN [dbo].[tblServices] s ON otbr.ServiceId = s.Id
    WHERE otbr.OrganizationId = @pOrganizationId
    ORDER BY otbr.DayOfWeek, otbr.StartTime;
END
