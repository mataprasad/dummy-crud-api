using System.Linq;
using System.Collections.Generic;
using SqlKata;
using Dapper;
using DbContextProvider;

namespace DummyCrudApi.Services
{
    public static class DbContextExtentions
    {
        public static IEnumerable<T> PagedList<T>(this IDbContext dbContext, string tableName, int pageSize, int pageNo)
        {
            var offset = (pageNo - 1) * pageSize;
            var query = new Query(tableName).Offset(offset).Limit(pageSize);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Query<T>(queryInfo) ?? new List<T>();
        }

        public static T Single<T>(this IDbContext dbContext, string tableName, object id, string idColumn = "id")
        {
            var query = new Query(tableName).Where(idColumn, id);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return (dbContext.Query<T>(queryInfo) ?? new List<T>()).FirstOrDefault();
        }

        public static bool Delete(this IDbContext dbContext, string tableName, object id, string idColumn = "id")
        {
            var query = new Query(tableName).Where(idColumn, id).AsDelete();
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Execute(queryInfo) > 0;
        }

        public static bool Insert(this IDbContext dbContext, string tableName, object data)
        {
            var query = new Query(tableName).AsInsert(data);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Execute(queryInfo) > 0;
        }

        public static bool Update(this IDbContext dbContext, string tableName, object data, object id, string idColumn = "id")
        {
            var query = new Query(tableName).Where(idColumn, id).AsUpdate(data);
            var queryInfo = dbContext.QueryInfo(query, tableName);
            return dbContext.Execute(queryInfo) > 0;
        }
    }
}
