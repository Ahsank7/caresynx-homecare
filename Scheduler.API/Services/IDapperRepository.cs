using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SqlMapper = Dapper.SqlMapper;

namespace Scheduler.API.Services
{
    public interface IDapperRepository
    {
        //T execute_sp<T>(string query, DynamicParameters sp_params, CommandType commandType = CommandType.StoredProcedure);
        //List<T> GetAll<T>(string query, DynamicParameters sp_params, CommandType commandType = CommandType.StoredProcedure);

        // Synchronous methods (keeping for backward compatibility)
        IEnumerable<T> GetList<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        T Get<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        ValueTuple<List<T>, int> GetAll<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        int Execute(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        T Insert<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        T Update<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        TResult Query<TFirst, TSecond, TResult>(string sp, Func<TFirst, List<TSecond>, TResult> map,
        DynamicParameters param,
        CommandType? commandType = null
    );
        List<T> Query<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);

        // Asynchronous methods (new implementation)
        Task<IEnumerable<T>> GetListAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<(List<T>, int)> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<T> InsertAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<T> UpdateAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<TResult> QueryAsync<TFirst, TSecond, TResult>(string sp, Func<TFirst, List<TSecond>, TResult> map,
        DynamicParameters param,
        CommandType? commandType = null
    );
        Task<List<T>> QueryAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        
        // Multiple result sets support
        SqlMapper.GridReader GetMultiple(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<SqlMapper.GridReader> GetMultipleAsync(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
    }
}

