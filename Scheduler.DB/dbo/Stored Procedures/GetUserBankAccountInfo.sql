-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
---- =============================================  
  
CREATE PROCEDURE [dbo].[GetUserBankAccountInfo]  
 -- Add the parameters for the stored procedure here  
 @pUserId uniqueidentifier  
  
  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
 
    -- Insert statements for procedure here  
 
   
  
  Select   
    BA.Id,
    BA.BankAccountId ,
	BA.UserId,
    BA.AccountHolderName,
    BA.AccountNumber, 
    BA.BankId,
	BA.BranchCode,
	BA.IBAN,
	BA.ConnectedAccountId

 From   [dbo].tblBankAccount BA   
    WHERE BA.[UserId]=@pUserId  
  
  
  
  
END