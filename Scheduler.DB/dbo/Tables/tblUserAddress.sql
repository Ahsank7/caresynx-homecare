CREATE TABLE [dbo].[tblUserAddress] (
    [Id]               UNIQUEIDENTIFIER NULL,
    [UserId]           UNIQUEIDENTIFIER NULL,
    [AddressLine1]     NVARCHAR (100)   NULL,
    [AddressLine2]     NVARCHAR (100)   NULL,
    [AddressLIne3]     NVARCHAR (100)   NULL,
    [AddressType]      INT              NULL,
    [CountyId]         INT              NOT NULL,
    [StateId]          INT              NOT NULL,
    [CountryId]        INT              NULL,
    [Latitude]         FLOAT (53)       NULL,
    [Longitude]        FLOAT (53)       NULL,
    [IsPrimaryAddress] BIT              NULL,
    [IsActive]         BIT              NULL
);





