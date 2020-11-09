using System;
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

        [HttpGet("list")]
        public ActionResult<object> GetList([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            long totalCount;
            var data = dbContext.PagedListWithCount<Employee>("Employee", pageSize, pageNo,out totalCount).ToList();
            return new
            {
                data,
                totalCount
            };
        }

        [HttpGet("{id}")]
        public ActionResult<Employee> Get(string id)
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
            value.Id = DateTime.UtcNow.Ticks;
            dbContext.Insert("Employee", value, value.Id);
            return StatusCode(201, "Created");
        }

        [HttpPut("{id}")]
        public ActionResult<Employee> Put(string id, [FromBody] Employee value)
        {
            var obj = dbContext.Single<Customer>("Employee", id);
            if (obj != null)
            {
                dbContext.Update("Employee", value, id);
            }
            return Get(value._key);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Employee", id);
            return Ok("Deleted!");
        }
    }
}
