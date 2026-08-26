-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================

--select * from tblUserAddress

--exec [dbo].[uspGetAllUserAddress]   	@pSortColumn  ='AddressLine1',	@pSortType ='asc' ,	@PageNumber  =1,	@pPageSize  =10

CREATE PROCEDURE [dbo].[uspGetAllUserAddress] 	
-- Add the parameters for the stored procedure here
	@pUserId Nvarchar(50)=null,
	@pAddressTypeId int=null,
	@pAddress nvarchar =null,
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

		if isnull(@pAddressTypeId,0) =0
		 set @pAddressTypeId=null

		 if isnull(@pUserId,'') =''
		 set @pUserId=null

		 if isnull(@pAddress,'') =''
		 set @pAddress=null

		 
		
		
		Select 
		   
			 UA.Id
			,UA.UserId
			,UA.AddressLine1
			,UA.AddressLine2
			,UA.AddressLIne3
			,(select Top 1 N.Name from tblLookupItems N where N.Id=UA.AddressType and N.LookupType ='AddressType') AddressType --1 is address type lookup
			,(select Top 1 N.Name from tblLookupItems N where N.Id=UA.CountyId and N.LookupType ='County') County -- 4 is county lookup
			,(select Top 1 N.Name from tblLookupItems N where N.Id=UA.StateId and LookupType ='State') [State] --17 is state lookup
			,(select Top 1 N.Name from tblLookupItems N where N.Id=UA.CountryId and N.LookupType ='Country') Country -- 3 is country lookup
			,UA.Latitude
			,UA.Longitude
			,UA.IsPrimaryAddress
			,UA.IsActive
	Into  #Results
	From tblUserAddress UA
    WHERE 1=1
	 and IsNull(UA.IsActive,0)=1
	 and (@pUserId is Null OR UA.[UserId]=@pUserId)
	 and (@pAddressTypeId is Null OR UA.AddressType =@pAddressTypeId)
	 and (@pAddress is Null OR  (UA.AddressLine1 like '%'+@pAddress+'%'OR UA.AddressLine2 like '%'+@pAddress+'%' OR UA.AddressLIne3 like '%'+@pAddress+'%')) 

 Select * 
    into #FinalResults 
 From #Results

ORDER BY 
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,
CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='ASC' THEN UserId END ,
CASE WHEN @pSortColumn = 'UserId' AND @pSortType ='DESC' THEN UserId END DESC,
CASE WHEN @pSortColumn = 'AddressLine1' AND @pSortType ='ASC' THEN AddressLine1 END ,
CASE WHEN @pSortColumn = 'AddressLine1' AND @pSortType ='DESC' THEN AddressLine1 END DESC,
CASE WHEN @pSortColumn = 'AddressLine2' AND @pSortType ='ASC' THEN AddressLine2 END ,
CASE WHEN @pSortColumn = 'AddressLine2' AND @pSortType ='DESC' THEN AddressLine2 END DESC,
CASE WHEN @pSortColumn = 'AddressLine3' AND @pSortType ='ASC' THEN AddressLine3 END ,
CASE WHEN @pSortColumn = 'AddressLine3' AND @pSortType ='DESC' THEN AddressLine3 END DESC,
CASE WHEN @pSortColumn = 'AddressType' AND @pSortType ='ASC' THEN AddressType END ,
CASE WHEN @pSortColumn = 'AddressType' AND @pSortType ='DESC' THEN AddressType END DESC,

CASE WHEN @pSortColumn = 'County' AND @pSortType ='ASC' THEN County END ,
CASE WHEN @pSortColumn = 'County' AND @pSortType ='DESC' THEN County END DESC,
CASE WHEN @pSortColumn = 'State' AND @pSortType ='ASC' THEN [State] END ,
CASE WHEN @pSortColumn = 'State' AND @pSortType ='DESC' THEN [State] END DESC,
CASE WHEN @pSortColumn = 'Country' AND @pSortType ='ASC' THEN Country END ,
CASE WHEN @pSortColumn = 'Country' AND @pSortType ='DESC' THEN Country END DESC,

CASE WHEN @pSortColumn = 'Latitude' AND @pSortType ='ASC' THEN Latitude END ,
CASE WHEN @pSortColumn = 'Latitude' AND @pSortType ='DESC' THEN Latitude END DESC,
CASE WHEN @pSortColumn = 'Longitude' AND @pSortType ='ASC' THEN Longitude END ,
CASE WHEN @pSortColumn = 'Longitude' AND @pSortType ='DESC' THEN Longitude END DESC


OFFSET (@PageNumber-1)*@pPageSize ROWS
FETCH NEXT @pPageSize ROWS ONLY

select * from #FinalResults
select count(*) TotalRecords from #FinalResults

END