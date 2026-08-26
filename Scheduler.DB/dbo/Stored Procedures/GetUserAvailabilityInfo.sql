-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [Contact].[GetUserAvailabilityInfo] @pID='B5BAA3E6-C962-4A33-B67A-ADC8CA6AE41D'
CREATE   PROCEDURE [dbo].[GetUserAvailabilityInfo]
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select 
		     C.Id
		    ,C.[UserId]
			,Cast(C.[StartTime] as nvarchar(8)) [StartTime]
			,Cast(C.[EndTime] as nvarchar(8)) [EndTime]
			,C.Day
			,C.IsActive
			
	From   [dbo].tblUserAvailability C 
    WHERE C.[Id]=@pID




END