using Dapper;
using Scheduler.API.Models.Franchise;
using Scheduler.API.Models.Organization;
using Scheduler.API.Services.Security;
using System.Data;

namespace Scheduler.API.Services.Franchise
{
    public class FranchiseRepository : IFranchise
    {
        IDapperRepository _dapperRepository = null;
        ICrypto _crypto = null;
        
        public FranchiseRepository(IDapperRepository DapperRepository, ICrypto crypto)
        {
            _dapperRepository = DapperRepository;
            _crypto = crypto;
        }

        public async Task<Guid?> CreateOrUpdateFranchiseAsync(AddOrUpdateFranchiseViewModel saveFranchiseViewModel)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pName", saveFranchiseViewModel.Name, DbType.String);
                dp_params.Add("@pDescription", saveFranchiseViewModel.Description, DbType.String);
                dp_params.Add("@pLogo", saveFranchiseViewModel.logo, DbType.String);
                dp_params.Add("@pOrganizationId", saveFranchiseViewModel.OrganizationId, DbType.Guid);
                dp_params.Add("@pUserId", saveFranchiseViewModel.UserId, DbType.Guid);
                dp_params.Add("@pId", saveFranchiseViewModel.Id, DbType.Guid);
                dp_params.Add("@pIsActive", saveFranchiseViewModel.IsActive ?? true, DbType.Boolean);
                dp_params.Add("@pOutId", null, DbType.Guid, direction: ParameterDirection.Output);
                var result = await Task.FromResult(_dapperRepository.Insert<Guid?>("[Franchise].[InsertUpdateFranchise]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));


                return saveFranchiseViewModel.Id = dp_params.Get<Guid>("@pOutId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Guid?> CreateFranchiseAdminUserAsync(CreateFranchiseAdminUserViewModel model)
        {
            try
            {
                // Generate username: franchise name + "admin" (e.g., "kfcadmin")
                var sanitizedFranchiseName = model.FranchiseName.Replace(" ", "").ToLower();
                var userName = $"{sanitizedFranchiseName}admin";
                
                // Generate password: franchise name + "admin1234" (e.g., "kfcadmin1234")
                var plainPassword = $"{sanitizedFranchiseName}admin1234";
                
                // Encrypt the password
                var encryptedPassword = _crypto.Encrypt(plainPassword);
                
                var dp_params = new DynamicParameters();
                dp_params.Add("@pFranchiseId", model.FranchiseId, DbType.Guid);
                dp_params.Add("@pFranchiseName", model.FranchiseName, DbType.String);
                dp_params.Add("@pOrganizationName", model.OrganizationName, DbType.String);
                dp_params.Add("@pOrganizationId", model.OrganizationId, DbType.Guid);
                dp_params.Add("@pUserName", userName, DbType.String);
                dp_params.Add("@pPassword", encryptedPassword, DbType.String);
                dp_params.Add("@pOutUserId", null, DbType.Guid, direction: ParameterDirection.Output);
                
                var result = await Task.FromResult(_dapperRepository.Insert<Guid?>("[Franchise].[CreateFranchiseAdminUser]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return dp_params.Get<Guid?>("@pOutUserId");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public Guid DeleteFranchise(Guid id)
        {
            var dp_params = new DynamicParameters();
            dp_params.Add("@pFranchiseId", id, DbType.Guid);
            var result = _dapperRepository.Update<Guid>("[Franchise].[DeleteFranchise]"
                , dp_params,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public Task<FranchiseInfo> GetFranchiseInfoByIdAsync(Guid franchiseId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FranchiseInfo>> GetFranchisesByOrganizationIdAsync(Guid organizationId)
        {
            List<FranchiseInfo> franchiseInfos = new List<FranchiseInfo>(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetAll<FranchiseInfo>("[Franchise].[uspGetFranchisesByOranizationId]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            franchiseInfos = result.Item1;

            return franchiseInfos;
        }

        public async Task<List<FranchiseInfo>> GetFranchisesByOrganizationIdAsync(Guid organizationId, Guid userId)
        {
            List<FranchiseInfo> franchiseInfos = new List<FranchiseInfo>(); ;

            var dp_params = new DynamicParameters();
            dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);
            dp_params.Add("@pUserId", userId, DbType.Guid);
            var result = await Task.FromResult(_dapperRepository.GetAll<FranchiseInfo>("[Franchise].[uspGetFranchisesByOranizationIdandUserId]"
                , dp_params,
                commandType: CommandType.StoredProcedure));

            franchiseInfos = result.Item1;

            return franchiseInfos;
        }

        public async Task<FranchiseDashboardResponse> GetFranchiseDashboardDataAsync(Guid franchiseId, DateTime startDate, DateTime endDate)
        {
            SqlMapper.GridReader result = null;
            
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pFranchiseId", franchiseId, DbType.Guid);
                dp_params.Add("@pStartDate", startDate, DbType.Date);
                dp_params.Add("@pEndDate", endDate, DbType.Date);

                Console.WriteLine($"Fetching dashboard data for franchise: {franchiseId}");
                Console.WriteLine($"Date range: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

                // Execute the stored procedure and get all result sets
                result = await Task.FromResult(_dapperRepository.GetMultiple("[dbo].[uspGetFranchiseDashboardData]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                var dashboardData = new FranchiseDashboardResponse();

                // Get stats (first result set)
                var stats = result.Read<DashboardStats>().FirstOrDefault();
                if (stats != null)
                {
                    dashboardData.Stats = stats;
                    Console.WriteLine($"Stats loaded: Clients={stats.TotalClients}, Providers={stats.TotalServiceProviders}, Staff={stats.TotalStaff}, Tasks={stats.TotalTasks}");
                }
                else
                {
                    Console.WriteLine("No stats data returned");
                }

                // Get popular services (second result set)
                var popularServices = result.Read<ServiceTypeData>().ToList();
                dashboardData.PopularServices = popularServices;
                Console.WriteLine($"Popular services loaded: {popularServices.Count} items");

                // Get service types (third result set)
                var serviceTypes = result.Read<ServiceTaskData>().ToList();
                dashboardData.ServiceTaskStatuses = serviceTypes;
                Console.WriteLine($"Service types loaded: {serviceTypes.Count} items");

                // Get task status distribution (fourth result set)
                var taskStatusDistribution = result.Read<TaskStatusData>().ToList();
                dashboardData.TaskStatusDistribution = taskStatusDistribution;
                Console.WriteLine($"Task status distribution loaded: {taskStatusDistribution.Count} items");

                // Get billing trend (fifth result set)
                var billingTrend = result.Read<BillingWageData>().ToList();
                dashboardData.BillingTrend = billingTrend;
                Console.WriteLine($"Billing trend loaded: {billingTrend.Count} items");

                // Get wage trend (sixth result set)
                var wageTrend = result.Read<BillingWageData>().ToList();
                dashboardData.WageTrend = wageTrend;
                Console.WriteLine($"Wage trend loaded: {wageTrend.Count} items");

                // Get billing summary (seventh result set)
                var billingSummary = result.Read<BillingWageSummary>().FirstOrDefault();
                if (billingSummary != null)
                {
                    dashboardData.BillingSummary = billingSummary;
                    Console.WriteLine($"Billing summary loaded: Total={billingSummary.TotalCount}, Paid={billingSummary.PaidCount}, Unpaid={billingSummary.UnpaidCount}");
                }
                else
                {
                    Console.WriteLine("No billing summary data returned");
                }

                // Get wage summary (eighth result set)
                var wageSummary = result.Read<BillingWageSummary>().FirstOrDefault();
                if (wageSummary != null)
                {
                    dashboardData.WageSummary = wageSummary;
                    Console.WriteLine($"Wage summary loaded: Total={wageSummary.TotalCount}, Paid={wageSummary.PaidCount}, Unpaid={wageSummary.UnpaidCount}");
                }
                else
                {
                    Console.WriteLine("No wage summary data returned");
                }

                Console.WriteLine("Dashboard data loading completed successfully");
                return dashboardData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Log the exception
                return new FranchiseDashboardResponse();
            }
            finally
            {
                // Properly dispose of resources
                if (result != null)
                {
                    result.Dispose();
                }
            }
        }

        public async Task<List<UserFranchiseAssignmentViewModel>> GetUserFranchiseAssignmentsAsync(Guid userId, Guid organizationId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@pUserId", userId, DbType.Guid);
                dp_params.Add("@pOrganizationId", organizationId, DbType.Guid);
                
                var result = await Task.FromResult(_dapperRepository.GetAll<UserFranchiseAssignmentViewModel>("[dbo].[uspGetUserFranchiseAssignments]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return result.Item1 ?? new List<UserFranchiseAssignmentViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user franchise assignments: {ex.Message}");
                return new List<UserFranchiseAssignmentViewModel>();
            }
        }

        public async Task<bool> AssignUserToFranchiseAsync(AssignUserFranchiseRequest request)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@UserId", request.UserId, DbType.Guid);
                dp_params.Add("@FranchiseId", request.FranchiseId, DbType.Guid);
                dp_params.Add("@IsActive", request.IsActive, DbType.Boolean);
                
                await Task.FromResult(_dapperRepository.Execute("[dbo].[UpsertUserFranchiseStatus]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error assigning user to franchise: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveUserFromFranchiseAsync(Guid userId, Guid franchiseId)
        {
            try
            {
                var dp_params = new DynamicParameters();
                dp_params.Add("@UserId", userId, DbType.Guid);
                dp_params.Add("@FranchiseId", franchiseId, DbType.Guid);
                dp_params.Add("@IsActive", false, DbType.Boolean);
                
                await Task.FromResult(_dapperRepository.Execute("[dbo].[UpsertUserFranchiseStatus]"
                    , dp_params,
                    commandType: CommandType.StoredProcedure));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing user from franchise: {ex.Message}");
                return false;
            }
        }
    }
}
