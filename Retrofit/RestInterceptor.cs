using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using RestSharp;
using RestSharp.Serializers.Newtonsoft.Json;

namespace Retrofit.Net
{
    public class RestInterceptor : IInterceptor
    {
        private IRestClient restClient;
        private IList<IHttpMiddleware> httpMiddlewares;

        public RestInterceptor(IRestClient restClient, IList<IHttpMiddleware> httpMiddlewares = null)
        {
            this.restClient = restClient;
            this.httpMiddlewares = httpMiddlewares;
            if (this.httpMiddlewares == null)
            {
                this.httpMiddlewares = new List<IHttpMiddleware>();
            }
        }

        public void Intercept(IInvocation invocation)
        {
            // Build Request
            var isRawRequest=IsRawRequest(invocation.Method);
            IRestRequest request = null;
            if (!isRawRequest)
            {
                var methodInfo = new RestMethodInfo(invocation.Method); // TODO: Memorize these objects in a hash for performance
                request = new RequestBuilder(methodInfo, invocation.Arguments).Build();
            }
            else
            {
                request = invocation.Arguments.FirstOrDefault() as IRestRequest;
            }

            request.JsonSerializer = new NewtonsoftJsonSerializer();

            foreach (var httpMiddleware in this.httpMiddlewares)
            {
                request = httpMiddleware.Before(request);
            }

            var responseType = invocation.Method.ReturnType;
            var methods = restClient.GetType().GetMethods();

            MethodInfo generic = methods.Where(m => m.Name == "Execute").FirstOrDefault(m => m.GetParameters().Count() == 1 && m.GetParameters()[0].ParameterType == typeof(IRestRequest));
            if (!isRawRequest)
            {
                var genericTypeArgument = responseType.GenericTypeArguments[0];
                // We have to find the method manually due to limitations of GetMethod()
                MethodInfo method = methods.Where(m => m.Name == "Execute").FirstOrDefault(m => m.IsGenericMethod && m.GetParameters().Count() == 1);
                generic = method.MakeGenericMethod(genericTypeArgument);
            }
            Logger.Log(restClient, request, null, 0);

            // Execute request
            var returnValue =  generic.Invoke(restClient, new object[] { request });

            invocation.ReturnValue = returnValue;

            foreach (var httpMiddleware in this.httpMiddlewares)
            {
                invocation = httpMiddleware.After(invocation);
            }

            Logger.Log(restClient, request, null, 0);
        }

        private bool IsRawRequest(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            if (parameters?.Count() == 1 && parameters[0].ParameterType == typeof(IRestRequest))
            {
                return true;
            }
            return false;
        }
    }
}