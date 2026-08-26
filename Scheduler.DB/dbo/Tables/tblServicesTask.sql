CREATE TABLE [dbo].[tblServicesTask] (
    [Id]                INT              IDENTITY (1, 1) NOT NULL,
    [ScheduleId]        INT              NULL,
    [EndTime]           DATETIME         NULL,
    [Date]              DATE             NULL,
    [StartTime]         DATETIME         NULL,
    [ClientId]          UNIQUEIDENTIFIER NULL,
    [ServiceProviderId] UNIQUEIDENTIFIER NULL,
    [UpdatedBy]         UNIQUEIDENTIFIER NULL,
    [CreatedDate]       DATETIME         NULL,
    [CreatedBy]         UNIQUEIDENTIFIER NULL,
    [UpdatedDate]       DATETIME         NULL,
    [Status]            INT              NULL,
    [Notes]             NVARCHAR (500)   NULL,
    [CheckIn]           DATETIME         NULL,
    [CheckOut]          DATETIME         NULL,
    [BillingAmount]     DECIMAL (18, 2)  CONSTRAINT [DF_tblServicesTask_BillingAmount] DEFAULT ((0)) NULL,
    [BillingRate]       DECIMAL (18, 2)  NULL,
    [WageAmount]        DECIMAL (18, 2)  CONSTRAINT [DF_tblServicesTask_WageAmount] DEFAULT ((0)) NULL,
    [WageRate]          DECIMAL (18, 2)  NULL,
    [CalculationTypeId] INT              NULL,
    [WageOptionId]      INT              NULL,
    [IsConfirmed]       BIT              CONSTRAINT [DF_tblServicesTask_IsConfirmed] DEFAULT ((0)) NULL,
    [ClientResponsibilityAmount] DECIMAL (18, 2) NULL CONSTRAINT [DF_tblServicesTask_ClientResp] DEFAULT ((0)),
    [PayerResponsibilityAmount]  DECIMAL (18, 2) NULL CONSTRAINT [DF_tblServicesTask_PayerResp] DEFAULT ((0)),
    [FundingPayerId]    UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_tblServicesTask] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblServicesTask_ClientId]
    ON [dbo].[tblServicesTask]([ClientId] ASC)
    INCLUDE([Id], [Date], [Status]);


GO

CREATE TRIGGER dbo.trg_tblServicesTask_TenantValidation
ON dbo.tblServicesTask
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate that ClientId and ServiceProviderId belong to the same franchise
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN tblUser client ON i.ClientId = client.Id
        INNER JOIN tblUser provider ON i.ServiceProviderId = provider.Id
        WHERE client.FranchiseId != provider.FranchiseId
    )
    BEGIN
        RAISERROR ('Tenant validation failed: Client and Service Provider must belong to the same franchise', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END
END