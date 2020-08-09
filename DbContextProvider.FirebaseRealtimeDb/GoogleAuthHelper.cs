using System;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Caching.Memory;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class GoogleAuthHelper
    {
        private const string CACHE_KEY = "FirebaseAccessToken";
        private MemoryCache memory;
        private GoogleCredential googleCredential;

        public GoogleAuthHelper(GoogleCredential googleCredential)
        {
            this.googleCredential = googleCredential;
            memory = new MemoryCache(new MemoryCacheOptions());
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
                  "https://www.googleapis.com/auth/firebase.database"
            });
            return scoped.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;
        }
    }
}
