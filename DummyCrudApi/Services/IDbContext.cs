using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using SqlKata;

namespace DummyCrudApi.Services
{
    public interface IDbContext
    {
        string Tag { get; }
        QueryInfo QueryInfo(Query query);
        int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        DataTable ReadMetaData(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    }

    public class QueryInfo
    {
        public SqlResult SqlResult { get; set; }

        public QueryInfo(SqlResult sqlResult)
        {
            this.SqlResult = sqlResult;
        }

        public string Query
        {
            get
            {
                return this.SqlResult.Sql;
            }
        }

        public DynamicParameters Params
        {
            get
            {
                return new DynamicParameters(this.SqlResult.NamedBindings);
            }
        }
    }
}
