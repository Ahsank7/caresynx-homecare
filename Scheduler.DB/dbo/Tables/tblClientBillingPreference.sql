-- Default "send invoice to" for the client's share of charges (self, org payer, or contact guarantor).
CREATE TABLE [dbo].[tblClientBillingPreference] (
    [ClientId]         UNIQUEIDENTIFIER NOT NULL,
    [BillToType]       TINYINT          NOT NULL,
    [PayerId]          UNIQUEIDENTIFIER NULL,
    [UserContactId]   UNIQUEIDENTIFIER NULL,
    [UpdatedDate]      DATETIME2 (7)    NULL,
    [UpdatedBy]        UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblClientBillingPreference] PRIMARY KEY CLUSTERED ([ClientId] ASC)
);

-- BillToType: 1 = Client (self), 2 = Organization payer, 3 = Client contact (guarantor)
-- UserContactId references tblUserContact.Id (relationship enforced in application; column is not FK due to legacy nullable Id on contacts).
GO

ALTER TABLE [dbo].[tblClientBillingPreference] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientBillingPreference_tblUser] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[tblUser] ([Id]);

GO

ALTER TABLE [dbo].[tblClientBillingPreference] WITH NOCHECK
    ADD CONSTRAINT [FK_tblClientBillingPreference_tblPayer] FOREIGN KEY ([PayerId]) REFERENCES [dbo].[tblPayer] ([Id]);
