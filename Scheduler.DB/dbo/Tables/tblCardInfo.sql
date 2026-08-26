CREATE TABLE [dbo].[tblCardInfo] (
    [Id]             INT              IDENTITY (1, 1) NOT NULL,
    [CardId]         UNIQUEIDENTIFIER NULL,
    [UserId]         UNIQUEIDENTIFIER NOT NULL,
    [CardHolderName] VARCHAR (100)    NOT NULL,
    [CardNumber]     NVARCHAR (500)   NOT NULL,
    [ExpiryMonth]    TINYINT          NOT NULL,
    [ExpiryYear]     SMALLINT         NOT NULL,
    [CVV]            NVARCHAR (100)   NOT NULL,
    [TypeId]         INT              NOT NULL,
    [CreatedAt]      DATETIME         CONSTRAINT [DF__tblCardIn__Creat__0A688BB1] DEFAULT (getutcdate()) NULL,
    CONSTRAINT [PK__tblCardI__3214EC071976F8A1] PRIMARY KEY CLUSTERED ([Id] ASC)
);

