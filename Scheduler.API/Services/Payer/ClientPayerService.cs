using Dapper;
using Scheduler.API.Models.Payer;
using System.Data;
using System.Linq;

// Row shape for [dbo].[GetPayerCardInfo]
file class PayerCardRow
{
    public int Id { get; set; }
    public Guid CardId { get; set; }
    public Guid PayerId { get; set; }
    public string CardHolderName { get; set; }
    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string CVV { get; set; }
    public int TypeId { get; set; }
}

namespace Scheduler.API.Services.Payer
{
    public class ClientPayerService : IClientPayerService
    {
        private readonly IDapperRepository _dapper;

        public ClientPayerService(IDapperRepository dapper)
        {
            _dapper = dapper;
        }

        public async Task<IReadOnlyList<PayerDto>> GetPayersByOrganizationAsync(Guid organizationId)
        {
            const string sql = @"
SELECT Id, OrganizationId, LegalName, PayerType,
  BillingAddressLine1, BillingAddressLine2, BillingAddressLine3,
  BillingEmail, DefaultPaymentTermsDays, TaxId, IsActive
FROM [dbo].[tblPayer] WITH (NOLOCK)
WHERE OrganizationId = @OrgId AND IsActive = 1
ORDER BY LegalName;";
            var p = new DynamicParameters();
            p.Add("@OrgId", organizationId, DbType.Guid);
            var rows = await _dapper.QueryAsync<PayerDto>(sql, p, CommandType.Text);
            return rows;
        }

        public async Task<Guid> SavePayerAsync(PayerDto model)
        {
            if (!model.Id.HasValue || model.Id == Guid.Empty) model.Id = Guid.NewGuid();
            const string sql = @"
MERGE [dbo].[tblPayer] AS t
USING (SELECT @Id AS Id) AS s ON t.Id = s.Id
WHEN MATCHED THEN UPDATE SET
  LegalName = @LegalName, PayerType = @PayerType,
  BillingAddressLine1 = @A1, BillingAddressLine2 = @A2, BillingAddressLine3 = @A3,
  BillingEmail = @E, DefaultPaymentTermsDays = @Terms, TaxId = @Tax, IsActive = @Active,
  UpdatedDate = GETUTCDATE()
WHEN NOT MATCHED THEN INSERT
  (Id, OrganizationId, LegalName, PayerType, BillingAddressLine1, BillingAddressLine2, BillingAddressLine3,
   BillingEmail, DefaultPaymentTermsDays, TaxId, IsActive, CreatedDate)
  VALUES (@Id, @OrgId, @LegalName, @PayerType, @A1, @A2, @A3, @E, @Terms, @Tax, @Active, GETUTCDATE());
";
            var p = new DynamicParameters();
            p.Add("@Id", model.Id, DbType.Guid);
            p.Add("@OrgId", model.OrganizationId, DbType.Guid);
            p.Add("@LegalName", model.LegalName, DbType.String);
            p.Add("@PayerType", model.PayerType, DbType.Byte);
            p.Add("@A1", model.BillingAddressLine1, DbType.String);
            p.Add("@A2", model.BillingAddressLine2, DbType.String);
            p.Add("@A3", model.BillingAddressLine3, DbType.String);
            p.Add("@E", model.BillingEmail, DbType.String);
            p.Add("@Terms", model.DefaultPaymentTermsDays, DbType.Int32);
            p.Add("@Tax", model.TaxId, DbType.String);
            p.Add("@Active", model.IsActive, DbType.Boolean);
            await _dapper.ExecuteAsync(sql, p, CommandType.Text);
            return model.Id!.Value;
        }

