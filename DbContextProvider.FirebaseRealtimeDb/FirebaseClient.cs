using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseClient
    {
        public const string FIREBASE_DB_URL_CONFIG_KEY = "FirebaseDbUrl";
        public const string FIREBASE_HTTP_CLIENT_NAME = "FIREBASE_HTTP_CLIENT";
        private HttpClient httpClient;

        public FirebaseClient(IConfiguration configuration, IHttpClientFactory httpClient)
        {
            this.httpClient = httpClient.CreateClient(FIREBASE_HTTP_CLIENT_NAME);
            this.httpClient.BaseAddress = new Uri(configuration[FIREBASE_DB_URL_CONFIG_KEY]);
        }

        public string Get(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var resp = this.httpClient.SendAsync(request).Result;
            resp?.EnsureSuccessStatusCode();
            return resp?.Content?.ReadAsStringAsync().Result;
        }

        public IEnumerable<T> Get<T>(string url, bool isList = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            return this.httpClient.SendAsync<T>(request, isList).Result;
        }

        public HttpResponseMessage Execute(HttpMethod method, string url, object body = null)
        {
            var request = new HttpRequestMessage(method, url);
            if (body != null)
            {
                var json = JsonConvert.SerializeObject(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            var resp = this.httpClient.SendAsync(request).Result;
            resp?.EnsureSuccessStatusCode();
            return resp;
        }
    }

    public class RefreshAndSetFirebaseAuthToken : DelegatingHandler
    {
        private GoogleAuthHelper googleAuthHelper;

        public RefreshAndSetFirebaseAuthToken(GoogleAuthHelper googleAuthHelper)
        {
            this.googleAuthHelper = googleAuthHelper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = this.googleAuthHelper?.GetAccessToken();
            if (!String.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
