using System.Collections.Generic;
using System.Linq;
using DbContextProvider;
using DummyCrudApi.Models;
using DummyCrudApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DummyCrudApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("api/customers/")]
    [ApiController]
    public class CustomerController : DummyCrudApiController
    {

        public CustomerController(IDbContext dbContext, ILogger<CustomerController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Customer>("Customer", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> Get(string id)
        {
            var obj = dbContext.Single<Customer>("Customer", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Customer> Post([FromBody] Customer value)
        {
            dbContext.Insert("Customer", value);
            return Get(value.Id);
        }

        [HttpPut("{id}")]
        public ActionResult<Customer> Put(string id, [FromBody] Customer value)
        {
            var obj = dbContext.Single<Customer>("Customer", id);
            if (obj != null)
            {
                value.Id = id;
                dbContext.Update("Customer", value, id);
            }
            return Get(value.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Customer", id);
            return Ok("Deleted!");
        }
    }
}
