-- Run once on existing databases after deploy (new installs get these via PopulateDefaultMenus).
-- Inserts org-level Payers and Organization funding menu rows for RBAC, if missing.

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblMenu] WHERE [MenuId] = N'org-payers')
BEGIN
    DECLARE @OrgId uniqueidentifier;
    SELECT TOP 1 @OrgId = [Id] FROM [dbo].[tblOrganization] ORDER BY [Id];

    INSERT INTO [dbo].[tblMenu] ([Id], [MenuId], [MenuName], [ParentMenuId], [MenuPath], [MenuIcon], [MenuOrder], [OrganizationId])
    VALUES (NEWID(), N'org-payers', N'Payers', N'organization-settings', NULL, N'IconUsers', 24, @OrgId);
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblMenu] WHERE [MenuId] = N'org-funding')
BEGIN
    DECLARE @OrgId2 uniqueidentifier;
    SELECT TOP 1 @OrgId2 = [Id] FROM [dbo].[tblOrganization] ORDER BY [Id];

    INSERT INTO [dbo].[tblMenu] ([Id], [MenuId], [MenuName], [ParentMenuId], [MenuPath], [MenuIcon], [MenuOrder], [OrganizationId])
    VALUES (NEWID(), N'org-funding', N'Organization funding', N'organization-settings', NULL, N'IconPercentage', 25, @OrgId2);
END
GO
