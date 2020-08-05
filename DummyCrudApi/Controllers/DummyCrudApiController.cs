using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DummyCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DummyCrudApi.Controllers
{
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
}
