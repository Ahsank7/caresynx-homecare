-- =============================================  
-- Author:  <Author,,Name>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
---- =============================================  
  
CREATE PROCEDURE [dbo].[GetUserCreadentialsInfo]  
 -- Add the parameters for the stored procedure here  
 @pUserId uniqueidentifier  
  
  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
    -- Insert statements for procedure here  
  
   
  
  Select   
    UA.Id as UserId  
   ,UA.UserName  
   ,UA.Password  
   ,(SELECT TOP 1 RoleId FROM [dbo].[tblUserRole] WHERE  UserId=UA.Id and  IsActive=1) AS RoleId 
 From   [dbo].tblUser UA  
    WHERE UA.[Id]=@pUserId  
  
  
  
  
END