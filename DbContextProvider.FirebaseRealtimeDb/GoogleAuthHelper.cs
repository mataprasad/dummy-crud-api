using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Caching.Memory;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class GoogleAuthHelper
    {
        private const string CACHE_KEY = "GoogleAuthHelperAccessToken";
        private MemoryCache memory;
        private GoogleCredential googleCredential;

        public GoogleAuthHelper(GoogleCredential googleCredential, MemoryCache memory)
        {
            this.googleCredential = googleCredential;
            this.memory = memory;// new MemoryCache(new MemoryCacheOptions());
        }

        public string GetAccessToken()
        {
            var token = memory.Get<string>(CACHE_KEY);
            if(string.IsNullOrWhiteSpace(token))
            {
                token = GetFirebaseAccessToken();
                memory.Set(CACHE_KEY, token, new DateTimeOffset(DateTime.UtcNow.AddMinutes(55)));
            }
            return token;
        }

        public string GetFirebaseAccessToken()
        {
            var scoped = googleCredential
                .CreateScoped(new string[] {
                  "https://www.googleapis.com/auth/userinfo.email",
                  "https://www.googleapis.com/auth/firebase.database",
                  "https://www.googleapis.com/auth/cloud-platform",
                  "https://www.googleapis.com/auth/devstorage.full_control"
            });
            return scoped.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;
        }
    }

    public class RefreshAndSetGoogleAuthToken : DelegatingHandler
    {
        private GoogleAuthHelper googleAuthHelper;

        public RefreshAndSetGoogleAuthToken(GoogleAuthHelper googleAuthHelper)
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
