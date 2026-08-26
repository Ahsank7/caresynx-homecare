-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================



--exec [User].[uspGetAllUsers]    @pUserId =null,	@pUserId =null,	@pEmail =null,	@pPhoneNumber =null,	@pJoiningDate =null,	@pLastName =null,	@pFirstName =null,	@pGenderId =null,	@pEthnicityId =null,	@pStatusId =null,	@pSortColumn  =null,	@pSortType =null ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [User].[uspGetAllUsers] 	
-- Add the parameters for the stored procedure here
    @pFranchiseId uniqueidentifier,
	@pUserId Nvarchar(50)=null,
	@pUserType int =1,
	@pUserNo nvarchar(50) ='',
	@pEmail nvarchar(50)=null,
	@pPhoneNumber nvarchar(50)=null,
	@pMobileNumber nvarchar(50)=null,
	@pJoiningDate date=null,
	@pLastName nvarchar(50)=null,
	@pFirstName nvarchar(50)=null,
	@pGenderId int=null,
	@pEthnicityId int=null,
	@pStatusId int=null,
	-- varchar without length defaults to varchar(1); 'FirstName'/'ASC' were truncated so ORDER BY never matched.
	@pSortColumn nvarchar(50) = null,
	@pSortType nvarchar(10) = null,
	@PageNumber int =1,
	@pPageSize int =10,
	@pCurrentUserId uniqueidentifier = NULL  -- NEW: Current logged-in user ID for role hierarchy filtering
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Get current user's role level for hierarchy filtering (only for Staff UserType=3)
	DECLARE @vCurrentUserRoleLevel INT = NULL;
	
	IF @pUserType = 3 AND @pCurrentUserId IS NOT NULL
	BEGIN
		SELECT @vCurrentUserRoleLevel = r.RoleLevel
		FROM tblUserRole ur
		INNER JOIN tblRole r ON ur.RoleId = r.Id
		WHERE ur.UserId = @pCurrentUserId 
		AND ur.IsActive = 1
		AND r.IsActive = 1;
	END;

		IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults
		IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results

		 if isnull(@pUserId,'') =''
		 set @pUserId=null

		 if isnull(@pEmail,'') =''
		 set @pEmail=null

		 if isnull(@pPhoneNumber,'') =''
		 set @pPhoneNumber=null

		 if isnull(@pMobileNumber,'') =''
		 set @pMobileNumber=null
		 

		 if isnull(@pLastName,'') =''
		 set @pLastName=null

		  if isnull(@pFirstName,'') =''
		 set @pFirstName=null

		 if isnull(@pUserNo,'') =''
		 set @pUserNo=null

		 if isnull(@pGenderId,0) =0
		 set @pGenderId=null
		
		 if isnull(@pEthnicityId,0) =0
		 set @pEthnicityId=null

		 if isnull(@pStatusId,0) =0
		 set @pStatusId=null

		 
		
		
		Select U.[Id] as UserId
		    ,U.UserNo
			,U.[FirstName]    
			,U.[SurName]	  
			,U.[LastName]	  
			,U.[Alias]		  
			,U.[Age]		  
			,(select Top 1 G.Name from tblLookupItems G where G.LookupType='Gender' And G.Id=U.[Gender]) Gender 
			,(select Top 1 M.Name from tblLookupItems M where M.LookupType='MaritalStatus' And M.Id=U.[MaritalStatus]) MaritalStatus  
			,(select Top 1 E.Name from tblLookupItems E where E.LookupType='Ethnicity' And E.Id=U.[Ethnicity]) Ethnicity  	  
			,U.[BirthDate]	  
			,U.[JoiningDate]  
			,U.[PassportNo]	  
			,U.[IdentityNo]	  
			,U.[MobileNo]	  
			,U.[PhoneNo]	  
			,U.[Email]		  
			,U.[UpdatedDate]  
			,(select Top 1 N.Name from tblLookupItems N where N.LookupType='Nationality' And  N.Id=U.[NationalityId]) Nationality
			,U.[FranchiseId]
			,LI.[Name] as [Status]
			,'Notes' as Notes
	Into  #Results
	From  [dbo].[tblUser] U
	JOIN [dbo].tblLookupItems LI on LookupType='UserStatus' and  LI.Id=U.[Status]
	-- LEFT JOIN to get user's role (for Staff UserType=3 filtering)
	LEFT JOIN [dbo].tblUserRole UR ON U.Id = UR.UserId AND UR.IsActive = 1
	LEFT JOIN [dbo].tblRole R ON UR.RoleId = R.Id AND R.IsActive = 1
    WHERE 1=1
     and U.FranchiseId=@pFranchiseId
	 and U.UserType=@pUserType
	 and (@pUserId is Null OR U.[Id]=@pUserId)
	 and (@pEmail is Null OR U.Email = @pEmail)
	 and (@pUserNo is Null OR U.UserNo like '%'+@pUserNo+'%')
	 and (@pFirstName is Null OR U.FirstName like '%'+@pFirstName+'%')
	 and (@pLastName is Null OR U.LastName like '%'+@pLastName+'%')
	 and (@pPhoneNumber is Null OR U.PhoneNo like '%'+@pPhoneNumber+'%')
	 and (@pMobileNumber is Null OR U.MobileNo like '%'+@pMobileNumber+'%')
	 and (@pGenderId is Null OR u.Gender =@pGenderId)
	 and (@pEthnicityId is Null OR u.Ethnicity =@pEthnicityId)
	 and (@pStatusId is Null OR [Status] =@pStatusId)
	 and (@pJoiningDate is Null OR u.JoiningDate =@pJoiningDate)
	 -- Role Hierarchy Filter: Only show staff with RoleLevel >= current user's level (or no role assigned yet)
	 -- This only applies when fetching Staff (UserType=3) and current user has a role
	 and (
		@vCurrentUserRoleLevel IS NULL  -- No filtering if current user has no role or not fetching staff
		OR R.RoleLevel IS NULL  -- Include staff with no role assigned yet
		OR R.RoleLevel >= @vCurrentUserRoleLevel  -- Include staff with equal or lower authority (higher level number)
	 )


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='ASC' THEN UserId END ,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='DESC' THEN UserId END DESC,
CASE WHEN @pSortColumn = 'FirstName' AND @pSortType ='ASC' THEN FirstName END ,
CASE WHEN @pSortColumn = 'FirstName' AND @pSortType ='DESC' THEN FirstName END DESC,
CASE WHEN @pSortColumn = 'LastName' AND @pSortType ='ASC' THEN LastName END ,
CASE WHEN @pSortColumn = 'LastName' AND @pSortType ='DESC' THEN LastName END DESC,
CASE WHEN @pSortColumn = 'JoiningDate' AND @pSortType ='ASC' THEN [JoiningDate] END ,
CASE WHEN @pSortColumn = 'JoiningDate' AND @pSortType ='DESC' THEN [JoiningDate] END DESC,
CASE WHEN @pSortColumn = 'Age' AND @pSortType ='ASC' THEN Age END ,
CASE WHEN @pSortColumn = 'Age' AND @pSortType ='DESC' THEN Age END DESC,

