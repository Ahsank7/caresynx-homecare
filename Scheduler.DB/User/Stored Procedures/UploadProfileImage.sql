-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
---- =============================================


CREATE    PROCEDURE [User].[UploadProfileImage] 
	-- Add the parameters for the stored procedure here 
	@pUserId uniqueidentifier,
	@pProfileImagePath nvarchar(500) =null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	--print @pProfileImagePath
	--print @pUserId

	--select ProfileImagePath from dbo.tblUser
	--	where Id=@pUserId

		UPDATE dbo.tblUser
		SET ProfileImagePath=@pProfileImagePath
		where Id=@pUserId

END