using System;
using System.Collections.Generic;
using Castle.DynamicProxy;
using RestSharp;
using Retrofit;
using Retrofit.Net.Attributes.Methods;
using Retrofit.Net.Attributes.Parameters;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public interface FirebaseClient
    {
        public const string FIREBASE_DB_URL_CONFIG_KEY = "FirebaseDbUrl";

        [Get("/{user}/{table}.json")]
        RestResponse<List<T>> Get<T>([Path("user")] string user, [Path("table")] string table);

        [Get("/{user}/{table}.json")]
        RestResponse<T> GetSingle<T>([Path("user")] string user, [Path("table")] string table);

        RestResponse Raw(IRestRequest rest);
    }

    public class FirebaseCallInterceptor : IHttpMiddleware
    {
        private GoogleAuthHelper googleAuthHelper;

        public FirebaseCallInterceptor(GoogleAuthHelper googleAuthHelper)
        {
            this.googleAuthHelper = googleAuthHelper;
        }

        public IInvocation After(IInvocation restRequest)
        {
            return restRequest;
        }

        public IRestRequest Before(IRestRequest restRequest)
        {
            var token = this.googleAuthHelper?.GetAccessToken();
            if (!String.IsNullOrWhiteSpace(token))
            {
                restRequest.AddHeader("Authorization", String.Concat("Bearer ", token));
            }
            return restRequest;
        }
    }
}