CASE WHEN @pSortColumn = 'Gender' AND @pSortType ='ASC' THEN Gender END ,
CASE WHEN @pSortColumn = 'Gender' AND @pSortType ='DESC' THEN Gender END DESC,
CASE WHEN @pSortColumn = 'Email' AND @pSortType ='ASC' THEN Email END ,
CASE WHEN @pSortColumn = 'Email' AND @pSortType ='DESC' THEN Email END DESC,
CASE WHEN @pSortColumn = 'PhoneNo' AND @pSortType ='ASC' THEN PhoneNo END ,
CASE WHEN @pSortColumn = 'PhoneNo' AND @pSortType ='DESC' THEN PhoneNo END DESC,

CASE WHEN @pSortColumn = 'PassportNo' AND @pSortType ='ASC' THEN PassportNo END ,
CASE WHEN @pSortColumn = 'PassportNo' AND @pSortType ='DESC' THEN PassportNo END DESC,
CASE WHEN @pSortColumn = 'IdentityNo' AND @pSortType ='ASC' THEN IdentityNo END ,
CASE WHEN @pSortColumn = 'IdentityNo' AND @pSortType ='DESC' THEN IdentityNo END DESC,
CASE WHEN @pSortColumn = 'BirthDate' AND @pSortType ='ASC' THEN BirthDate END ,
CASE WHEN @pSortColumn = 'BirthDate' AND @pSortType ='DESC' THEN BirthDate END DESC,

CASE WHEN @pSortColumn = 'MobileNo' AND @pSortType ='ASC' THEN MobileNo END ,
CASE WHEN @pSortColumn = 'MobileNo' AND @pSortType ='DESC' THEN MobileNo END DESC,
CASE WHEN @pSortColumn = 'Ethnicity' AND @pSortType ='ASC' THEN Ethnicity END ,
CASE WHEN @pSortColumn = 'Ethnicity' AND @pSortType ='DESC' THEN Ethnicity END DESC,
CASE WHEN @pSortColumn = 'MaritalStatus' AND @pSortType ='ASC' THEN MaritalStatus END ,
CASE WHEN @pSortColumn = 'MaritalStatus' AND @pSortType ='DESC' THEN MaritalStatus END DESC,
CASE WHEN @pSortColumn = 'Status' AND @pSortType ='ASC' THEN [Status] END ,
CASE WHEN @pSortColumn = 'Status' AND @pSortType ='DESC' THEN [Status] END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END