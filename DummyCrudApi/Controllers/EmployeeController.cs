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
    [Route("api/employees/")]
    [ApiController]
    public class EmployeeController : DummyCrudApiController
    {

        public EmployeeController(IDbContext dbContext, ILogger<EmployeeController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Employee>("Employee", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> Get(int id)
        {
            var obj = dbContext.Single<Employee>("Employee", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Employee> Post([FromBody] Employee value)
        {
            dbContext.Insert("Employee", value);
            return Get(value.Id);
        }

        [HttpPut("{id}")]
        public ActionResult<Employee> Put(int id, [FromBody] Employee value)
        {
            var obj = dbContext.Single<Customer>("Employee", id);
            if (obj != null)
            {
                value.Id = id;
                dbContext.Update("Employee", value, id);
            }
            return Get(value.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Employee", id);
            return Ok("Deleted!");
        }
    }
}
