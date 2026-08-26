-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
---- =============================================  
  
CREATE PROCEDURE [dbo].[GetServiceProviderContractInfo]  
 -- Add the parameters for the stored procedure here  
 @pUserId uniqueidentifier  
  
  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
    -- Insert statements for procedure here  
  
   
  
  Select  
       [Id]
      ,[ContractType]
      ,[StartDate]
      ,[EndDate]
      ,[OptionId]

      ,[Rate]
      ,[FrequencyId]
      ,[ServiceProviderUserId]
      ,[isActive]

 From   [dbo].[tblServiceProviderContract] BA   
    WHERE BA.[ServiceProviderUserId]=@pUserId  
	  and isActive = 1
  
  
  
  
END