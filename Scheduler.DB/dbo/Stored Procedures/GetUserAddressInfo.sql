-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================

--exec [dbo].[GetUserContactInfo] @pID='B5BAA3E6-C962-4A33-B67A-ADC8CA6AE41D'
CREATE PROCEDURE [dbo].[GetUserAddressInfo]
	-- Add the parameters for the stored procedure here
	@pID uniqueidentifier


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

		Select 
			 UA.Id 
			,UA.AddressType as AddressTypeId
			,UA.AddressLine1
			,UA.AddressLine2
			,UA.AddressLIne3
			,UA.StateId
			,UA.CountyId
			,UA.CountryId
			,UA.Latitude
			,UA.Longitude
			,UA.IsPrimaryAddress
			,UA.IsActive
	From   [dbo].tblUserAddress UA 
    WHERE UA.[Id]=@pID




END