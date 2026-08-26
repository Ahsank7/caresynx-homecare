CREATE TABLE [dbo].[tblServiceProviderContract] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL,
    [ContractType]          INT              NULL,
    [StartDate]             DATE             NULL,
    [EndDate]               DATE             NULL,
    [OptionId]              INT              NULL,
    [Rate]                  DECIMAL (18)     NULL,
    [FrequencyId]           INT              NULL,
    [ServiceProviderUserId] UNIQUEIDENTIFIER NULL,
    [isActive]              BIT              NULL,
    [CreatedAt]             DATETIME         NULL,
    [CreatedById]           UNIQUEIDENTIFIER NULL,
    [UpdatedAt]             DATETIME         NULL,
    [UpdatedById]           UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__tblServi__3214EC074767FE48] PRIMARY KEY CLUSTERED ([Id] ASC)
);





