using System.Collections.Generic;
using Castle.DynamicProxy;
using RestSharp;

namespace Retrofit.Net
{
    public class RestAdapter
    {
        private static readonly ProxyGenerator _generator = new ProxyGenerator();
        private IRestClient restClient;
        private IList<IHttpMiddleware> httpMiddlewares;

        public RestAdapter(string baseUrl, IList<IHttpMiddleware> httpMiddlewares=null)
        {
            this.restClient = new RestClient(baseUrl);
            this.httpMiddlewares = httpMiddlewares;
        }

        public RestAdapter(IRestClient client, IList<IHttpMiddleware> httpMiddlewares = null)
        {
            this.restClient = client;
            this.httpMiddlewares = httpMiddlewares;
        }

        public string Server
        {
            get; set;
        }

        public T Create<T>() where T : class
        {
            return _generator.CreateInterfaceProxyWithoutTarget<T>(new RestInterceptor(restClient, this.httpMiddlewares));
        }
    }
}