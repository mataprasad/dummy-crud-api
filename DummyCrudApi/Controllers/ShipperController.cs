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
    [Route("api/shippers/")]
    [ApiController]
    public class ShipperController : DummyCrudApiController
    {

        public ShipperController(IDbContext dbContext, ILogger<ShipperController> logger)
            : base(dbContext, logger) { }

        [HttpGet]
        public ActionResult<IEnumerable<Shipper>> Get([FromQuery] int pageSize = 100, [FromQuery] int pageNo = 1)
        {
            return dbContext.PagedList<Shipper>("Shipper", pageSize, pageNo).ToList();
        }

        [HttpGet("{id}")]
        public ActionResult<Shipper> Get(string id)
        {
            var obj = dbContext.Single<Shipper>("Shipper", id);
            if (obj != null)
            {
                return obj;
            }
            return NotFound();
        }

        [HttpPost]
        public ActionResult<Shipper> Post([FromBody] Shipper value)
        {
            value.Id = DateTime.UtcNow.Ticks;
            dbContext.Insert("Shipper", value, value.Id);
            return StatusCode(201, "Created");
        }

        [HttpPut("{id}")]
        public ActionResult<Shipper> Put(string id, [FromBody] Shipper value)
        {
            var obj = dbContext.Single<Shipper>("Shipper", id);
            if (obj != null)
            {
                dbContext.Update("Shipper", value, id);
            }
            return Get(value._key);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            dbContext.Delete("Shipper", id);
            return Ok("Deleted!");
        }
    }
}
