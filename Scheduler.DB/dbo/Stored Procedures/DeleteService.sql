CREATE    PROCEDURE [dbo].[DeleteService]
    @pId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @csvNeedle NVARCHAR(30) = ',' + CAST(@pId AS NVARCHAR(20)) + ',';

    IF EXISTS (
        SELECT 1
        FROM [dbo].[tblScheduler] sch
        WHERE sch.CSVServiceIds IS NOT NULL
          AND LEN(LTRIM(RTRIM(sch.CSVServiceIds))) > 0
          AND CHARINDEX(@csvNeedle, ',' + REPLACE(sch.CSVServiceIds, ' ', '') + ',') > 0
    )
    OR EXISTS (
        SELECT 1
        FROM [dbo].[tblServicesTask] st
        INNER JOIN [dbo].[tblScheduler] sch ON sch.Id = st.ScheduleId
        WHERE sch.CSVServiceIds IS NOT NULL
          AND LEN(LTRIM(RTRIM(sch.CSVServiceIds))) > 0
          AND CHARINDEX(@csvNeedle, ',' + REPLACE(sch.CSVServiceIds, ' ', '') + ',') > 0
    )
    BEGIN
        SELECT CAST(0 AS BIT) AS Deleted,
               N'You cannot delete this service because it is already in use on a schedule or task.' AS [Message];
        RETURN;
    END

    UPDATE [dbo].[tblServices]
    SET IsActive = 0
    WHERE Id = @pId;

    SELECT CAST(1 AS BIT) AS Deleted,
           N'Service deleted successfully.' AS [Message];
END
