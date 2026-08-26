-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [ServiceProvider].[GetServiceProviderInfo] @pID='B5BAA3E6-C962-4A33-B67A-ADC8CA6AE41D'
CREATE PROCEDURE [ServiceProvider].[GetServiceProviderAvailibility] --[CLIENT].[GetClientInfo] '6A1E3D10-C58F-4653-830F-A2E3642880C8'
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select 
		     U.[Id]
			,U.[AvailableDays]    
			,U.[StartTime]	  
			,U.[EndTime]	  
			,U.[ServiceProviderUserId]		  
	From  [dbo].[tblServiceProviderAvailability] U
    WHERE U.[Id]=@pID




END