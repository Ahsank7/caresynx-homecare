CREATE PROCEDURE [payment].[uspGetPaymentData]
    @PaymentType VARCHAR(10)
AS
BEGIN
    IF @PaymentType = 'WAGE'
    BEGIN
        SELECT 
            spw.Id as Id,
			spw.Row_Guid,
            TotalAmount as Amount,
            ServiceProviderId as UserId,
            u.Email as Email,
            ba.AccountNumber as BankAccountId,
            ba.AccountHolderName,
            ba.AccountNumber,
            ba.BankId as BankName,
            ba.BranchCode,
            ba.IBAN,
            ba.ConnectedAccountId,
            ci.CardHolderName,
            ci.CardNumber,
            ci.CVV,
            ci.ExpiryMonth,
            ci.ExpiryYear,
            org.CurrencyId,
			ISNULL((select [Name] from tblLookupItems li where li.LookupType='currency' and li.Id=org.CurrencyId),'usd') as CurrencyCode,
			ISNULL((select [Name] from tblLookupItems li where li.LookupType='currencySign' and li.Id=org.CurrencyId),'$') as CurrencySign,
            CAST(NULL AS TINYINT) AS BillToType,
            CAST(NULL AS UNIQUEIDENTIFIER) AS BillToPayerId,
            CAST('ClientCard' AS VARCHAR(20)) AS ChargeSource
        FROM tblServiceProviderWage spw 
        LEFT JOIN tblBankAccount ba on spw.ServiceProviderId = ba.UserId
        LEFT JOIN tblCardInfo ci on spw.ServiceProviderId=ci.UserId
        LEFT JOIN dbo.tblUser u on spw.ServiceProviderId = u.Id
        LEFT JOIN dbo.tbUserFranchise uf on spw.ServiceProviderId = uf.UserId
        LEFT JOIN dbo.tblFranchise f on uf.FranchiseId = f.Id
        LEFT JOIN dbo.tblOrganization org on f.OrganizationId = org.Id
        WHERE ISNULL(IsPaid,0)=0
    END
    ELSE IF @PaymentType = 'INVOICE'
    BEGIN
        SELECT 
            spw.Id as Id,
			spw.Row_Guid,
            spw.AmountAfterTax as Amount,
            spw.ClientId as UserId,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN pay.BillingEmail
                ELSE u.Email
            END AS Email,
            ba.AccountNumber as BankAccountId,
            ba.AccountHolderName,
            ba.AccountNumber,
            ba.BankId as BankName,
            ba.BranchCode,
            ba.IBAN,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pci.Id IS NOT NULL THEN pci.CardHolderName
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN NULL
                ELSE ci.CardHolderName
            END AS CardHolderName,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pci.Id IS NOT NULL THEN pci.CardNumber
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN NULL
                ELSE ci.CardNumber
            END AS CardNumber,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pci.Id IS NOT NULL THEN pci.CVV
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN NULL
                ELSE ci.CVV
            END AS CVV,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pci.Id IS NOT NULL THEN pci.ExpiryMonth
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN NULL
                ELSE ci.ExpiryMonth
            END AS ExpiryMonth,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pci.Id IS NOT NULL THEN pci.ExpiryYear
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN NULL
                ELSE ci.ExpiryYear
            END AS ExpiryYear,
            org.CurrencyId,
			ISNULL((select [Name] from tblLookupItems li where li.LookupType='currency' and li.Id=org.CurrencyId),'usd') as CurrencyCode,
			ISNULL((select [Name] from tblLookupItems li where li.LookupType='currencySign' and li.Id=org.CurrencyId),'$') as CurrencySign,
            spw.BillToType,
            spw.BillToPayerId,
            CASE
                WHEN spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL THEN
                    CASE WHEN pci.Id IS NOT NULL THEN 'PayerCard' ELSE 'PayerManual' END
                ELSE 'ClientCard'
            END AS ChargeSource
        FROM tblBillingInvoice spw 
        LEFT JOIN dbo.tblUser u ON spw.ClientId = u.Id
        LEFT JOIN dbo.tblPayer pay ON spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pay.Id = spw.BillToPayerId
        LEFT JOIN dbo.tblPayerCardInfo pci ON spw.BillToType = 2 AND spw.BillToPayerId IS NOT NULL AND pci.PayerId = spw.BillToPayerId
        LEFT JOIN tblBankAccount ba on spw.ClientId = ba.UserId
        LEFT JOIN tblCardInfo ci ON spw.ClientId = ci.UserId
        LEFT JOIN dbo.tbUserFranchise uf on spw.ClientId = uf.UserId
        LEFT JOIN dbo.tblFranchise f on uf.FranchiseId = f.Id
        LEFT JOIN dbo.tblOrganization org on f.OrganizationId = org.Id
        WHERE ISNULL(spw.IsPaid,0)=0
    END
END
