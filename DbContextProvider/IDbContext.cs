using System;
using System.Collections.Generic;
using System.Data;
using SqlKata;

namespace DbContextProvider
{
    public interface IDbContext
    {
        public const string CurrentUserSettingKey = "user";
        IDictionary<string,string> GlobalSetting { get; }
        bool IsDbExists { get; }
        string Tag { get; }
        QueryInfo QueryInfo(Query query, string tableName);
        int Execute(QueryInfo queryInfo, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<T> Query<T>(QueryInfo queryInfo, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
        object ExecuteScalar(QueryInfo queryInfo, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null);
        IEnumerable<T> QueryRaw<T>(string query);
        string QuerySingleRaw(string query);
        IEnumerable<dynamic> Query(QueryInfo queryInfo, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
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

        public string TableName { get; set; }

        public Dictionary<string, object> Params => this.SqlResult.NamedBindings;
    }
}
