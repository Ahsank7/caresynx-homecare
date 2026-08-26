  
  
CREATE   PROCEDURE [dbo].[UpdateUserAuthenticationInfo]   
 -- Add the parameters for the stored procedure here  

 @pOutId uniqueidentifier=null output,  
 @pUserId uniqueidentifier=null,  
 @pRoleId  int =null,          
 @pUserName nvarchar(500)=null,   
 @pPassword nvarchar(500)=null
  
AS  
BEGIN  
 -- SET NOCOUNT ON added to prevent extra result sets from  
 -- interfering with SELECT statements.  
 SET NOCOUNT ON;  
  
    -- Insert statements for procedure here  
  

  UPDATE dbo.tblUser  
   SET [Password]  = @pPassword  
      ,[UserName]  = @pUserName  
  WHERE [Id]=@pUserId  
     AND (
            ISNULL([UserName], '') <> ISNULL(@pUserName, '')  
         OR ISNULL([Password], '') <> ISNULL(@pPassword, '')
      );

  ---- Handle role update using the new ManageUserRole procedure
  --IF @pRoleId IS NOT NULL AND @pRoleId > 0
  --BEGIN
  --    EXEC [dbo].[ManageUserRole] @pUserId, @pRoleId, @pUserId
  --END

  
  SET @pOutId = @pUserId   
  

END