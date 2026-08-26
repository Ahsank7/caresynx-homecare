CREATE   PROCEDURE [dbo].[PopulateDefaultMenus]
  @OrganizationId uniqueIdentifier
AS
BEGIN
    SET NOCOUNT ON;

    -- Clear existing menu data
    DELETE FROM tblMenu;

    -- Organization Level Menus
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, OrganizationId) VALUES
    (NEWID(), 'organizations', 'Organizations', NULL, '/organizations', 'IconSitemap', 1, @OrganizationId),
    (NEWID(), 'organization-settings', 'Organization Settings', 'organizations', '/organizations/:id/organization-settings', 'IconSettings', 2, @OrganizationId);

    -- Franchise Level Menus
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder ,OrganizationId) VALUES
    (NEWID(), 'franchises', 'Franchises', NULL, '/franchises', 'IconBuilding', 3, @OrganizationId),
    (NEWID(), 'franchise-dashboard', 'Dashboard', 'franchises', '/franchises/:name/dashboard', 'IconDashboard', 4, @OrganizationId),
    (NEWID(), 'planboard', 'Planboard', 'franchises', '/franchises/:name/planboard', 'IconClipboard', 5, @OrganizationId),
    (NEWID(), 'to-confirm', 'To Confirm', 'franchises', '/franchises/:name/toConfirm', 'IconChecklist', 6, @OrganizationId);

    -- Profile Management
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, OrganizationId) VALUES
    (NEWID(), 'profile', 'Profile', 'franchises', NULL, 'IconUserCheck', 7, @OrganizationId),
    (NEWID(), 'staffs', 'Staffs', 'profile', '/franchises/:name/profile/staffs', 'IconUsers', 8, @OrganizationId);

    -- Billing & Payroll
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, OrganizationId) VALUES
    (NEWID(), 'billing', 'Billing', 'franchises', NULL, 'IconReceipt', 9, @OrganizationId),
    (NEWID(), 'billing-details', 'Billing Details', 'billing', '/franchises/:name/billingDetails', 'IconReceipt', 10, @OrganizationId),
    (NEWID(), 'payroll', 'Payroll', 'franchises', NULL, 'IconCashBanknote', 11, @OrganizationId),
    (NEWID(), 'wage-details', 'Wage Details', 'payroll', '/franchises/:name/wageDetails', 'IconCashBanknote', 12, @OrganizationId);

    -- Transactions
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, OrganizationId) VALUES
    (NEWID(), 'transactions', 'Transactions', 'franchises', NULL, 'IconExchange', 13, @OrganizationId),
    (NEWID(), 'transaction-details', 'Transaction Details', 'transactions', '/franchises/:name/transactionDetails', 'IconExchange', 14, @OrganizationId);

    -- Reports & AI Copilot
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, OrganizationId) VALUES
    (NEWID(), 'reports', 'Reports', 'franchises', NULL, 'IconChartBar', 15, @OrganizationId),
    (NEWID(), 'ai-copilot', 'AI Copilot', 'franchises', '/franchises/:name/copilot', 'IconMessageCircle2', 16, @OrganizationId);

    -- Organization Settings Menus
    INSERT INTO tblMenu (Id, MenuId, MenuName, ParentMenuId, MenuPath, MenuIcon, MenuOrder, OrganizationId) VALUES
    (NEWID(), 'basic-settings', 'Basic Settings', 'organization-settings', NULL, 'IconSettings', 17, @OrganizationId),
    (NEWID(), 'lookups', 'Lookups', 'organization-settings', NULL, 'IconList', 18, @OrganizationId),
    (NEWID(), 'documents', 'Documents', 'organization-settings', NULL, 'IconFile', 19, @OrganizationId),
    (NEWID(), 'services', 'Services', 'organization-settings', NULL, 'IconTools', 20, @OrganizationId),
    (NEWID(), 'access-control', 'Access Control', 'organization-settings', NULL, 'IconShield', 21, @OrganizationId),
    (NEWID(), 'rates-billing', 'Rates & Billing', 'organization-settings', NULL, 'IconReceipt', 22, @OrganizationId),
    (NEWID(), 'login-history', 'Login History', 'organization-settings', NULL, 'IconClock', 23, @OrganizationId),
    (NEWID(), 'org-payers', 'Payers', 'organization-settings', NULL, 'IconUsers', 24, @OrganizationId),
    (NEWID(), 'org-funding', 'Organization funding', 'organization-settings', NULL, 'IconPercentage', 25, @OrganizationId);

    PRINT 'Default staff menus populated successfully!';
END