using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseClient
    {
        private HttpClient httpClient;
        private FirebaseAuthOption firebaseAuthOption;

        public FirebaseClient(ILogger<FirebaseClient> logger, IConfiguration configuration, IHttpClientFactory httpClient, FirebaseAuthOption firebaseAuthOption)
        {
            this.firebaseAuthOption = firebaseAuthOption;
            this.httpClient = httpClient.CreateClient(DI.GOOGLE_HTTP_CLIENT);
            this.httpClient.BaseAddress = new Uri(this.firebaseAuthOption.FirebaseDbUrl);
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
}
