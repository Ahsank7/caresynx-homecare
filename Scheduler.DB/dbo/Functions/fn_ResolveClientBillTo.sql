-- Resolves "who to invoice" for the client's own share of charges (self, org payer, or contact guarantor).
CREATE FUNCTION [dbo].[fn_ResolveClientBillTo]
(
    @ClientId UNIQUEIDENTIFIER,
    @AsOfDate  DATE,
    @OrganizationId UNIQUEIDENTIFIER
)
RETURNS TABLE
AS
RETURN
(
    SELECT
        r.BillToType,
        r.BillToPayerId,
        r.BillToUserContactId,
        r.BillToName,
        r.BillToEmail
    FROM (
        SELECT
            COALESCE(
                p.BillToType,
                CASE WHEN d.PayerId IS NOT NULL THEN 2 END,
                1
            ) AS BillToType,
            COALESCE(
                p.PayerId,
                d.PayerId
            ) AS PayerId,
            p.UserContactId
        FROM (SELECT 1 AS x) z
        OUTER APPLY (
            SELECT TOP 1 cbf.BillToType, cbf.PayerId, cbf.UserContactId
            FROM [dbo].[tblClientBillingPreference] cbf
            WHERE cbf.ClientId = @ClientId
        ) p
        OUTER APPLY (
            SELECT TOP 1 cp.PayerId
            FROM [dbo].[tblClientPayer] cp
            INNER JOIN [dbo].[tblPayer] ap
                ON ap.Id = cp.PayerId
               AND ap.OrganizationId = @OrganizationId
               AND ap.IsActive = 1
            WHERE cp.ClientId = @ClientId
              AND ISNULL(cp.IsActive, 0) = 1
              AND ISNULL(cp.IsDefaultBillTo, 0) = 1
              AND cp.EffectiveFrom <= @AsOfDate
              AND (cp.EffectiveTo IS NULL OR cp.EffectiveTo >= @AsOfDate)
            ORDER BY cp.Id
        ) d
    ) a
    CROSS APPLY (SELECT
        a.BillToType,
        CASE WHEN a.BillToType = 2 THEN a.PayerId END AS BillToPayerId,
        CASE WHEN a.BillToType = 3 THEN a.UserContactId END AS BillToUserContactId
    ) c
    LEFT OUTER JOIN [dbo].[tblPayer] pay
        ON c.BillToType = 2
       AND pay.Id = c.BillToPayerId
    LEFT OUTER JOIN [dbo].[tblUser] cu
        ON c.BillToType = 1
       AND cu.Id = @ClientId
    -- tblUserContact: UserId = guarantor/contact person (tblUser); ContactUserId = care recipient (client).
    -- Matches [Contact].[InsertUpdateContact] and [Contact].[uspGetAllContacts] (C.ContactUserId = client).
    LEFT OUTER JOIN [dbo].[tblUserContact] tuc
        ON c.BillToType = 3
       AND tuc.Id = c.BillToUserContactId
       AND tuc.ContactUserId = @ClientId
       AND ISNULL(tuc.IsActive, 0) = 1
    LEFT OUTER JOIN [dbo].[tblUser] g
        ON c.BillToType = 3
       AND g.Id = tuc.UserId
    CROSS APPLY (SELECT
        CASE
            WHEN c.BillToType = 1 THEN
                LTRIM(RTRIM(
                    ISNULL(cu.FirstName, N'')
                    + N' ' + ISNULL(cu.SurName, N'')
                    + N' ' + ISNULL(cu.LastName, N'')
                ))
            WHEN c.BillToType = 2 THEN
                pay.LegalName
            WHEN c.BillToType = 3 THEN
                LTRIM(RTRIM(
                    ISNULL(g.FirstName, N'')
                    + N' ' + ISNULL(g.SurName, N'')
                    + N' ' + ISNULL(g.LastName, N'')
                ))
        END AS BillToName,
        CASE
            WHEN c.BillToType = 1 THEN cu.Email
            WHEN c.BillToType = 2 THEN pay.BillingEmail
            WHEN c.BillToType = 3 THEN g.Email
        END AS BillToEmail
    ) nm
    CROSS APPLY (SELECT
        c.BillToType,
        c.BillToPayerId,
        c.BillToUserContactId,
        LTRIM(RTRIM(
            replace(replace(replace(replace(ISNULL(nm.BillToName, N''), N'  ', N' ' + NCHAR(0)), N' ' + NCHAR(0) + N' ', N' '), N' ' + NCHAR(0) + N' ', N' '), N' ' + NCHAR(0), N' ')
        )) AS Nm2,
        nm.BillToEmail
    ) r2
    CROSS APPLY (SELECT
        r2.BillToType,
        r2.BillToPayerId,
        r2.BillToUserContactId,
        r2.Nm2 AS BillToName,
        r2.BillToEmail
    ) r
);
