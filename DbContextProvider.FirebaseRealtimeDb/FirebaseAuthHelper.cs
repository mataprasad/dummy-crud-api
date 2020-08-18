using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseAuthHelper
    {
        private const string CACHE_KEY = "FirebaseAuthHelperAccessToken";
        private MemoryCache memory;
        private HttpClient httpClient;
        public FirebaseAuthOption firebaseAuthOption { get; }

        public FirebaseAuthHelper(MemoryCache memory, IHttpClientFactory httpClientFactory, FirebaseAuthOption firebaseAuthOption)
        {
            this.firebaseAuthOption = firebaseAuthOption;
            httpClient = httpClientFactory.CreateClient();
            this.memory = memory;
        }

        public string GetAccessToken()
        {
            var token = memory.Get<string>(CACHE_KEY);
            if (string.IsNullOrWhiteSpace(token))
            {
                token = GetFirebaseAccessToken();
                memory.Set(CACHE_KEY, token, new DateTimeOffset(DateTime.UtcNow.AddMinutes(55)));
            }
            return token;
        }

        private string GetFirebaseAccessToken()
        {
            var url = string.Concat(firebaseAuthOption.VerifyPasswordUrl,firebaseAuthOption.ApiKey);
            var authRequest = new HttpRequestMessage(HttpMethod.Post, url);
            var payload = new JObject();
            payload.Add("email", firebaseAuthOption.Email);
            payload.Add("password", firebaseAuthOption.Password);
            payload.Add("returnSecureToken", true);
            authRequest.Content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
            
            var authResponse = httpClient.SendAsync(authRequest).Result;
            authResponse.EnsureSuccessStatusCode();
            var body = JObject.Parse(authResponse.Content.ReadAsStringAsync().Result);
            return body["idToken"].ToString();
        }
    }

    public class RefreshAndSetFirebaseAuthToken : DelegatingHandler
    {
        private FirebaseAuthHelper firebaseAuthHelper;

        public RefreshAndSetFirebaseAuthToken(FirebaseAuthHelper firebaseAuthHelper)
        {
            this.firebaseAuthHelper = firebaseAuthHelper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = this.firebaseAuthHelper?.GetAccessToken();
            if (!String.IsNullOrWhiteSpace(token))
            {
                if (String.Equals(request.RequestUri.Host, new Uri(this.firebaseAuthHelper.firebaseAuthOption.FirebaseDbUrl).Host, StringComparison.OrdinalIgnoreCase))
                {
                    request.RequestUri = new Uri(request.RequestUri.OriginalString + (String.IsNullOrWhiteSpace(request.RequestUri.Query) ? "?auth=" : "&auth=") + token);
                }
                else
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Firebase", token);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

    public class FirebaseAuthOption
    {
        public string VerifyPasswordUrl { get; set; }
        public string ApiKey { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirebaseDbUrl { get; set; }
        public string Bucket { get; set; }
        public string FirebaseStorageEndpoint { get; set; }
        public string ServiceAccountCredFileFullPath { get; set; }
    }
}
