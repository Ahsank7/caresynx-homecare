using Dapper;
using Scheduler.API.Models.Complaint;
using System.Data;

namespace Scheduler.API.Services.Complaint
{
    public class ComplaintRepository : IComplaint
    {
        private readonly IDapperRepository _dapperRepository;

        public ComplaintRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<List<ComplaintInfo>> GetComplaintsAsync(GetComplaintsRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@UserId", request.UserId, DbType.Guid);
                dp_params.Add("@ComplainantId", request.ComplainantId, DbType.Guid);
                dp_params.Add("@ComplainedAgainstId", request.ComplainedAgainstId, DbType.Guid);
                dp_params.Add("@FranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@Status", request.Status, DbType.Int32);
                dp_params.Add("@Category", request.Category, DbType.Int32);
                dp_params.Add("@Severity", request.Severity, DbType.Int32);
                dp_params.Add("@IncludeInactive", request.IncludeInactive, DbType.Boolean);

                var result = await Task.FromResult(_dapperRepository.GetList<ComplaintInfo>(
                    "[dbo].[uspGetComplaints]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result?.ToList() ?? new List<ComplaintInfo>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting complaints: {ex.Message}", ex);
            }
        }

        public async Task<ComplaintInfo> GetComplaintByIdAsync(Guid complaintId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ComplaintId", complaintId, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Get<ComplaintInfo>(
                    "[dbo].[uspGetComplaintById]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting complaint by ID: {ex.Message}", ex);
            }
        }

        public async Task<ComplaintInfo> CreateComplaintAsync(Guid complainedAgainstId, int complainedAgainstType, CreateComplaintRequest request, Guid? createdBy)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ComplainantId", request.ComplainantId, DbType.Guid);
                dp_params.Add("@ComplainantType", request.ComplainantType, DbType.Int32);
                dp_params.Add("@ComplainedAgainstId", complainedAgainstId, DbType.Guid);
                dp_params.Add("@ComplainedAgainstType", complainedAgainstType, DbType.Int32);
                dp_params.Add("@FranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@Title", request.Title, DbType.String);
                dp_params.Add("@Description", request.Description, DbType.String);
                dp_params.Add("@Category", request.Category, DbType.Int32);
                dp_params.Add("@Severity", request.Severity, DbType.Int32);
                dp_params.Add("@Status", request.Status, DbType.Int32);
                dp_params.Add("@CreatedBy", createdBy, DbType.Guid);
                dp_params.Add("@ComplaintId", dbType: DbType.Guid, direction: ParameterDirection.Output);

                var result = await Task.FromResult(_dapperRepository.Get<ComplaintInfo>(
                    "[dbo].[uspCreateComplaint]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating complaint: {ex.Message}", ex);
            }
        }

        public async Task<ComplaintInfo> UpdateComplaintAsync(UpdateComplaintRequest request, Guid? updatedBy)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ComplaintId", request.ComplaintId, DbType.Guid);
                dp_params.Add("@Title", request.Title, DbType.String);
                dp_params.Add("@Description", request.Description, DbType.String);
                dp_params.Add("@Category", request.Category, DbType.Int32);
                dp_params.Add("@Severity", request.Severity, DbType.Int32);
                dp_params.Add("@Status", request.Status, DbType.Int32);
                dp_params.Add("@Resolution", request.Resolution, DbType.String);
                dp_params.Add("@ResolvedBy", request.ResolvedBy, DbType.Guid);
                dp_params.Add("@UpdatedBy", updatedBy, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Get<ComplaintInfo>(
                    "[dbo].[uspUpdateComplaint]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating complaint: {ex.Message}", ex);
            }
        }

        public async Task<int> DeleteComplaintAsync(Guid complaintId, Guid? updatedBy)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@ComplaintId", complaintId, DbType.Guid);
                dp_params.Add("@UpdatedBy", updatedBy, DbType.Guid);

                var result = await Task.FromResult(_dapperRepository.Execute(
                    "[dbo].[uspDeleteComplaint]",
                    dp_params,
                    commandType: CommandType.StoredProcedure
                ));

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting complaint: {ex.Message}", ex);
            }
        }
    }
}

