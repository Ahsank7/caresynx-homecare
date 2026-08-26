-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
---- =============================================  
  
CREATE PROCEDURE [dbo].[GetUserCardInfo]  
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
    BA.CardId ,
	BA.UserId,
    BA.CardHolderName,
    BA.CardNumber, 
    BA.ExpiryMonth,
	BA.ExpiryYear,
	BA.CVV,
	BA.TypeId

 From   [dbo].tblCardInfo BA   
    WHERE BA.[UserId]=@pUserId  
  
  
  
  
END