CREATE TABLE [dbo].[tblComplaint] (
    [Id]                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [ComplainantId]         UNIQUEIDENTIFIER NOT NULL,
    [ComplainantType]       INT              NOT NULL,  -- 1=Client, 2=ServiceProvider, 3=Staff
    [ComplainedAgainstId]   UNIQUEIDENTIFIER NOT NULL,
    [ComplainedAgainstType] INT              NOT NULL,  -- 1=Client, 2=ServiceProvider, 3=Staff
    [FranchiseId]           UNIQUEIDENTIFIER NULL,
    [Title]                 NVARCHAR (200)   NOT NULL,
    [Description]           NVARCHAR (2000)  NOT NULL,
    [Category]              INT              NULL,      -- Lookup: ComplaintCategory
    [Severity]              INT              NULL,      -- Lookup: ComplaintSeverity (Low, Medium, High, Critical)
    [Status]                INT              NOT NULL DEFAULT 1,  -- Lookup: ComplaintStatus (Submitted, UnderReview, InProgress, Resolved, Closed, Rejected)
    [Resolution]            NVARCHAR (2000)  NULL,
    [ResolutionDate]        DATETIME         NULL,
    [ResolvedBy]            UNIQUEIDENTIFIER NULL,
    [CreatedDate]           DATETIME         NOT NULL DEFAULT GETDATE(),
    [UpdatedDate]           DATETIME         NULL,
    [CreatedBy]             UNIQUEIDENTIFIER NULL,
    [UpdatedBy]             UNIQUEIDENTIFIER NULL,
    [IsActive]              BIT              NOT NULL DEFAULT 1,
    CONSTRAINT [PK_tblComplaint] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_tblComplaint_Complainant] FOREIGN KEY ([ComplainantId]) REFERENCES [dbo].[tblUser]([Id]),
    CONSTRAINT [FK_tblComplaint_ComplainedAgainst] FOREIGN KEY ([ComplainedAgainstId]) REFERENCES [dbo].[tblUser]([Id]),
    CONSTRAINT [FK_tblComplaint_Franchise] FOREIGN KEY ([FranchiseId]) REFERENCES [dbo].[tblFranchise]([Id]),
    CONSTRAINT [FK_tblComplaint_ResolvedBy] FOREIGN KEY ([ResolvedBy]) REFERENCES [dbo].[tblUser]([Id])
);

