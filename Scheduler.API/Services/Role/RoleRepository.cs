using Dapper;
using Scheduler.API.Models.Role;
using Scheduler.API.Common;
using System.Data;

namespace Scheduler.API.Services.Role
{
    public class RoleRepository : IRole
    {
        private readonly IDapperRepository _dapperRepository;

        public RoleRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<List<RoleInfo>> GetAvailableRolesAsync(Guid? organizationId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pOrganizationId", organizationId);

            var result = await _dapperRepository.QueryAsync<RoleInfo>(
                "GetAvailableRoles",
                parameters,
                CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<RoleInfo>();
        }

        public async Task<List<RoleInfo>> GetAvailableRolesForUserAsync(Guid? organizationId, Guid? currentUserId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pOrganizationId", organizationId);
            parameters.Add("@pCurrentUserId", currentUserId);

            var result = await _dapperRepository.QueryAsync<RoleInfo>(
                "GetAvailableRolesForUser",
                parameters,
                CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<RoleInfo>();
        }

        public async Task<RoleInfo> GetRoleByIdAsync(int roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pRoleId", roleId);

            var result = await _dapperRepository.QueryAsync<RoleInfo>(
                "SELECT * FROM tblRole WHERE Id = @pRoleId AND IsActive = 1",
                parameters,
                CommandType.Text
            );

            return result?.FirstOrDefault();
        }

        public async Task<bool> CreateRoleAsync(RoleInfo role)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pName", role.Name);
            parameters.Add("@pDescription", role.Description);
            parameters.Add("@pOrganizationId", role.OrganizationId);
            parameters.Add("@pCreatedBy", role.CreatedBy);

            var sql = @"
                INSERT INTO tblRole (Name, Description, OrganizationId, IsActive, CreatedDate, CreatedBy)
                VALUES (@pName, @pDescription, @pOrganizationId, 1, GETUTCDATE(), @pCreatedBy)";

            var result = await _dapperRepository.ExecuteAsync(sql, parameters, CommandType.Text);
            return result > 0;
        }

        public async Task<bool> UpdateRoleAsync(RoleInfo role)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pId", role.Id);
            parameters.Add("@pName", role.Name);
            parameters.Add("@pDescription", role.Description);
            parameters.Add("@pOrganizationId", role.OrganizationId);
            parameters.Add("@pUpdatedBy", role.UpdatedBy);

            var sql = @"
                UPDATE tblRole 
                SET Name = @pName, 
                    Description = @pDescription, 
                    OrganizationId = @pOrganizationId,
                    UpdatedDate = GETUTCDATE(),
                    UpdatedBy = @pUpdatedBy
                WHERE Id = @pId AND IsActive = 1";

            var result = await _dapperRepository.ExecuteAsync(sql, parameters, CommandType.Text);
            return result > 0;
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pRoleId", roleId);

            var sql = "UPDATE tblRole SET IsActive = 0 WHERE Id = @pRoleId";

            var result = await _dapperRepository.ExecuteAsync(sql, parameters, CommandType.Text);
            return result > 0;
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, int roleId, Guid? createdBy = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pUserId", userId);
            parameters.Add("@pRoleId", roleId);
            parameters.Add("@pCreatedBy", createdBy);

            var result = await _dapperRepository.ExecuteAsync(
                "ManageUserRole",
                parameters,
                CommandType.StoredProcedure
            );

            return result > 0;
        }
    }
}
