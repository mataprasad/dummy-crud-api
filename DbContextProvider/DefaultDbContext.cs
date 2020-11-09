using System;
using System.Collections.Generic;
using System.Data;
using SqlKata;
using SqlKata.Compilers;
using Dapper;
using System.Linq;

namespace DbContextProvider
{
    public class DefaultDbContext : IDbContext
    {
        private Dictionary<string, string> globalSetting;
        private SqlHelper dbContext;

        public DefaultDbContext(Func<IDbConnection> dbConnection, Compiler compiler)
        {
            this.globalSetting = new Dictionary<string, string>();
            this.dbContext = new SqlHelper(dbConnection,compiler);
        }

        public string Tag => "DefaultDbContext";

        public IDictionary<string, string> GlobalSetting => this.globalSetting;

        public bool IsDbExists
        {
            get
            {
                var queryInfo = new QueryInfo(new SqlResult() { Sql = "SELECT 1" });
                return Convert.ToInt32(dbContext.ExecuteScalar(queryInfo)) > 0;
            }
        }

        public bool Delete(string tableName, object id, string idColumn = "id")
        {
            var query = new Query(tableName).Where(idColumn, id).AsDelete();
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Execute(queryInfo) > 0;
        }

        public bool Insert(string tableName, object data, object id, string idColumn = "id")
        {
            var query = new Query(tableName).AsInsert(data);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Execute(queryInfo) > 0;
        }

        public IEnumerable<T> PagedList<T>(string tableName, int pageSize, int pageNo)
        {
            var offset = (pageNo - 1) * pageSize;
            var query = new Query(tableName).Offset(offset).Limit(pageSize);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Query<T>(queryInfo) ?? new List<T>();
        }

        public IEnumerable<T> PagedListWithCount<T>(string tableName, int pageSize, int pageNo, out long totalCount)
        {
            var query = new Query(tableName).AsCount();
            var queryInfo = dbContext.QueryInfo(query, tableName);
            totalCount = Convert.ToInt64(dbContext.ExecuteScalar(queryInfo));
            return PagedList<T>(tableName, pageSize, pageNo);
        }

        public void ResetDb()
        {
        }

        public T Single<T>(string tableName, object id, string idColumn = "id")
        {
            var query = new Query(tableName).Where(idColumn, id);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return (dbContext.Query<T>(queryInfo) ?? new List<T>()).FirstOrDefault();
        }

        public bool Update(string tableName, object data, object id, string idColumn = "id")
        {
            var query = new Query(tableName).Where(idColumn, id).AsUpdate(data);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Execute(queryInfo) > 0;
        }
    }

    public class SqlHelper
    {
        private Compiler compiler;
        private Func<IDbConnection> dbConnection;

        public SqlHelper(Func<IDbConnection> dbConnection, Compiler compiler)
        {
            this.compiler = compiler;
            this.dbConnection = dbConnection;
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