        public async Task<IReadOnlyList<ClientPayerCoverageDto>> GetClientCoverageAsync(Guid clientId)
        {
            const string sql = @"
SELECT cp.Id, cp.ClientId, cp.PayerId, p.LegalName AS PayerLegalName, cp.EffectiveFrom, cp.EffectiveTo,
  cp.IsDefaultBillTo, cp.MemberNumber, cp.PolicyNumber, cp.Notes, cp.IsActive
FROM [dbo].[tblClientPayer] cp WITH (NOLOCK)
JOIN [dbo].[tblPayer] p ON p.Id = cp.PayerId
WHERE cp.ClientId = @ClientId
ORDER BY cp.EffectiveFrom DESC;";
            var p = new DynamicParameters();
            p.Add("@ClientId", clientId, DbType.Guid);
            var rows = await _dapper.QueryAsync<ClientPayerCoverageDto>(sql, p, CommandType.Text);
            return rows;
        }

        public async Task<int> SaveClientCoverageAsync(ClientPayerCoverageDto model)
        {
            if (model.Id <= 0)
            {
                const string ins = @"
INSERT INTO [dbo].[tblClientPayer] (ClientId, PayerId, EffectiveFrom, EffectiveTo, IsDefaultBillTo, MemberNumber, PolicyNumber, Notes, IsActive)
VALUES (@ClientId, @PayerId, @From, @To, @Def, @Mem, @Pol, @Notes, @Act);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                var dp = new DynamicParameters();
                dp.Add("@ClientId", model.ClientId, DbType.Guid);
                dp.Add("@PayerId", model.PayerId, DbType.Guid);
                dp.Add("@From", model.EffectiveFrom.Date, DbType.Date);
                dp.Add("@To", model.EffectiveTo?.Date, DbType.Date);
                dp.Add("@Def", model.IsDefaultBillTo, DbType.Boolean);
                dp.Add("@Mem", model.MemberNumber, DbType.String);
                dp.Add("@Pol", model.PolicyNumber, DbType.String);
                dp.Add("@Notes", model.Notes, DbType.String);
                dp.Add("@Act", model.IsActive, DbType.Boolean);
                var ids = await _dapper.QueryAsync<int>(ins, dp, CommandType.Text);
                return ids.FirstOrDefault();
            }
            const string upd = @"
UPDATE [dbo].[tblClientPayer] SET
  PayerId = @PayerId, EffectiveFrom = @From, EffectiveTo = @To, IsDefaultBillTo = @Def,
  MemberNumber = @Mem, PolicyNumber = @Pol, Notes = @Notes, IsActive = @Act
WHERE Id = @Id AND ClientId = @ClientId;";
            var p2 = new DynamicParameters();
            p2.Add("@Id", model.Id, DbType.Int32);
            p2.Add("@ClientId", model.ClientId, DbType.Guid);
            p2.Add("@PayerId", model.PayerId, DbType.Guid);
            p2.Add("@From", model.EffectiveFrom.Date, DbType.Date);
            p2.Add("@To", model.EffectiveTo?.Date, DbType.Date);
            p2.Add("@Def", model.IsDefaultBillTo, DbType.Boolean);
            p2.Add("@Mem", model.MemberNumber, DbType.String);
            p2.Add("@Pol", model.PolicyNumber, DbType.String);
            p2.Add("@Notes", model.Notes, DbType.String);
            p2.Add("@Act", model.IsActive, DbType.Boolean);
            await _dapper.ExecuteAsync(upd, p2, CommandType.Text);
            return model.Id;
        }

        public async Task<ClientBillingPreferenceDto> GetClientBillingPreferenceAsync(Guid clientId)
        {
            const string sql = @"
SELECT ClientId, BillToType, PayerId, UserContactId
FROM [dbo].[tblClientBillingPreference] WITH (NOLOCK)
WHERE ClientId = @ClientId;";
            var p = new DynamicParameters();
            p.Add("@ClientId", clientId, DbType.Guid);
            var row = await _dapper.GetAsync<ClientBillingPreferenceDto>(sql, p, CommandType.Text);
            if (row != null && row.ClientId != Guid.Empty)
                return row;
            return new ClientBillingPreferenceDto { ClientId = clientId, BillToType = 1 };
        }

