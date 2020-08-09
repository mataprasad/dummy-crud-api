using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Fdb=Firebase.Database;
using RestSharp;
using SqlKata;
using Firebase.Database.Query;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseDbContext : IDbContext
    {
        private FirebaseClientFactory firebaseClientFactory;
        private FirebaseClient firebaseClient;
        private Dictionary<string, string> globalSetting;

        public FirebaseDbContext(FirebaseClientFactory firebaseClientFactory)
        {
            this.firebaseClientFactory = firebaseClientFactory;
            this.firebaseClient = this.firebaseClientFactory.Get();
            this.globalSetting = new Dictionary<string, string>();
        }

        public string Tag => "FirebaseDbContext";

        public IDictionary<string, string> GlobalSetting => this.globalSetting;

        public bool IsDbExists
        {
            get
            {
                var request = new RestRequest(String.Format("/{0}.json?shallow=true", globalSetting["user"]), Method.GET);
                var resp = firebaseClient.Raw(request);
                return resp != null && JToken.Parse(resp.Content).HasValues;
            }
        }

        public QueryInfo QueryInfo(Query query, string tableName)
        {
            var qInfo = new QueryInfo(new SqlResult());
            qInfo.TableName = tableName.NormalizeAsFirebaseDbKey();
            return qInfo;
        }

        public int Execute(QueryInfo queryInfo, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar(QueryInfo queryInfo, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Query<T>(QueryInfo queryInfo, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {

            var firebase = new Fdb.FirebaseClient("");
            var dinos =firebase
   .Child("dinosaurs")
   .OrderByKey()
   .StartAt("pterodactyl")
   .LimitToFirst(2).BuildUrlAsync().Result;



            var data = firebaseClient.Get<T>(GlobalSetting[IDbContext.CurrentUserSettingKey], queryInfo.TableName);
            return data.Data;
        }

        public IEnumerable<dynamic> Query(QueryInfo queryInfo, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            var data = firebaseClient.Get<dynamic>(GlobalSetting[IDbContext.CurrentUserSettingKey], queryInfo.TableName);
            return data.Data;
        }

        public IEnumerable<T> QueryRaw<T>(string query)
        {
            var request = new RestRequest(query, Method.GET);
            var resp = firebaseClient.Raw(request);
            return JsonConvert.DeserializeObject<List<T>>(resp.Content);
        }

        public string QuerySingleRaw(string query)
        {
            var request = new RestRequest(query, Method.GET);

            return firebaseClient.Raw(request)?.Content;
        }
    }

    public static class FirebaseDbContextExt
    {
        public static string NormalizeAsFirebaseDbKey(this string str)
        {
            return str?.Trim().ToLower();
        }
    }
}
