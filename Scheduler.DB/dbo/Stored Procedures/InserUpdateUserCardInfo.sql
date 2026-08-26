  
  
CREATE   PROCEDURE [dbo].[InserUpdateUserCardInfo]   
 -- Add the parameters for the stored procedure here  

 @pOutId uniqueidentifier=null output,  
 @pUserId uniqueidentifier=null,  
 @pCardId  uniqueidentifier =null,          
 @pCVV nvarchar(100)=null,   
 @pCardHolderName nvarchar(500)=null,
 @pCardNumber nvarchar(500)=null,
 @pTypeId int=null,
 @pExpiryYear int=null,
 @pExpiryMonth int=null
  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
    -- Insert statements for procedure here  
  
 if @pCardId is null
 begin

 SET @pOutId =  NEWID()	



 INSERT INTO [dbo].[tblCardInfo]
           ([CardId]
           ,[UserId]
           ,[CardHolderName]
           ,[CardNumber]
           ,[ExpiryMonth]
           ,[ExpiryYear]
           ,[CVV]
		   ,TypeId)
     VALUES
           (@pOutId,@pUserId,@pCardHolderName,@pCardNumber,@pExpiryMonth,@pExpiryYear,@pCVV,@pTypeId)
	
   end

	else
	begin



  UPDATE dbo.tblCardInfo  
   SET 
	   CardHolderName = @pCardHolderName,
	   CardNumber = @pCardNumber, 
	   ExpiryMonth = @pExpiryMonth,
	   ExpiryYear =@pExpiryYear,
	   CVV = @pCVV,
	   TypeId =@pTypeId

  WHERE [CardId]=@pCardId  
  
  
  SET @pOutId = @pCardId   

  END
  

END