using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseDbContext : IDbContext
    {
        private FirebaseClient firebaseClient;
        private Dictionary<string, string> globalSetting;

        public FirebaseDbContext(FirebaseClient firebaseClient)
        {
            this.firebaseClient = firebaseClient;
            this.globalSetting = new Dictionary<string, string>();
        }

        public string Tag => "FirebaseDbContext";

        public IDictionary<string, string> GlobalSetting => this.globalSetting;

        public bool IsDbExists
        {
            get
            {
                var resp = this.firebaseClient.Get($"/{globalSetting["user"]}.json?shallow=true");
                return resp != null && JToken.Parse(resp).HasValues;
            }
        }

        public IEnumerable<T> PagedList<T>(string tableName, int pageSize, int pageNo)
        {
            return this.firebaseClient.Get<T>($"/{globalSetting["user"]}/{tableName.AsFirebaseNodeKey()}.json", true);
        }

        public T Single<T>(string tableName, object id, string idColumn = "id")
        {
            return this.firebaseClient.Get<T>($"/{globalSetting["user"]}/{tableName.AsFirebaseNodeKey()}/{id}.json").FirstOrDefault();
        }

        public bool Delete(string tableName, object id, string idColumn = "id")
        {
            var resp = this.firebaseClient.Execute(HttpMethod.Delete, $"/{globalSetting["user"]}/{tableName.AsFirebaseNodeKey()}/{id}.json");
            return resp.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public bool Insert(string tableName, object data, object id, string idColumn = "id")
        {
            var resp = this.firebaseClient.Execute(HttpMethod.Post, $"/{globalSetting["user"]}/{tableName.AsFirebaseNodeKey()}.json", data);
            var body = JToken.Parse(resp.Content.ReadAsStringAsync().Result);
            var name = body["name"];
            var updatedData = JObject.FromObject(data);
            updatedData["_key"] = name;
            var yes = Update(tableName, updatedData, name, idColumn);
            return resp.StatusCode == System.Net.HttpStatusCode.OK && yes;
        }

        public bool Update(string tableName, object data, object id, string idColumn = "id")
        {
            var resp = this.firebaseClient.Execute(HttpMethod.Put, $"/{globalSetting["user"]}/{tableName.AsFirebaseNodeKey()}/{id}.json", data);
            return resp.StatusCode == System.Net.HttpStatusCode.OK;
        }
    }

    public static class FirebaseDbContextExt
    {
        public static string AsFirebaseNodeKey(this string str)
        {
            return str?.Trim().ToLower();
        }
    }
}
