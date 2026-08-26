-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [dbo].[GetUserTransactionInfo] @pID='4443d895-be7b-4396-a2b3-aebb9b571af2'
CREATE PROCEDURE [dbo].[GetUserTransactionInfo]
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select 

			 UA.TransactionId
			,UA.UserId
			,UA.Remarks
			,UA.ReferenceId
			,(
			    CASE when UA.StatusId=1 Then 'Succeeded'
			         when UA.StatusId=2 Then 'Failed'
				ELSE 'Undefined' 
				END
			
			
			) [Status]
			,UA.TransactionDate
			,UA.CardId
			,UA.BankAccountId
			,UA.TypeId
			,UA.StatusId
	From   [dbo].tblTransaction UA 
    WHERE UA.[TransactionId]=@pID




END