-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================



CREATE PROCEDURE [Organization].[InsertUpdateOrganization] 
	-- Add the parameters for the stored procedure here
	@pId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pName nvarchar(50)=null,   
	@pDescription nvarchar(50)=null,
	@pDefaultBillingRate decimal(18,2)=0,
	@pDefaultWageRate decimal(18,2)=0,
	@pCompleteAddress  nvarchar(500)=null,
	@pContactNo  nvarchar(50)=null,
	@pEmail  nvarchar(50)=null,
	@pWebSite  nvarchar(50)=null,
	@pCurrencyId  int=0,
	@pcalculationTypeId int=0,
	@ptaxPercentage decimal(18,2)=0,
	@pdiscountPercentage decimal(18,2)=0,
	@pCurrencySignId   int=0,
	@pServiceRateForBilling int=1,
	@pTimeZone  nvarchar(100)=null

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	

	if @pId is null
	begin


SET @pOutId =  NEWID()	

-----------------------------------------------------Organization Basic Info--------------------------------------------


INSERT INTO [dbo].[tblOrganization]
           (Id
		   ,[Name]
           ,[Description]
		   ,DefaultBillingRate
		   ,DefaultWageRate
		   ,CompleteAddress
		   ,ContactNo
		   ,CurrencyId
		   ,Email
		   ,WebSite
		   ,CalculationTypeId
		   ,TaxPercentage
		   ,DiscountPercentage
		   ,CurrencySignId
		   ,ServiceRateForBilling
		   ,TimeZone
           ,[IsActive])
     VALUES(@pOutId,@pName,@pDescription,@pDefaultBillingRate,@pDefaultWageRate,@pCompleteAddress,@pContactNo,@pCurrencyId,@pEmail,@pWebSite,@pcalculationTypeId,@ptaxPercentage,@pdiscountPercentage,@pCurrencySignId,@pServiceRateForBilling,@pTimeZone,1)


     EXEC dbo.sp_Seed_MasterData @OrganizationId = @pOutId

	end

	else
	begin

		UPDATE [dbo].[tblOrganization]
		  SET  [Name]     =@pName
			  ,[Description]	   =@pDescription
			  ,DefaultBillingRate = @pDefaultBillingRate
			  ,DefaultWageRate	  = @pDefaultWageRate
			  ,CompleteAddress	  = @pCompleteAddress
			  ,ContactNo		  = @pContactNo
		      ,[CurrencyId]  =      @pCurrencyId
			  ,Email  =  @pEmail
			  ,WebSite  = @pWebSite
			  ,CalculationTypeId = @pCalculationTypeId
		      ,TaxPercentage =@pTaxPercentage
		      ,DiscountPercentage = @pDiscountPercentage
			  ,CurrencySignId = @pCurrencySignId
			  ,ServiceRateForBilling = @pServiceRateForBilling
			  ,TimeZone = @pTimeZone
			  ,[IsActive]	   =1
		WHERE [Id]=@pId

		SET @pOutId = @pId	


	end

END