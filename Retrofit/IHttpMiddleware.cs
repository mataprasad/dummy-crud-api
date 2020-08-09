using System;
using Castle.DynamicProxy;
using RestSharp;

namespace Retrofit
{
    public interface IHttpMiddleware
    {
        IRestRequest Before(IRestRequest restRequest);
        IInvocation After(IInvocation restRequest);
    }
}
