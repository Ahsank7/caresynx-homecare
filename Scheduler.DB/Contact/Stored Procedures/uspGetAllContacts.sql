-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================


--exec [Contact].[uspGetAllContacts]    @pUserId='B821366E-FE29-4884-8859-1ED6316F7558',	@pSortColumn  ='FirstName',	@pSortType ='asc' ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [Contact].[uspGetAllContacts] 	
-- Add the parameters for the stored procedure here
	@pUserId Nvarchar(50)=null,
	@pEmail nvarchar(50)=null,
	@pPhoneNumber nvarchar(50)=null,
	@pMobileNumber nvarchar(50)=null,
	@pLastName nvarchar(50)=null,
	@pFirstName nvarchar(50)=null,
	@pGenderId int=null,
	@pEthnicityId int=null,
	@pContactTypeId int=null,
	@pSortColumn nvarchar(50) = null,
	@pSortType nvarchar(10) = null ,
	@PageNumber int =1,
	@pPageSize int =10
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

		IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults
		IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results

		if isnull(@pContactTypeId,'') =''
		 set @pContactTypeId=null

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

		 if isnull(@pGenderId,0) =0
		 set @pGenderId=null
		
		 if isnull(@pEthnicityId,0) =0
		 set @pEthnicityId=null

		print @pUserId
		
		Select 
		     C.Id
		    ,U.[Id] as UserId
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
			,(select Top 1 N.Name from tblLookupItems N where N.Id=c.ContactTypeId and LookupType='ContactType') ContactType
			,U.[FranchiseId]
			,C.Notes
			,C.ContactUserId
	Into  #Results
	From  [dbo].tblUserContact C 
	JOIN  [dbo].[tblUser] U on U.Id = C.UserId
    WHERE 1=1
	 and  IsNULL(C.IsActive,0)=1
	 and C.ContactUserId=@pUserId
	 and (@pContactTypeId is Null) OR (c.ContactTypeId=@pContactTypeId)
	 and (@pEmail is Null) OR (U.Email = @pEmail)
	 and (@pFirstName is Null) OR (U.FirstName like '%'+@pFirstName+'%')
	 and (@pLastName is Null) OR (U.FirstName like '%'+@pLastName+'%')
	 and (@pPhoneNumber is Null) OR (U.PhoneNo like '%'+@pPhoneNumber+'%')
	 and (@pMobileNumber is Null) OR (U.PhoneNo like '%'+@pMobileNumber+'%')
	 and (@pGenderId is Null) OR (u.Gender =@pGenderId)
	 and (@pEthnicityId is Null) OR (u.Ethnicity =@pEthnicityId)


 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
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
CASE WHEN @pSortColumn = 'ContactType' AND @pSortType ='ASC' THEN ContactType END ,
CASE WHEN @pSortColumn = 'ContactType' AND @pSortType ='DESC' THEN ContactType END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END