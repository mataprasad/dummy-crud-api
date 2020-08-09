using Retrofit.Net;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class FirebaseClientFactory
    {
        private RestAdapter restAdapter;

        public FirebaseClientFactory(RestAdapter restAdapter)
        {
            this.restAdapter = restAdapter;
        }

        public FirebaseClient Get()
        {
            return this.restAdapter.Create<FirebaseClient>();
        }
    }
}
