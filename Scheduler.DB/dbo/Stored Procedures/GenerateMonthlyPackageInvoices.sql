CREATE PROCEDURE [dbo].[GenerateMonthlyPackageInvoices]
    @pBillingMonth INT, -- 1-12
    @pBillingYear INT,
    @pOrganizationId UNIQUEIDENTIFIER = NULL -- NULL = all organizations
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @vBillingPeriodStart DATETIME2
    DECLARE @vBillingPeriodEnd DATETIME2
    DECLARE @vInvoiceNumber NVARCHAR(50)
    DECLARE @vBillableUserCount INT
    DECLARE @vSubTotal DECIMAL(18, 2)
    DECLARE @vTaxAmount DECIMAL(18, 2)
    DECLARE @vTotalAmount DECIMAL(18, 2)
    DECLARE @vTaxPercentage DECIMAL(18, 2)
    DECLARE @vOrganizationPackageId UNIQUEIDENTIFIER
    DECLARE @vOrganizationId UNIQUEIDENTIFIER
    DECLARE @vPerClientCharge DECIMAL(18, 2)
    DECLARE @vInitialOneTimeCost DECIMAL(18, 2)
    DECLARE @vInfrastructureCost DECIMAL(18, 2)
    DECLARE @vSupportCharges DECIMAL(18, 2)
    DECLARE @vNewFeatureReportCharges DECIMAL(18, 2)
    DECLARE @vStartDate DATETIME2
    
    -- Calculate billing period (first day to last day of month)
    SET @vBillingPeriodStart = DATEFROMPARTS(@pBillingYear, @pBillingMonth, 1)
    SET @vBillingPeriodEnd = EOMONTH(@vBillingPeriodStart)
    
    -- Cursor to iterate through active organization packages
    DECLARE org_package_cursor CURSOR FOR
    SELECT 
        op.Id AS OrganizationPackageId,
        op.OrganizationId,
        op.PerClientCharge,
        op.InitialOneTimeCost,
        op.InfrastructureCost,
        op.SupportCharges,
        op.NewFeatureReportCharges,
        op.StartDate,
        ISNULL(org.TaxPercentage, 0) AS TaxPercentage
    FROM [dbo].[tblOrganizationPackage] op
    INNER JOIN [dbo].[tblOrganization] org ON org.Id = op.OrganizationId
    WHERE op.IsActive = 1
      AND op.EndDate IS NULL
      AND (@pOrganizationId IS NULL OR op.OrganizationId = @pOrganizationId)
      AND op.StartDate <= @vBillingPeriodEnd
      -- Check if invoice already exists for this period
      AND NOT EXISTS (
          SELECT 1 FROM [dbo].[tblPackageInvoice] pi
          WHERE pi.OrganizationId = op.OrganizationId
            AND pi.BillingPeriodStart = @vBillingPeriodStart
            AND pi.BillingPeriodEnd = @vBillingPeriodEnd
      );
    
    OPEN org_package_cursor;
    FETCH NEXT FROM org_package_cursor INTO 
        @vOrganizationPackageId, @vOrganizationId, @vPerClientCharge, 
        @vInitialOneTimeCost, @vInfrastructureCost, @vSupportCharges, 
        @vNewFeatureReportCharges, @vStartDate, @vTaxPercentage;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- Billable seat count: distinct active users (no UserType filter) linked to this org via franchises
        SELECT @vBillableUserCount = COUNT(DISTINCT u.Id)
        FROM [dbo].[tblUser] u
        INNER JOIN [dbo].[tbUserFranchise] uf ON uf.UserId = u.Id
        INNER JOIN [dbo].[tblFranchise] f ON f.Id = uf.FranchiseId
        WHERE f.OrganizationId = @vOrganizationId
          AND u.IsActive = 1;
        
        -- Determine if this is initial charge (first month after package assignment)
        DECLARE @vIsInitialCharge BIT = 0
        IF YEAR(@vStartDate) = @pBillingYear AND MONTH(@vStartDate) = @pBillingMonth
        BEGIN
            SET @vIsInitialCharge = 1
        END
        
        -- Calculate subtotal
        SET @vSubTotal = 
            (@vPerClientCharge * @vBillableUserCount) +
            (CASE WHEN @vIsInitialCharge = 1 THEN @vInitialOneTimeCost ELSE 0 END) +
            @vInfrastructureCost +
            @vSupportCharges +
            @vNewFeatureReportCharges
        
        -- Calculate tax
        SET @vTaxAmount = @vSubTotal * (@vTaxPercentage / 100.0)
        
        -- Calculate total
        SET @vTotalAmount = @vSubTotal + @vTaxAmount
        
        -- Generate invoice number
        SET @vInvoiceNumber = 'PKG-' + CAST(@pBillingYear AS VARCHAR(4)) + 
                             '-' + RIGHT('0' + CAST(@pBillingMonth AS VARCHAR(2)), 2) + 
                             '-' + SUBSTRING(CAST(@vOrganizationId AS VARCHAR(36)), 1, 8)
        
        -- Insert invoice
        INSERT INTO [dbo].[tblPackageInvoice]
            (OrganizationId, OrganizationPackageId, InvoiceDate, BillingPeriodStart, BillingPeriodEnd,
             PerClientCharge, ClientCount, InitialOneTimeCost, InfrastructureCost, 
             SupportCharges, NewFeatureReportCharges, SubTotal, TaxAmount, TotalAmount,
             IsInitialCharge, PaymentStatus, InvoiceNumber)
        VALUES
            (@vOrganizationId, @vOrganizationPackageId, SYSUTCDATETIME(), 
             @vBillingPeriodStart, @vBillingPeriodEnd,
             @vPerClientCharge, @vBillableUserCount, 
             CASE WHEN @vIsInitialCharge = 1 THEN @vInitialOneTimeCost ELSE 0 END,
             @vInfrastructureCost, @vSupportCharges, @vNewFeatureReportCharges,
             @vSubTotal, @vTaxAmount, @vTotalAmount,
             @vIsInitialCharge, 'Pending', @vInvoiceNumber);
        
        FETCH NEXT FROM org_package_cursor INTO 
            @vOrganizationPackageId, @vOrganizationId, @vPerClientCharge, 
            @vInitialOneTimeCost, @vInfrastructureCost, @vSupportCharges, 
            @vNewFeatureReportCharges, @vStartDate, @vTaxPercentage;
    END
    
    CLOSE org_package_cursor;
    DEALLOCATE org_package_cursor;
    
    -- Return count of generated invoices
    SELECT COUNT(*) AS GeneratedInvoiceCount
    FROM [dbo].[tblPackageInvoice]
    WHERE BillingPeriodStart = @vBillingPeriodStart
      AND BillingPeriodEnd = @vBillingPeriodEnd;
END