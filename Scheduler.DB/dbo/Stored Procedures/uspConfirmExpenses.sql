-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspConfirmExpenses]
    @pExpenseIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Input validation
    IF @pExpenseIds IS NULL OR LEN(TRIM(@pExpenseIds)) = 0
    BEGIN
        RETURN;
    END

    -- Use table variable for expense IDs
    DECLARE @ExpenseIds TABLE (ExpenseId uniqueidentifier PRIMARY KEY);

    -- Parse expense IDs efficiently
    INSERT INTO @ExpenseIds (ExpenseId)
    SELECT CAST(value AS uniqueidentifier)
    FROM STRING_SPLIT(@pExpenseIds, ',')
    WHERE value IS NOT NULL AND LEN(TRIM(value)) > 0;

    -- Update expenses to confirmed
    UPDATE ue
    SET ue.IsConfirmed = 1,
        ue.UpdatedAt = GETUTCDATE()
    FROM [dbo].[tblUserExpense] ue
    INNER JOIN @ExpenseIds ei ON ue.Id = ei.ExpenseId
    WHERE ue.IsActive = 1;

END
