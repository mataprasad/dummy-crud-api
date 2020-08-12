using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DummyCrudApi.Fx
{
    public class SwaggerUnauthorizedResponse : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("401", new OpenApiResponse()
            {
                Description = "Unauthorized"
            });

            if (context.ApiDescription.HttpMethod.ToUpper() == "POST")
            {
                operation.Responses.Remove("200");
                operation.Responses.Add("201", new OpenApiResponse()
                {
                    Description = "Created"
                });
            }
        }
    }
}
