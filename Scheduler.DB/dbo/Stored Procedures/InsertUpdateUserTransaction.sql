

CREATE   PROCEDURE [dbo].[InsertUpdateUserTransaction] 
	-- Add the parameters for the stored procedure here
	@pTransactionId uniqueidentifier=null,
	@pOutId uniqueidentifier=null output,
	@pUserId uniqueidentifier=null,
	@pTypeId  int =null, 
	@pStatusId int=null,         
	@pCardId uniqueidentifier=null, 
	@pBankAccountId uniqueidentifier=null, 
	@pReferenceId nvarchar(500)=null,
	@pRemarks nvarchar(500)=null


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

	print @pTransactionId

--	if @pTransactionId is null
--	begin
--	print 'i am in if'

--SET @pOutId =  NEWID()	
	

-----------------------------------------------------------Transaction-----------------------------------------------


  INSERT INTO dbo.tblTransaction
           (TransactionId
		   ,UserId
		   ,[TypeId]
           ,[StatusId]
           ,[CardId]
           ,[BankAccountId]
           ,[Remarks]
		   ,[ReferenceId])
     VALUES
           (@pTransactionId,
		    @pUserId,
		    @pTypeId,
            @pStatusId,
            @pCardId,
            @pBankAccountId,
            @pRemarks, 
			@pReferenceId)

	--end

	--else
	--begin
	--print 'i am in else'
	--	UPDATE dbo.tblTransaction
	--	  SET   TypeId    =  @pTypeId 
	--	       ,StatusId         = @pStatusId   
	--		   ,Remarks	= @pRemarks
	--		   ,ReferenceId	= @pReferenceId

	--	WHERE [TransactionId]=@pTransactionId


	--	SET @pOutId = @pTransactionId	

	--end

END