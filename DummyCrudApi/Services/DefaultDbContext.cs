using System;
using System.Collections.Generic;
using System.Data;
using SqlKata;
using SqlKata.Compilers;
using Dapper;

namespace DummyCrudApi.Services
{
    public class DefaultDbContext : IDbContext
    {
        private Compiler compiler;
        private Func<IDbConnection> dbConnection;

        public DefaultDbContext(Func<IDbConnection> dbConnection, Compiler compiler)
        {
            this.compiler = compiler;
            this.dbConnection = dbConnection;
        }

        public string Tag => "DefaultDbContext";

        public QueryInfo QueryInfo(Query query)
        {
            return new QueryInfo(compiler.Compile(query));
        }

        public int Execute(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.Execute(sql, param, transaction, commandTimeout, commandType);
            }
        }

        public IEnumerable<T> Query<T>(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.Query<T>(sql, param, transaction, buffered, commandTimeout, commandType);
            }
        }

        public object ExecuteScalar(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.ExecuteScalar(sql, param, transaction, commandTimeout, commandType);
            }
        }

        public DataTable ReadMetaData(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                using (var dr = db.ExecuteReader(sql, param, transaction, commandTimeout, commandType))
                {
                    return dr.GetSchemaTable();
                }
            }
        }

        public IEnumerable<dynamic> Query(string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            using (var db = dbConnection())
            {
                return db.Query<dynamic>(sql, param, transaction, buffered, commandTimeout, commandType);
            }
        }
    }
}
