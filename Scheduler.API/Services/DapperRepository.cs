using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SqlMapper = Dapper.SqlMapper;

namespace Scheduler.API.Services
{
    public class DapperRepository : IDapperRepository
    {
        private readonly IConfiguration _configuration;
        public DapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            return db.Execute(sp, parms, commandType: commandType);
        }

        public T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            return db.Query<T>(sp, parms, commandType: commandType).FirstOrDefault()!;
        }

        public (List<T>, int) GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            var multi = db.QueryMultiple(sp, parms, commandType: commandType);

            var data = multi.Read<T>().ToList();
            var totalrecords = multi.ReadFirst<int>();

            return (data, totalrecords);
        }

        public IEnumerable<T> GetList<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            var lst = db.Query<T>(sp, parms, commandType: commandType);

            return lst;
        }

        public T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        public TResult Query<TFirst, TSecond, TResult>(string storeProcedure, Func<TFirst, List<TSecond>, TResult> map,
    DynamicParameters param,
    CommandType? commandType = null
)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));

                var result = db.QueryMultiple(storeProcedure, param, commandType: commandType);
                var dataFirst = result.Read<TFirst>().FirstOrDefault();
                var dataSecond = result.Read<TSecond>().ToList();
                return map(dataFirst, dataSecond);
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                throw;
            }
        }

        public  List<T> Query<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            return db.Query<T>(sp, parms, commandType: commandType).ToList();

        }

        public T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            T result;
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            try
            {
                if (db.State == ConnectionState.Closed)
                    db.Open();

                using var tran = db.BeginTransaction();
                try
                {
                    result = db.Query<T>(sp, parms, commandType: commandType, transaction: tran).FirstOrDefault();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                    db.Close();
            }

            return result;
        }

        // Async implementations
        public async Task<int> ExecuteAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            return await db.ExecuteAsync(sp, parms, commandType: commandType);
        }

        public async Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            var result = await db.QueryAsync<T>(sp, parms, commandType: commandType);
            return result.FirstOrDefault()!;
        }

        public async Task<(List<T>, int)> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            using var multi = await db.QueryMultipleAsync(sp, parms, commandType: commandType);

            var data = (await multi.ReadAsync<T>()).ToList();
            var totalrecords = await multi.ReadFirstAsync<int>();

            return (data, totalrecords);
        }

        public async Task<IEnumerable<T>> GetListAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            return await db.QueryAsync<T>(sp, parms, commandType: commandType);
        }

        // Multiple result sets support
        public SqlMapper.GridReader GetMultiple(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            var db = new SqlConnection(_configuration.GetConnectionString("default"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            
            return db.QueryMultiple(sp, parms, commandType: commandType);
        }



        public async Task<SqlMapper.GridReader> GetMultipleAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using SqlConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            if (db.State == ConnectionState.Closed)
                await db.OpenAsync();
            
            return await db.QueryMultipleAsync(sp, parms, commandType: commandType);
        }

        public async Task<T> InsertAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using SqlConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            await db.OpenAsync();
            
            using var tran = await db.BeginTransactionAsync();
            try
            {
                var result = await db.QueryAsync<T>(sp, parms, commandType: commandType, transaction: tran);
                await tran.CommitAsync();
                return result.FirstOrDefault();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<TResult> QueryAsync<TFirst, TSecond, TResult>(string storeProcedure, Func<TFirst, List<TSecond>, TResult> map,
            DynamicParameters param, CommandType? commandType = null)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            using var result = await db.QueryMultipleAsync(storeProcedure, param, commandType: commandType);
            
            var dataFirst = (await result.ReadAsync<TFirst>()).FirstOrDefault();
            var dataSecond = (await result.ReadAsync<TSecond>()).ToList();
            return map(dataFirst, dataSecond);
        }

        public async Task<List<T>> QueryAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            var result = await db.QueryAsync<T>(sp, parms, commandType: commandType);
            return result.ToList();
        }

        public async Task<T> UpdateAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using SqlConnection db = new SqlConnection(_configuration.GetConnectionString("default"));
            await db.OpenAsync();
            
            using var tran = await db.BeginTransactionAsync();
            try
            {
                var result = await db.QueryAsync<T>(sp, parms, commandType: commandType, transaction: tran);
                await tran.CommitAsync();
                return result.FirstOrDefault();
            }
            catch (Exception)
            {
                await tran.RollbackAsync();
                throw;
            }
        }


    }
}
