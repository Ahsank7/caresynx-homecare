using Dapper;
using Scheduler.API.Helper;
using Scheduler.API.Models.Document;
using Scheduler.API.Models.Expense;
using System.Data;

namespace Scheduler.API.Services.Document
{
    public class DocumentRepository : IDocument
    {
        IDapperRepository _dapperRepository = null;
        public DocumentRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public bool DeleteDocument(int Id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pId", Id, DbType.Int32);
                var result = _dapperRepository.Update<Guid>("[dbo].[DeleteDocument]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DocumentInfo> GeDocumentInfoByIdAsync(int Id)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pID", Id, DbType.Int32);
                var result = await Task.FromResult(_dapperRepository.GetList<DocumentInfo>("[dbo].[GetDocumentInfo]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DocumentInfo> GeDocumentInfoByUserIdAsync(Guid UserId, int documentTypeId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserID", UserId, DbType.Guid);
                dp_params.Add("@pDocumentType", documentTypeId, DbType.Int32);
                var result = await Task.FromResult(_dapperRepository.GetList<DocumentInfo>("[dbo].[GetDocumentInfoByUserId]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));
                return result.FirstOrDefault()!;

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<DocumentSearchResponse> GetUserDocumentsAsync(DocumentSearchRequest request)
        {
            try
            {
                DocumentSearchResponse response = new DocumentSearchResponse();

                var dp_params = new DynamicParameters();
                dp_params.Add("@pPageSize", request.PageSize, DbType.Int32);
                dp_params.Add("@pSortColumn", request.SortColumn, DbType.String);
                dp_params.Add("@pSortType", request.SortType, DbType.String);
                dp_params.Add("@pUserId", request.UserId, DbType.Guid);
                dp_params.Add("@pDocumentTypeId", request.DocumentTypeId, DbType.Int32);
                dp_params.Add("@PageNumber", request.PageNumber, DbType.Int32);
                var result = await Task.FromResult(_dapperRepository.GetAll<DocumentInfo>("[dbo].[uspGetUserDocuments]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                response.Response = result.Item1;
                response.TotalRecords = result.Item2;

                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<int> UploadDocumentAsync(DocumentUploadModel documentUploadModel)
        {
            try
            {
                var dp_params = new DynamicParameters();

                dp_params.Add("@pName", documentUploadModel.Name, DbType.String);
                dp_params.Add("@pDescription", documentUploadModel.Description, DbType.String);
                dp_params.Add("@pDocumentTypeId", documentUploadModel.DocumentTypeId, DbType.Int32);
                dp_params.Add("@pAccessRoles", documentUploadModel.AccessRoles, DbType.String);
                dp_params.Add("@pDocumentPath", documentUploadModel.DocumentPath, DbType.String);
                dp_params.Add("@pUserId", documentUploadModel.UserId, DbType.Guid);
                dp_params.Add("@pOutId", null, DbType.Int32, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<int>("[dbo].[UploadDocument]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return dp_params.Get<int>("@pOutId");
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