        public async Task SaveClientBillingPreferenceAsync(ClientBillingPreferenceDto model)
        {
            const string mrg = @"
IF EXISTS (SELECT 1 FROM [dbo].[tblClientBillingPreference] WHERE ClientId = @ClientId)
  UPDATE [dbo].[tblClientBillingPreference] SET BillToType = @Btt, PayerId = @PayerId, UserContactId = @Cid, UpdatedDate = GETUTCDATE() WHERE ClientId = @ClientId
ELSE
  INSERT INTO [dbo].[tblClientBillingPreference] (ClientId, BillToType, PayerId, UserContactId, UpdatedDate) VALUES (@ClientId, @Btt, @PayerId, @Cid, GETUTCDATE());";
            var p = new DynamicParameters();
            p.Add("@ClientId", model.ClientId, DbType.Guid);
            p.Add("@Btt", model.BillToType, DbType.Byte);
            p.Add("@PayerId", model.PayerId, DbType.Guid);
            p.Add("@Cid", model.UserContactId, DbType.Guid);
            await _dapper.ExecuteAsync(mrg, p, CommandType.Text);
        }

        public async Task<IReadOnlyList<ClientPayerServiceFundingDto>> GetFundingRulesAsync(Guid clientId, Guid organizationId)
        {
            const string sql = @"
SELECT Id, OrganizationId, ClientId, PayerId, ServiceId, FundedPercent, EffectiveFrom, EffectiveTo, IsActive
FROM [dbo].[tblClientPayerServiceFunding] WITH (NOLOCK)
WHERE ClientId = @ClientId AND OrganizationId = @Org
ORDER BY EffectiveFrom DESC, Id DESC;";
            var p = new DynamicParameters();
            p.Add("@ClientId", clientId, DbType.Guid);
            p.Add("@Org", organizationId, DbType.Guid);
            var rows = await _dapper.QueryAsync<ClientPayerServiceFundingDto>(sql, p, CommandType.Text);
            return rows;
        }

        public async Task<int> SaveFundingRuleAsync(ClientPayerServiceFundingDto model)
        {
            if (model.Id <= 0)
            {
                const string ins = @"
INSERT INTO [dbo].[tblClientPayerServiceFunding] (OrganizationId, ClientId, PayerId, ServiceId, FundedPercent, EffectiveFrom, EffectiveTo, IsActive)
VALUES (@Org, @Client, @Payer, @Svc, @Pct, @From, @To, @Act);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                var dp = new DynamicParameters();
                dp.Add("@Org", model.OrganizationId, DbType.Guid);
                dp.Add("@Client", model.ClientId, DbType.Guid);
                dp.Add("@Payer", model.PayerId, DbType.Guid);
                dp.Add("@Svc", model.ServiceId, DbType.Int32);
                dp.Add("@Pct", model.FundedPercent, DbType.Decimal);
                dp.Add("@From", model.EffectiveFrom.Date, DbType.Date);
                dp.Add("@To", model.EffectiveTo?.Date, DbType.Date);
                dp.Add("@Act", model.IsActive, DbType.Boolean);
                var ids = await _dapper.QueryAsync<int>(ins, dp, CommandType.Text);
                return ids.FirstOrDefault();
            }
            const string upd = @"
UPDATE [dbo].[tblClientPayerServiceFunding] SET
  PayerId = @Payer, ServiceId = @Svc, FundedPercent = @Pct, EffectiveFrom = @From, EffectiveTo = @To, IsActive = @Act
WHERE Id = @Id AND ClientId = @Client AND OrganizationId = @Org;";
            var p2 = new DynamicParameters();
            p2.Add("@Id", model.Id, DbType.Int32);
            p2.Add("@Org", model.OrganizationId, DbType.Guid);
            p2.Add("@Client", model.ClientId, DbType.Guid);
            p2.Add("@Payer", model.PayerId, DbType.Guid);
            p2.Add("@Svc", model.ServiceId, DbType.Int32);
            p2.Add("@Pct", model.FundedPercent, DbType.Decimal);
            p2.Add("@From", model.EffectiveFrom.Date, DbType.Date);
            p2.Add("@To", model.EffectiveTo?.Date, DbType.Date);
            p2.Add("@Act", model.IsActive, DbType.Boolean);
            await _dapper.ExecuteAsync(upd, p2, CommandType.Text);
            return model.Id;
        }

