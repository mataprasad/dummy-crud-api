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
    [Route("api/suppliers/")]
    [ApiController]
    public class SupplierController : DummyCrudApiController
    {

        public SupplierController(IDbContext dbContext, ILogger<SupplierController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Supplier>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Supplier>("Supplier", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Supplier> Get(long id)
        {
            var obj = dbContext.Single<Supplier>("Supplier", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Supplier> Post([FromBody] Supplier value)
        {
            dbContext.Insert("Supplier", value);
            return Get(value.Id);
        }

        [HttpPut("{id}")]
        public ActionResult<Supplier> Put(long id, [FromBody] Supplier value)
        {
            var obj = dbContext.Single<Supplier>("Supplier", id);
            if (obj != null)
            {
                value.Id = id;
                dbContext.Update("Supplier", value, id);
            }
            return Get(value.Id);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            dbContext.Delete("Supplier", id);
            return Ok("Deleted!");
        }
    }
}
