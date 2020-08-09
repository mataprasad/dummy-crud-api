using System;
using DbContextProvider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DummyCrudApi.Controllers
{
    [ServiceFilter(typeof(ApiKeyFilter))]
    public class DummyCrudApiController : ControllerBase
    {
        protected readonly IDbContext dbContext;
        protected readonly ILogger<DummyCrudApiController> logger;

        public DummyCrudApiController(IDbContext dbContext, ILogger<DummyCrudApiController> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
    }

    public class ApiKeyFilter : IActionFilter
    {
        private IDbContext dbContext;

        public ApiKeyFilter(IDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var apiKey = context.HttpContext.Request.Headers[Startup.API_KEY_NAME].ToString();
            if(String.IsNullOrWhiteSpace(apiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key missing ins request.");
                return;
            }
            this.dbContext.GlobalSetting[IDbContext.CurrentUserSettingKey] = apiKey;
            if(!this.dbContext.IsDbExists)
            {
                context.Result = new UnauthorizedObjectResult("Not a valid API Key.");
            }
        }
    }
}