        public async Task DeleteFundingRuleAsync(int id, Guid organizationId)
        {
            const string sql = @"DELETE FROM [dbo].[tblClientPayerServiceFunding] WHERE Id = @Id AND OrganizationId = @Org;";
            var p = new DynamicParameters();
            p.Add("@Id", id, DbType.Int32);
            p.Add("@Org", organizationId, DbType.Guid);
            await _dapper.ExecuteAsync(sql, p, CommandType.Text);
        }

        public async Task<IReadOnlyList<OrganizationPayerServiceFundingDto>> GetOrganizationFundingRulesAsync(Guid organizationId)
        {
            const string sql = @"
SELECT Id, OrganizationId, PayerId, ServiceId, FundedPercent, EffectiveFrom, EffectiveTo, IsActive
FROM [dbo].[tblOrganizationPayerServiceFunding] WITH (NOLOCK)
WHERE OrganizationId = @Org
ORDER BY EffectiveFrom DESC, Id DESC;";
            var p = new DynamicParameters();
            p.Add("@Org", organizationId, DbType.Guid);
            var rows = await _dapper.QueryAsync<OrganizationPayerServiceFundingDto>(sql, p, CommandType.Text);
            return rows;
        }

        public async Task<int> SaveOrganizationFundingRuleAsync(OrganizationPayerServiceFundingDto model)
        {
            if (model.Id <= 0)
            {
                const string ins = @"
INSERT INTO [dbo].[tblOrganizationPayerServiceFunding] (OrganizationId, PayerId, ServiceId, FundedPercent, EffectiveFrom, EffectiveTo, IsActive)
VALUES (@Org, @Payer, @Svc, @Pct, @From, @To, @Act);
SELECT CAST(SCOPE_IDENTITY() AS int);";
                var dp = new DynamicParameters();
                dp.Add("@Org", model.OrganizationId, DbType.Guid);
                dp.Add("@Payer", model.PayerId, DbType.Guid);
                dp.Add("@Svc", model.ServiceId, DbType.Int32);
                dp.Add("@Pct", model.FundedPercent, DbType.Decimal);
                dp.Add("@From", model.EffectiveFrom.Date, DbType.Date);
                dp.Add("@To", model.EffectiveTo?.Date, DbType.Date);
                dp.Add("@Act", model.IsActive, DbType.Boolean);
                var ids = await _dapper.QueryAsync<int>(ins, dp, CommandType.Text);
                return ids.FirstOrDefault();
            }
            const string upd = @"
UPDATE [dbo].[tblOrganizationPayerServiceFunding] SET
  PayerId = @Payer, ServiceId = @Svc, FundedPercent = @Pct, EffectiveFrom = @From, EffectiveTo = @To, IsActive = @Act
WHERE Id = @Id AND OrganizationId = @Org;";
            var p2 = new DynamicParameters();
            p2.Add("@Id", model.Id, DbType.Int32);
            p2.Add("@Org", model.OrganizationId, DbType.Guid);
            p2.Add("@Payer", model.PayerId, DbType.Guid);
            p2.Add("@Svc", model.ServiceId, DbType.Int32);
            p2.Add("@Pct", model.FundedPercent, DbType.Decimal);
            p2.Add("@From", model.EffectiveFrom.Date, DbType.Date);
            p2.Add("@To", model.EffectiveTo?.Date, DbType.Date);
            p2.Add("@Act", model.IsActive, DbType.Boolean);
            await _dapper.ExecuteAsync(upd, p2, CommandType.Text);
            return model.Id;
        }

