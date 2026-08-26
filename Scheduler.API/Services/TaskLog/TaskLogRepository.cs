using Dapper;
using Scheduler.API.Models.TaskLog;
using System.Data;

namespace Scheduler.API.Services.TaskLog
{
    public class TaskLogRepository : ITaskLog
    {
        private readonly IDapperRepository _dapperRepository;

        public TaskLogRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<TaskLogResponse> GetTaskLogsAsync(TaskLogRequest request)
        {
            var response = new TaskLogResponse();

            var dp_params = new DynamicParameters();
            dp_params.Add("@pTaskId", request.TaskId, DbType.Int32);
            dp_params.Add("@pPageNumber", request.PageNumber, DbType.Int32);
            dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);

            var result = await Task.FromResult(_dapperRepository.GetAll<TaskLogEntry>("[dbo].[GetTaskLogs]", dp_params, commandType: CommandType.StoredProcedure));

            response.Logs = result.Item1;
            response.TotalRecords = result.Item2;

            return response;
        }

        public async Task<int> InsertTaskLogAsync(int taskId, string actionType, string? previousValue = null, string? newValue = null, string? fieldName = null, string? description = null, Guid? createdBy = null)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pTaskId", taskId, DbType.Int32);
            dp_params.Add("@pActionType", actionType, DbType.String);
            dp_params.Add("@pPreviousValue", previousValue, DbType.String);
            dp_params.Add("@pNewValue", newValue, DbType.String);
            dp_params.Add("@pFieldName", fieldName, DbType.String);
            dp_params.Add("@pDescription", description, DbType.String);
            dp_params.Add("@pCreatedBy", createdBy, DbType.Guid);

            var result = await Task.FromResult(_dapperRepository.Insert<int>("[dbo].[InsertTaskLog]", dp_params, commandType: CommandType.StoredProcedure));
            
            return result;
        }
    }
}
