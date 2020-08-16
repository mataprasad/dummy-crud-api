using System;
using System.Collections.Generic;
using System.Data;
using SqlKata;

namespace DbContextProvider
{
    public interface IDbContext
    {
        public const string CurrentUserSettingKey = "user";
        IDictionary<string, string> GlobalSetting { get; }
        bool IsDbExists { get; }
        string Tag { get; }
        void ResetDb();
        IEnumerable<T> PagedList<T>(string tableName, int pageSize, int pageNo);
        T Single<T>(string tableName, object id, string idColumn = "id");
        bool Delete(string tableName, object id, string idColumn = "id");
        bool Insert(string tableName, object data, object id, string idColumn = "id");
        bool Update(string tableName, object data, object id, string idColumn = "id");
    }
}
