CREATE TABLE [dbo].[tbUserFranchise] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [UserId]      UNIQUEIDENTIFIER NULL,
    [FranchiseId] UNIQUEIDENTIFIER NULL,
    [IsActive]    BIT              NULL,
    CONSTRAINT [PK_tbUserFranchise] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tbUserFranchise_UserId_FranchiseId]
    ON [dbo].[tbUserFranchise]([UserId] ASC, [FranchiseId] ASC)
    INCLUDE([IsActive]);

