  
  
CREATE   PROCEDURE [dbo].[InserUpdateUserBankAccountInfo]   
 -- Add the parameters for the stored procedure here  

 @pOutId uniqueidentifier=null output,  
 @pUserId uniqueidentifier=null,  
 @pBankAccountId  uniqueidentifier =null,          
 @pIBAN nvarchar(500)=null,   
 @pAccountHolderName nvarchar(500)=null,
 @pBranchCode nvarchar(500)=null,
 @pBankId nvarchar(500)=null,
 @pAccountNumber nvarchar(500)=null,
 @pConnectedAccountId nvarchar(255)=null
  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
 
    -- Insert statements for procedure here  
 
 if @pBankAccountId is null
 begin

 SET @pOutId =  NEWID()	

 INSERT INTO [dbo].[tblBankAccount]
           ([BankAccountId]
           ,[UserId]
           ,[AccountHolderName]
           ,[AccountNumber]
           ,[BankId]
           ,[BranchCode]
           ,[IBAN]
           ,[ConnectedAccountId])
     VALUES
           (@pOutId,@pUserId,@pAccountHolderName,@pAccountNumber,@pBankId,@pBranchCode,@pIBAN,@pConnectedAccountId)
	
   end

	else
	begin



  UPDATE dbo.tblBankAccount  
   SET 
	   AccountHolderName = @pAccountHolderName,
	   AccountNumber = @pAccountNumber, 
	   BankId = @pBankId,
	   BranchCode =@pBranchCode,
	   IBAN = @pIBAN,
	   ConnectedAccountId = @pConnectedAccountId

  WHERE [BankAccountId]=@pBankAccountId  
  
  
  SET @pOutId = @pBankAccountId   

  END  
 

END