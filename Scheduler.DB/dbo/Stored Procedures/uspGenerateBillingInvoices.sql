CREATE PROCEDURE [dbo].[uspGenerateBillingInvoices] -- [dbo].[uspGenerateBillingInvoices] "2025-01-01", "2025-03-30", '6F3E3B03-3D2F-463C-8C88-4350F6C93B08'
    @pStartDate       DATE = NULL,
    @pEndDate         DATE = NULL,
    @pOrganizationId  UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @pStartDate IS NULL OR @pEndDate IS NULL OR @pOrganizationId IS NULL
    BEGIN
        SELECT 0 AS GeneratedInvoiceCount;
        RETURN;
    END

    DECLARE @GeneratedInvoiceCount INT = 0;
    DECLARE @TaskIdList NVARCHAR (MAX) = N'';

    SELECT
        @TaskIdList = STUFF((
        SELECT
            N',' + CAST(st.Id AS NVARCHAR(20))
        FROM
            [dbo].[tblServicesTask] st
            INNER JOIN [dbo].[tbUserFranchise] sf
                ON sf.UserId = st.ClientId
            INNER JOIN [dbo].[tblFranchise] f
                ON f.Id = sf.FranchiseId
        WHERE
            st.IsConfirmed = 1
            AND st.[Date] BETWEEN @pStartDate AND @pEndDate
            AND f.OrganizationId = @pOrganizationId
        FOR XML PATH(N''), TYPE
        ).value(N'.[1]', N'nvarchar(max)'),
        1, 1, N'');

    IF (LEN(RTRIM(@TaskIdList)) > 0)
    BEGIN
        EXEC [dbo].[uspApplyTaskBillingFunding]
            @pServicesTasks = @TaskIdList,
            @pOrganizationId = @pOrganizationId;
    END

    IF OBJECT_ID('tempdb..#LineQ') IS NOT NULL
        DROP TABLE #LineQ;

    CREATE TABLE #LineQ
    (
        ClientId              UNIQUEIDENTIFIER NOT NULL,
        TaskId                INT              NULL,
        ExpenseId              UNIQUEIDENTIFIER NULL,
        LineRole               TINYINT         NOT NULL,
        LineAmount             DECIMAL(18, 2)  NOT NULL,
        ExpensePortion         DECIMAL(18, 2)  NULL,
        BillToType             TINYINT         NOT NULL,
        BillToPayerId          UNIQUEIDENTIFIER NULL,
        BillToUserContactId   UNIQUEIDENTIFIER NULL,
        BillToName              NVARCHAR(200)  NULL,
        BillToEmail             NVARCHAR(200)  NULL
    );

    /* Client / household (LineRole 1) task lines */
    INSERT INTO #LineQ
    (ClientId, TaskId, ExpenseId, LineRole, LineAmount, ExpensePortion, BillToType, BillToPayerId, BillToUserContactId, BillToName, BillToEmail)
    SELECT
        b.ClientId,
        b.TaskId,
        NULL,
        1,
        b.LineAmt - b.Billed,
        NULL,
        r.BillToType,
        r.BillToPayerId,
        r.BillToUserContactId,
        r.BillToName,
        r.BillToEmail
    FROM
    (SELECT
        st.Id AS TaskId,
        st.ClientId,
        st.[Date] AS D,
        CASE
            WHEN st.ClientResponsibilityAmount IS NULL
                THEN
                    ISNULL(st.BillingAmount, 0)
            ELSE
                st.ClientResponsibilityAmount
        END AS LineAmt,
        (SELECT
            ISNULL(SUM(d.Amount), 0)
         FROM
            [dbo].[tblBillingInvoiceDetail] d
         WHERE
            d.TaskId = st.Id
            AND ISNULL(d.LineRole, 1) = 1) AS Billed
     FROM
        [dbo].[tblServicesTask] st
        INNER JOIN [dbo].[tbUserFranchise] sf
            ON sf.UserId = st.ClientId
        INNER JOIN [dbo].[tblFranchise] f
            ON f.Id = sf.FranchiseId
     WHERE
        st.IsConfirmed = 1
        AND st.[Date] BETWEEN @pStartDate AND @pEndDate
        AND f.OrganizationId = @pOrganizationId) b
    CROSS APPLY
        [dbo].[fn_ResolveClientBillTo] (b.ClientId, b.D, @pOrganizationId) r
    WHERE
        b.LineAmt - b.Billed > 0.009;

    /* Payer-funded (LineRole 2) */
    INSERT INTO #LineQ
    (ClientId, TaskId, ExpenseId, LineRole, LineAmount, ExpensePortion, BillToType, BillToPayerId, BillToUserContactId, BillToName, BillToEmail)
    SELECT
        b.ClientId,
        b.TaskId,
        NULL,
        2,
        b.LineAmt - b.Billed,
        NULL,
        2,
        b.FundingPayerId,
        NULL,
        pay.LegalName,
        pay.BillingEmail
    FROM
    (SELECT
        st.Id AS TaskId,
        st.ClientId,
        st.FundingPayerId,
        ISNULL(st.PayerResponsibilityAmount, 0) AS LineAmt,
        (SELECT
            ISNULL(SUM(d.Amount), 0)
         FROM
            [dbo].[tblBillingInvoiceDetail] d
         WHERE
            d.TaskId = st.Id
            AND d.LineRole = 2) AS Billed
     FROM
        [dbo].[tblServicesTask] st
        INNER JOIN [dbo].[tbUserFranchise] sf
            ON sf.UserId = st.ClientId
        INNER JOIN [dbo].[tblFranchise] f
            ON f.Id = sf.FranchiseId
     WHERE
        st.IsConfirmed = 1
        AND st.[Date] BETWEEN @pStartDate AND @pEndDate
        AND f.OrganizationId = @pOrganizationId
        AND st.FundingPayerId IS NOT NULL) b
    INNER JOIN
        [dbo].[tblPayer] pay
        ON pay.Id = b.FundingPayerId
    WHERE
        b.LineAmt - b.Billed > 0.009;

    /* Expense lines (all go to same debtor as client share for that visit) */
    INSERT INTO #LineQ
    (ClientId, TaskId, ExpenseId, LineRole, LineAmount, ExpensePortion, BillToType, BillToPayerId, BillToUserContactId, BillToName, BillToEmail)
    SELECT
        st.ClientId,
        ue.TaskId,
        ue.Id,
        1,
        0,
        (ue.Amount - ISNULL(eb.Billed, 0)),
        r.BillToType,
        r.BillToPayerId,
        r.BillToUserContactId,
        r.BillToName,
        r.BillToEmail
    FROM
        [dbo].[tblUserExpense] ue
        INNER JOIN [dbo].[tblServicesTask] st
            ON st.Id = ue.TaskId
        INNER JOIN [dbo].[tbUserFranchise] sf
            ON sf.UserId = st.ClientId
        INNER JOIN [dbo].[tblFranchise] f
            ON f.Id = sf.FranchiseId
    OUTER APPLY
        (SELECT
            ISNULL(SUM(d.ExpenseAmount), 0) AS Billed
         FROM
            [dbo].[tblBillingInvoiceDetail] d
         WHERE
            d.ExpenseId = ue.Id) eb
    CROSS APPLY
        [dbo].[fn_ResolveClientBillTo] (st.ClientId, st.[Date], @pOrganizationId) r
    WHERE
        f.OrganizationId = @pOrganizationId
        AND ISNULL(ue.IsActive, 0) = 1
        AND ISNULL(ue.IsConfirmed, 0) = 1
        AND st.IsConfirmed = 1
        AND st.[Date] BETWEEN @pStartDate AND @pEndDate
        AND (ue.Amount - ISNULL(eb.Billed, 0) > 0.009);

    IF NOT EXISTS (SELECT
        1
    FROM
        #LineQ)
    BEGIN
        SELECT
            @GeneratedInvoiceCount AS GeneratedInvoiceCount;
        RETURN;
    END

    /* One invoice per (ClientId, bill-to, period) */
    IF OBJECT_ID('tempdb..#Gr') IS NOT NULL
        DROP TABLE #Gr;
    SELECT
        q.ClientId,
        q.BillToType,
        q.BillToPayerId,
        q.BillToUserContactId,
        MAX(q.BillToName)  AS BillToName,
        MAX(q.BillToEmail)  AS BillToEmail
    INTO
        #Gr
    FROM
        #LineQ q
    GROUP BY
        q.ClientId,
        q.BillToType,
        q.BillToPayerId,
        q.BillToUserContactId;

    DECLARE
        @ClientIdG UNIQUEIDENTIFIER,
        @Btt TINYINT,
        @Bpid UNIQUEIDENTIFIER,
        @Bucid UNIQUEIDENTIFIER,
        @Bname NVARCHAR(200),
        @Bemail NVARCHAR(200);
    DECLARE
        grp_c CURSOR LOCAL FAST_FORWARD
        FOR
        SELECT
            g.ClientId,
            g.BillToType,
            g.BillToPayerId,
            g.BillToUserContactId,
            g.BillToName,
            g.BillToEmail
        FROM
            #Gr g;

    DECLARE @NewInvoiceId       INT;
    DECLARE @TaxPercentage     DECIMAL(18, 2);
    DECLARE @DiscountPercentage DECIMAL(18, 2);
    DECLARE @vTotalAmount       DECIMAL(18, 2);
    OPEN grp_c;
    FETCH NEXT
        FROM
            grp_c
        INTO
        @ClientIdG, @Btt, @Bpid, @Bucid, @Bname, @Bemail;
    WHILE (@@FETCH_STATUS = 0)
    BEGIN

        SELECT
            @TaxPercentage = ISNULL(org.TaxPercentage, 0),
            @DiscountPercentage = ISNULL(org.DiscountPercentage, 0)
        FROM
            [dbo].[tblUser] u
            INNER JOIN [dbo].tbUserFranchise sf
                ON sf.UserId = u.Id
            INNER JOIN
                [dbo].tblFranchise f
                ON f.Id = sf.FranchiseId
            INNER JOIN
                [dbo].tblOrganization org
                ON org.Id = f.OrganizationId
        WHERE
            u.Id = @ClientIdG
            AND f.OrganizationId = @pOrganizationId;

        INSERT INTO [dbo].[tblBillingInvoice]
        ( [ClientId],
         [Details],
         [Date],
         [StartDate],
         [EndDate],
         [DueDate],
         [IsPaid],
         [Row_Guid],
         [BillToType],
         [BillToPayerId],
         [BillToUserContactId],
         [BillToDisplayName],
         [DebtorEmail]
        )
        VALUES
        ( @ClientIdG,
         N'Invoice (recipient: ' + LTRIM(RTRIM(CAST(@ClientIdG AS NCHAR(36)))) + N')', -- service recipient always client; PDF uses BillTo
         CAST(GETDATE() AS DATE),
         @pStartDate,
         @pEndDate,
         DATEADD(DAY, 7, @pEndDate),
         0,
         NEWID(),
         @Btt,
         @Bpid,
         @Bucid,
         @Bname,
         @Bemail
        );
        SET @NewInvoiceId = SCOPE_IDENTITY();
        SET @GeneratedInvoiceCount += 1;

        INSERT INTO [dbo].[tblBillingInvoiceDetail] ([BillingInvoiceId], [TaskId], [Amount], [ExpenseId], [ExpenseAmount], [LineRole])
        SELECT
            @NewInvoiceId,
            t.TaskId,
            CASE
                WHEN t.ExpenseId IS NULL
                    THEN
                        t.LineAmount
                ELSE
                    0
            END,
            t.ExpenseId,
            t.ExpensePortion,
            t.LineRole
        FROM
            #LineQ t
        WHERE
            t.ClientId = @ClientIdG
            AND t.BillToType = @Btt
            AND ISNULL(t.BillToPayerId, '00000000-0000-0000-0000-000000000000') = ISNULL(@Bpid, '00000000-0000-0000-0000-000000000000')
            AND ISNULL(t.BillToUserContactId, '00000000-0000-0000-0000-000000000000') = ISNULL(@Bucid, '00000000-0000-0000-0000-000000000000');

        SELECT
            @vTotalAmount = SUM(ISNULL(Amount, 0) + ISNULL(ExpenseAmount, 0))
        FROM
            [dbo].[tblBillingInvoiceDetail] d
        WHERE
            d.BillingInvoiceId = @NewInvoiceId;
        UPDATE
            [dbo].[tblBillingInvoice]
        SET
            [TotalAmount] = ISNULL(@vTotalAmount, 0),
            [DiscountPercentage] = @DiscountPercentage,
            [AmountAfterDiscount] = ISNULL(@vTotalAmount, 0) - (ISNULL(@vTotalAmount, 0) * @DiscountPercentage / 100),
            [TaxPercentage] = @TaxPercentage,
            [AmountAfterTax] = (ISNULL(@vTotalAmount, 0) - (ISNULL(@vTotalAmount, 0) * @DiscountPercentage / 100))
            + ((ISNULL(@vTotalAmount, 0) - (ISNULL(@vTotalAmount, 0) * @DiscountPercentage / 100)) * @TaxPercentage / 100)
        WHERE
            Id = @NewInvoiceId;

        FETCH NEXT
            FROM
                grp_c
            INTO
        @ClientIdG, @Btt, @Bpid, @Bucid, @Bname, @Bemail;
    END
    CLOSE grp_c;
    DEALLOCATE grp_c;

    SELECT
        @GeneratedInvoiceCount AS GeneratedInvoiceCount;
END
