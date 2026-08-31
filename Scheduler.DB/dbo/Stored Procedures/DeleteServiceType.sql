CREATE   PROCEDURE [dbo].[DeleteServiceType]
    @pId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM [dbo].[tblScheduler]
        WHERE ServiceType = @pId
    )
    OR EXISTS (
        SELECT 1
        FROM [dbo].[tblServices] s
        INNER JOIN [dbo].[tblScheduler] sch
            ON sch.CSVServiceIds IS NOT NULL
           AND LEN(LTRIM(RTRIM(sch.CSVServiceIds))) > 0
           AND CHARINDEX(',' + CAST(s.Id AS NVARCHAR(20)) + ',', ',' + REPLACE(sch.CSVServiceIds, ' ', '') + ',') > 0
        WHERE s.ServiceTypeId = @pId
    )
    OR EXISTS (
        SELECT 1
        FROM [dbo].[tblServicesTask] st
        INNER JOIN [dbo].[tblScheduler] sch ON sch.Id = st.ScheduleId
        INNER JOIN [dbo].[tblServices] s ON s.ServiceTypeId = @pId
        WHERE sch.CSVServiceIds IS NOT NULL
          AND LEN(LTRIM(RTRIM(sch.CSVServiceIds))) > 0
          AND CHARINDEX(',' + CAST(s.Id AS NVARCHAR(20)) + ',', ',' + REPLACE(sch.CSVServiceIds, ' ', '') + ',') > 0
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Deleted,
               N'You cannot delete this service type because it is already in use on a schedule or task.' AS [Message];
        RETURN;
    END

    UPDATE [dbo].[tblServices]
    SET IsActive = 0
    WHERE ServiceTypeId = @pId;

    UPDATE [dbo].[tblServicesType]
    SET IsActive = 0
    WHERE Id = @pId;

    SELECT CAST(1 AS BIT) AS Deleted,
           N'Service type deleted successfully.' AS [Message];
END