        public async Task DeleteOrganizationFundingRuleAsync(int id, Guid organizationId)
        {
            const string sql = @"DELETE FROM [dbo].[tblOrganizationPayerServiceFunding] WHERE Id = @Id AND OrganizationId = @Org;";
            var p = new DynamicParameters();
            p.Add("@Id", id, DbType.Int32);
            p.Add("@Org", organizationId, DbType.Guid);
            await _dapper.ExecuteAsync(sql, p, CommandType.Text);
        }

        public async Task<PayerCardInfoDto?> GetPayerCardAsync(Guid organizationId, Guid payerId)
        {
            const string verify = @"
SELECT CAST(COUNT(1) AS int) FROM [dbo].[tblPayer] WITH (NOLOCK)
WHERE [Id] = @PayerId AND [OrganizationId] = @OrgId AND [IsActive] = 1;";
            var vp = new DynamicParameters();
            vp.Add("@PayerId", payerId, DbType.Guid);
            vp.Add("@OrgId", organizationId, DbType.Guid);
            var cnt = await _dapper.GetAsync<int>(verify, vp, CommandType.Text);
            if (cnt == 0)
                throw new InvalidOperationException("Payer not found in this organization.");

            var dp = new DynamicParameters();
            dp.Add("@pPayerId", payerId, DbType.Guid);
            var rows = await _dapper.GetListAsync<PayerCardRow>("[dbo].[GetPayerCardInfo]", dp, CommandType.StoredProcedure);
            var row = rows.FirstOrDefault();
            if (row == null)
                return null;

            return new PayerCardInfoDto
            {
                CardId = row.CardId,
                PayerId = row.PayerId,
                CardHolderName = row.CardHolderName,
                CardNumber = row.CardNumber,
                ExpiryMonth = row.ExpiryMonth,
                ExpiryYear = row.ExpiryYear,
                CVV = row.CVV,
                TypeId = row.TypeId
            };
        }

        public async Task<Guid> UpsertPayerCardAsync(UpsertPayerCardViewModel model)
        {
            const string verify = @"
SELECT CAST(COUNT(1) AS int) FROM [dbo].[tblPayer] WITH (NOLOCK)
WHERE [Id] = @PayerId AND [OrganizationId] = @OrgId AND [IsActive] = 1;";
            var vp = new DynamicParameters();
            vp.Add("@PayerId", model.PayerId, DbType.Guid);
            vp.Add("@OrgId", model.OrganizationId, DbType.Guid);
            var cnt = await _dapper.GetAsync<int>(verify, vp, CommandType.Text);
            if (cnt == 0)
                throw new InvalidOperationException("Payer not found in this organization.");

            var dp = new DynamicParameters();
            dp.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
            dp.Add("@pPayerId", model.PayerId, DbType.Guid);
            dp.Add("@pCardId", model.CardId, DbType.Guid);
            dp.Add("@pCVV", model.CVV, DbType.String);
            dp.Add("@pCardHolderName", model.CardHolderName, DbType.String);
            dp.Add("@pCardNumber", model.CardNumber, DbType.String);
            dp.Add("@pTypeId", model.TypeId, DbType.Int32);
            dp.Add("@pExpiryYear", model.ExpiryYear, DbType.Int32);
            dp.Add("@pExpiryMonth", model.ExpiryMonth, DbType.Int32);

            await _dapper.ExecuteAsync("[dbo].[InsertUpdatePayerCardInfo]", dp, CommandType.StoredProcedure);
            var outId = dp.Get<Guid>("@pOutId");
            if (outId == Guid.Empty)
                throw new InvalidOperationException("Could not save payer card.");
            return outId;
        }
    }
}
