CREATE PROCEDURE [dbo].[GetTaskLogs]
    @pTaskId INT,
    @pPageNumber INT = 1,
    @pPageSize INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@pPageNumber - 1) * @pPageSize;
    
    -- Get paginated results
    SELECT 
        tl.[Id],
        tl.[TaskId],
        tl.[ActionType],
        tl.[PreviousValue],
        tl.[NewValue],
        tl.[FieldName],
        tl.[Description],
        tl.[CreatedBy],
        tl.[CreatedDate],
        tl.[IPAddress],
        tl.[UserAgent],
        u.[FirstName] + ' ' + u.[LastName] AS UserName,
        u.[UserNo] AS UserNo
    FROM [dbo].[tblTaskLog] tl
    INNER JOIN [dbo].[tblUser] u ON u.[Id] = tl.[CreatedBy]
    WHERE tl.[TaskId] = @pTaskId
    ORDER BY tl.[CreatedDate] DESC
    OFFSET @Offset ROWS
    FETCH NEXT @pPageSize ROWS ONLY;
    
    -- Get total count
    SELECT COUNT(*) AS TotalRecords
    FROM [dbo].[tblTaskLog] tl
    WHERE tl.[TaskId] = @pTaskId;
END
