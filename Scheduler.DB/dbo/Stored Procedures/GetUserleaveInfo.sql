-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [Contact].[GetUserContactInfo] @pID='B5BAA3E6-C962-4A33-B67A-ADC8CA6AE41D'
create   PROCEDURE [dbo].[GetUserleaveInfo]
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
           ,UL.[StartTime]
           ,UL.[EndTime]
           ,UL.[CreatedDate]
           ,UL.[CreatedBy]
           ,UL.[IsActive]
           ,UL.[Notes]
		   ,UL.[Status]
		   ,UL.[Type]
	
	From  [dbo].[tblUserLeave] UL
    WHERE 1=1
    and UL.[Id]=@pID




END