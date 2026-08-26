CREATE TABLE [dbo].[tblOrganization] (
    [Id]                       UNIQUEIDENTIFIER NOT NULL,
    [Name]                     NVARCHAR (50)    NULL,
    [Description]              NVARCHAR (500)   NULL,
    [LogoPath]                 NVARCHAR (500)   NULL,
    [DefaultBillingRate]       DECIMAL (18, 2)  NULL,
    [DefaultWageRate]          DECIMAL (18, 2)  NULL,
    [CompleteAddress]          NVARCHAR (500)   NULL,
    [ContactNo]                NVARCHAR (50)    NULL,
    [Email]                    NVARCHAR (50)    NULL,
    [WebSite]                  NVARCHAR (100)   NULL,
    [CurrencyId]               INT              NULL,
    [CalculationTypeId]        INT              CONSTRAINT [DF_tblOrganization_CalculationTypeId] DEFAULT ((1)) NULL,
    [TaxPercentage]            DECIMAL (18, 2)  NULL,
    [DiscountPercentage]       DECIMAL (18, 2)  NULL,
    [IsActive]                 BIT              NULL,
    [CurrencySignId]           INT              NULL,
    [UseServiceRateForBilling] BIT              CONSTRAINT [DF_tblOrganization_UseServiceRateForBilling] DEFAULT ((0)) NULL,
    [TimeZone]                 NVARCHAR (100)   CONSTRAINT [DF_tblOrganization_TimeZone] DEFAULT ('Pakistan Standard Time') NULL,
    [ServiceRateForBilling]    INT              NULL,
    CONSTRAINT [PK_tblOrganization_1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

