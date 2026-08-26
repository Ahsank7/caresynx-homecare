  
  
CREATE PROCEDURE [dbo].[InserUpdateServiceProviderContractInfo]   
 -- Add the parameters for the stored procedure here  

  @pOutId uniqueidentifier=null output
 ,@pId uniqueidentifier =null
 ,@pUserId uniqueidentifier=null
 ,@pContractType int=null
 ,@pStartDate  date=null
 ,@pEndDate  date=null
 ,@pOptionId  int=null

 ,@pRate decimal(18,2)=0
 ,@pFrequencyId int=null
 ,@pLoggedInId uniqueidentifier=null

  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
    -- Insert statements for procedure here  
  
 if @pId is null
 begin

 SET @pOutId =  NEWID()	



 INSERT INTO [dbo].[tblServiceProviderContract]
          ( [Id]
           ,[ContractType]
           ,[StartDate]
           ,[EndDate]
           ,[OptionId]

           ,[Rate]
           ,[FrequencyId]
           ,[ServiceProviderUserId]
           ,[isActive]
           ,[CreatedAt]
           ,[CreatedById])
     VALUES
           (@pOutId,
			@pContractType,
			@pStartDate,
			@pEndDate,
			@pOptionId,

			@pRate,
			@pFrequencyId,
			@pUserId,
			1,
			GETDATE(),
			@pLoggedInId)
	
   end

	else
	begin

	 SET @pOutId =  NEWID()	

	 INSERT INTO [dbo].[tblServiceProviderContract]
          ( [Id]
           ,[ContractType]
           ,[StartDate]
           ,[EndDate]
           ,[OptionId]

           ,[Rate]
           ,[FrequencyId]
           ,[ServiceProviderUserId]
           ,[isActive]
           ,[CreatedAt]
           ,[CreatedById])
     VALUES
           (@pOutId,
			@pContractType,
			@pStartDate,
			@pEndDate,
			@pOptionId,

			@pRate,
			@pFrequencyId,
			@pUserId,
			1,
			GETDATE(),
			@pLoggedInId)

  UPDATE [dbo].[tblServiceProviderContract] 
   SET 
	   isActive=0,
	   UpdatedAt=GETDATE(),
	   UpdatedById=@pLoggedInId

  WHERE [Id]=@pId  
  

  END
  

END