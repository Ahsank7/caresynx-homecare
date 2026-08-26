CREATE PROCEDURE [dbo].[uspApplyTaskBillingFunding]
    @pServicesTasks NVARCHAR (MAX) NULL,
    @pOrganizationId UNIQUEIDENTIFIER NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @pServicesTasks IS NULL OR LEN(RTRIM(@pServicesTasks)) = 0
        RETURN;

    DECLARE @ServiceTaskIds TABLE (ServiceTaskId INT PRIMARY KEY);
    INSERT INTO @ServiceTaskIds (ServiceTaskId)
    SELECT DISTINCT CAST([value] AS INT)
    FROM STRING_SPLIT(@pServicesTasks, ',')
    WHERE LEN(RTRIM([value])) > 0
      AND ISNUMERIC([value]) = 1;

    ;WITH TaskService AS (
        SELECT
            st.Id AS ServiceTaskId,
            st.ClientId,
            st.[Date],
            ISNULL(st.BillingAmount, 0) AS BillingAmount,
            (SELECT TOP 1 s.Id
             FROM [dbo].[tblScheduler] sch
             INNER JOIN [dbo].[tblServices] s
                ON sch.CSVServiceIds IS NOT NULL
               AND LEN(RTRIM(sch.CSVServiceIds)) > 0
               AND CHARINDEX(',' + CAST(s.Id AS NVARCHAR(10)) + ',', ',' + sch.CSVServiceIds + ',') > 0
             WHERE sch.Id = st.ScheduleId
             ORDER BY s.Id) AS ServiceId
        FROM [dbo].[tblServicesTask] st
        INNER JOIN @ServiceTaskIds t ON t.ServiceTaskId = st.Id
    ),
    MatchedRule AS (
        SELECT
            ts.ServiceTaskId,
            ts.BillingAmount,
            f.PayerId,
            f.FundedPercent,
            ROUND(ts.BillingAmount * f.FundedPercent / 100.0, 2) AS PayerPart
        FROM TaskService ts
        OUTER APPLY (
            SELECT TOP 1
                cpsf.PayerId,
                cpsf.FundedPercent
            FROM [dbo].[tblClientPayerServiceFunding] cpsf
            WHERE cpsf.OrganizationId = @pOrganizationId
              AND cpsf.ClientId = ts.ClientId
              AND cpsf.IsActive = 1
              AND cpsf.EffectiveFrom <= ts.[Date]
              AND (cpsf.EffectiveTo IS NULL OR cpsf.EffectiveTo >= ts.[Date])
              AND (cpsf.ServiceId IS NULL OR cpsf.ServiceId = ts.ServiceId)
            ORDER BY
                CASE WHEN cpsf.ServiceId IS NOT NULL THEN 0 ELSE 1 END,
                cpsf.Id
        ) cr
        OUTER APPLY (
            SELECT TOP 1
                opsf.PayerId,
                opsf.FundedPercent
            FROM [dbo].[tblOrganizationPayerServiceFunding] opsf
            WHERE @pOrganizationId IS NOT NULL
              AND opsf.OrganizationId = @pOrganizationId
              AND cr.PayerId IS NULL
              AND opsf.IsActive = 1
              AND opsf.EffectiveFrom <= ts.[Date]
              AND (opsf.EffectiveTo IS NULL OR opsf.EffectiveTo >= ts.[Date])
              AND (opsf.ServiceId IS NULL OR (ts.ServiceId IS NOT NULL AND opsf.ServiceId = ts.ServiceId))
            ORDER BY
                CASE WHEN opsf.ServiceId IS NOT NULL THEN 0 ELSE 1 END,
                opsf.Id
        ) orf
        CROSS APPLY (
            SELECT
                COALESCE(cr.PayerId, orf.PayerId) AS PayerId,
                COALESCE(cr.FundedPercent, orf.FundedPercent) AS FundedPercent
        ) f
    )
    UPDATE st
    SET
        st.ClientResponsibilityAmount = CASE
            WHEN m.PayerId IS NULL OR ISNULL(m.PayerPart, 0) <= 0 THEN m.BillingAmount
            ELSE m.BillingAmount - m.PayerPart
        END,
        st.PayerResponsibilityAmount = CASE
            WHEN m.PayerId IS NULL OR ISNULL(m.PayerPart, 0) <= 0 THEN 0
            ELSE m.PayerPart
        END,
        st.FundingPayerId = CASE
            WHEN m.PayerId IS NULL OR ISNULL(m.PayerPart, 0) <= 0 THEN NULL
            ELSE m.PayerId
        END
    FROM [dbo].[tblServicesTask] st
    INNER JOIN MatchedRule m ON m.ServiceTaskId = st.Id;
END
