CREATE TABLE [dbo].[tblUserContact] (
    [Id]            UNIQUEIDENTIFIER NULL,
    [UserId]        UNIQUEIDENTIFIER NULL,
    [ContactUserId] UNIQUEIDENTIFIER NULL,
    [ContactTypeId] INT              NULL,
    [IsActive]      BIT              NULL,
    [Notes]         NVARCHAR (500)   NULL,
    [IsBillingContact] BIT            NULL CONSTRAINT [DF_tblUserContact_IsBillingContact] DEFAULT ((0))
);





