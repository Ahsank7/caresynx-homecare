-- =============================================

-- Author:		<Author,,Name>

-- Create date:	<Create Date,,>

-- Description:	Billing invoice list; includes resolved bill-to name, email, address, and phone for PDF/UI.

--              tblUserContact: UserId = guarantor; ContactUserId = client (care recipient).

-- =============================================

CREATE PROCEDURE [dbo].[uspGetBillingInfo] 

	@pFranchiseId uniqueidentifier,

	@pUserId nvarchar(100)=null,

	@pDate date=null,

	@pTransactionId nvarchar(50)=null,

	@pUserNo nvarchar(20)=null,

	@pSortColumn nvarchar(50) = null,

	@pSortType nvarchar(10) = null ,

	@PageNumber int =1,

	@pPageSize int =10

AS

BEGIN

	SET NOCOUNT ON;



		IF OBJECT_ID('tempdb..#FinalResults') IS NOT NULL DROP TABLE #FinalResults

		IF OBJECT_ID('tempdb..#Results') IS NOT NULL DROP TABLE #Results



		if isnull(@pUserId,'') =''

		 set @pUserId=null



		if isnull(@pTransactionId,'') =''

		 set @pTransactionId=null



		if isnull(@pUserNo,'') =''

		 set @pUserNo=null



		if isnull(@pDate,'') =''

		 set @pDate=null



select BI.Id,

       Details,

	   TotalAmount,

	   Date,

	   StartDate,

	   EndDate,

	   DueDate,

	   IsPaid,

	   ClientId,

	   u.UserNo,

	   (u.FirstName +' '+ u.LastName) ClientName,

	   AmountAfterTax,

	   TaxPercentage,

	   AmountAfterDiscount,

	   DiscountPercentage,

	   BI.Row_Guid as TransactionId,

	   BI.BillToType,

	   BI.BillToPayerId,

	   BI.BillToUserContactId,

	   COALESCE(

		 CASE

		   WHEN (BI.BillToType IS NULL OR BI.BillToType = 1) THEN

			 LTRIM(RTRIM(CONCAT(ISNULL(u.FirstName,N''),N' ',ISNULL(u.SurName,N''),N' ',ISNULL(u.LastName,N''))))

		   WHEN BI.BillToType = 2 AND pay.LegalName IS NOT NULL THEN pay.LegalName

		   WHEN BI.BillToType = 3 AND ginfo.GFirst IS NOT NULL THEN

			 LTRIM(RTRIM(CONCAT(ISNULL(ginfo.GFirst,N''),N' ',ISNULL(ginfo.GSur,N''),N' ',ISNULL(ginfo.GLast,N''))))

		   ELSE NULL

		 END,

		 BI.BillToDisplayName

	   ) AS BillToDisplayName,

	   COALESCE(

		 CASE

		   WHEN (BI.BillToType IS NULL OR BI.BillToType = 1) THEN u.Email

		   WHEN BI.BillToType = 2 THEN pay.BillingEmail

		   WHEN BI.BillToType = 3 THEN ginfo.GEmail

		   ELSE NULL

		 END,

		 BI.DebtorEmail

	   ) AS DebtorEmail,

	   CASE

		 WHEN (BI.BillToType IS NULL OR BI.BillToType = 1) THEN ca.UAddr

		 WHEN BI.BillToType = 2 AND pay.LegalName IS NOT NULL THEN

		   LTRIM(RTRIM(CONCAT(

			 ISNULL(pay.BillingAddressLine1,N''),N' ',

			 ISNULL(pay.BillingAddressLine2,N''),N' ',

			 ISNULL(pay.BillingAddressLine3,N''))))

		 WHEN BI.BillToType = 3 THEN ga.GAddr

		 ELSE NULL

	   END AS BillToAddress,

	   CASE

		 WHEN (BI.BillToType IS NULL OR BI.BillToType = 1) THEN

		   NULLIF(LTRIM(RTRIM(COALESCE(u.MobileNo, u.PhoneNo, N''))),N'')

		 WHEN BI.BillToType = 2 THEN NULL

		 WHEN BI.BillToType = 3 AND ginfo.GuarantorUserId IS NOT NULL THEN

		   NULLIF(LTRIM(RTRIM(COALESCE(ginfo.GMobile, ginfo.GPhone, N''))),N'')

		 ELSE NULL

	   END AS BillToPhone

 Into  #Results

 from [dbo].[tblBillingInvoice] (Nolock) BI

	JOIN [dbo].[tblUser] (Nolock) u on u.Id=BI.ClientId

	OUTER APPLY (

		SELECT p.LegalName, p.BillingEmail, p.BillingAddressLine1, p.BillingAddressLine2, p.BillingAddressLine3

		FROM [dbo].[tblPayer] p WITH (NOLOCK)

		WHERE BI.BillToType = 2 AND p.Id = BI.BillToPayerId

	) pay

	OUTER APPLY (

		SELECT

			g.FirstName AS GFirst,

			g.SurName AS GSur,

			g.LastName AS GLast,

			g.Email AS GEmail,

			g.PhoneNo AS GPhone,

			g.MobileNo AS GMobile,

			tuc.UserId AS GuarantorUserId

		FROM [dbo].[tblUserContact] tuc WITH (NOLOCK)

		INNER JOIN [dbo].[tblUser] g WITH (NOLOCK) ON g.Id = tuc.UserId

		WHERE BI.BillToType = 3

		  AND tuc.Id = BI.BillToUserContactId

		  AND tuc.ContactUserId = BI.ClientId

		  AND ISNULL(tuc.IsActive,0) = 1

	) ginfo

	OUTER APPLY (

		SELECT TOP 1 LTRIM(RTRIM(CONCAT(

			ISNULL(ua.AddressLine1,N''),N' ',

			ISNULL(ua.AddressLine2,N''),N' ',

			ISNULL(ua.AddressLIne3,N'')))) AS UAddr

		FROM [dbo].[tblUserAddress] ua WITH (NOLOCK)

		WHERE (BI.BillToType IS NULL OR BI.BillToType = 1)

		  AND ua.UserId = u.Id

		  AND ISNULL(ua.IsActive,0) = 1

		ORDER BY CASE WHEN ua.IsPrimaryAddress = 1 THEN 0 ELSE 1 END

	) ca

	OUTER APPLY (

		SELECT TOP 1 LTRIM(RTRIM(CONCAT(

			ISNULL(ua.AddressLine1,N''),N' ',

			ISNULL(ua.AddressLine2,N''),N' ',

			ISNULL(ua.AddressLIne3,N'')))) AS GAddr

		FROM [dbo].[tblUserAddress] ua WITH (NOLOCK)

		WHERE BI.BillToType = 3

		  AND ginfo.GuarantorUserId IS NOT NULL

		  AND ua.UserId = ginfo.GuarantorUserId

		  AND ISNULL(ua.IsActive,0) = 1

		ORDER BY CASE WHEN ua.IsPrimaryAddress = 1 THEN 0 ELSE 1 END

	) ga

 Where 1=1

 and u.FranchiseId=@pFranchiseId

 and (@pUserNo is null OR u.UserNo=@pUserNo)

 and (@pTransactionId is null OR BI.TransactionId=@pTransactionId)

 and (@pDate is null OR BI.[Date]=@pDate)

 and ((@pUserId is Null) OR (BI.ClientId = @pUserId))





 Select * 

    into #FinalResults 

 From #Results



