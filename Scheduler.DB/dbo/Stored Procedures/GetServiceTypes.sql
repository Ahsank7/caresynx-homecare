-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [dbo].[GetUserExpenseInfo] @pID='5DC92804-1ACE-4D6A-A2E6-D642220C2ED9'
CREATE   PROCEDURE [dbo].[GetServiceTypes]
	-- Add the parameters for the stored procedure here
	@pOrganizationID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	Select	
	   d.[Id]
      ,d.[Name]
      ,d.[Description]
	From  dbo.tblServicesType d
    WHERE 1=1
    and d.[OrganizationId]=@pOrganizationID
	and IsNull(d.IsActive,0)=1




END