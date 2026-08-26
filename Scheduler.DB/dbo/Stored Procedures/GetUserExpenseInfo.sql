-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [dbo].[GetUserExpenseInfo] @pID='5DC92804-1ACE-4D6A-A2E6-D642220C2ED9'
CREATE   PROCEDURE [dbo].[GetUserExpenseInfo]
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	Select	
	   UL.[Id]
      ,UL.[UserId]
      ,UL.[Date]
      ,UL.[TaskId]
      ,UL.[Type]
      ,UL.[Amount]
      ,UL.[IsPaid]
      ,UL.[CreatedAt]
      ,UL.[UpdatedAt]
      ,UL.[CreatedBy]
      ,UL.[UpdatedBy]
      ,UL.[IsActive]
      ,UL.[IsConfirmed]
	  ,UL.Notes
	
	From  dbo.tblUserExpense UL
    WHERE 1=1
    and UL.[Id]=@pID




END