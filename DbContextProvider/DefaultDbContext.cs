using System;
using System.Collections.Generic;
using System.Data;
using SqlKata;
using SqlKata.Compilers;
using Dapper;

namespace DbContextProvider
{
    public class DefaultDbContext : IDbContext
    {
        private Compiler compiler;
        private Func<IDbConnection> dbConnection;
        private Dictionary<string, string> globalSetting;

        public DefaultDbContext(Func<IDbConnection> dbConnection, Compiler compiler)
        {
            this.compiler = compiler;
            this.dbConnection = dbConnection;
            this.globalSetting = new Dictionary<string, string>();
        }

        public string Tag => "DefaultDbContext";

        public IDictionary<string, string> GlobalSetting => this.globalSetting;

        public bool IsDbExists
        {
            get
            {
                using (var db = dbConnection())
                {
                    return db.ExecuteScalar<int>("select 1 ") > 0;
                }
            }
        }

        public QueryInfo QueryInfo(Query query, string tableName)
        {
            return new QueryInfo(compiler.Compile(query));
        }

        public int Execute(QueryInfo queryInfo, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.Execute(queryInfo.Query, queryInfo.ToDapperDynamicParameters(), transaction, commandTimeout, commandType);
            }
        }

        public IEnumerable<T> Query<T>(QueryInfo queryInfo, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.Query<T>(queryInfo.Query, queryInfo.ToDapperDynamicParameters(), transaction, buffered, commandTimeout, commandType);
            }
        }

        public object ExecuteScalar(QueryInfo queryInfo, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.ExecuteScalar(queryInfo.Query, queryInfo.ToDapperDynamicParameters(), transaction, commandTimeout, commandType);
            }
        }

        public IEnumerable<dynamic> Query(QueryInfo queryInfo, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.Query<dynamic>(queryInfo.Query, queryInfo.ToDapperDynamicParameters(), transaction, buffered, commandTimeout, commandType);
            }
        }

        public IEnumerable<T> QueryRaw<T>(string query)
        {
            using (var db = dbConnection())
            {
                return db.Query<T>(query);
            }
        }

        public string QuerySingleRaw(string query)
        {
            using (var db = dbConnection())
            {
                return db.QuerySingle<string>(query);
            }
        }
    }

    public static class DefaultDbContextExt
    {
        public static DynamicParameters ToDapperDynamicParameters(this QueryInfo queryInfo)
        {
            return new DynamicParameters(queryInfo.Params);
        }
    }
}