ORDER BY 

CASE WHEN @pSortColumn = 'Id' AND @pSortType ='ASC' THEN Id END ,

CASE WHEN @pSortColumn = 'Id' AND @pSortType ='DESC' THEN Id END DESC,

CASE WHEN @pSortColumn = 'TotalAmount' AND @pSortType ='ASC' THEN TotalAmount END ,

CASE WHEN @pSortColumn = 'TotalAmount' AND @pSortType ='DESC' THEN TotalAmount END DESC,

CASE WHEN @pSortColumn = 'Date' AND @pSortType ='ASC' THEN Date END ,

CASE WHEN @pSortColumn = 'Date' AND @pSortType ='DESC' THEN Date END DESC,

CASE WHEN @pSortColumn = 'IsPaid' AND @pSortType ='ASC' THEN IsPaid END ,

CASE WHEN @pSortColumn = 'IsPaid' AND @pSortType ='DESC' THEN IsPaid END DESC,

CASE WHEN @pSortColumn = 'UserNo' AND @pSortType ='ASC' THEN UserNo END ,

CASE WHEN @pSortColumn = 'UserNo' AND @pSortType ='DESC' THEN UserNo END DESC,

CASE WHEN @pSortColumn = 'ClientName' AND @pSortType ='ASC' THEN ClientName END ,

CASE WHEN @pSortColumn = 'ClientName' AND @pSortType ='DESC' THEN ClientName END DESC





OFFSET (@PageNumber-1)*@pPageSize ROWS

FETCH NEXT @pPageSize ROWS ONLY



select * from #FinalResults

select count(*) TotalRecords from #Results



END


