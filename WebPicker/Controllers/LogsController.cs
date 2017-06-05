using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPicker.Data;
using WebPicker.Models;
using System.Net;

namespace WebPicker.Controllers
{
    [Produces("application/json")]
    [Route("api/Logs")]
    public class LogsController : Controller
    {
        public LoggingDbContext ApplicationDbContext { get; }

        public LogsController(LoggingDbContext applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }
        // GET: api/Logs
        [HttpGet]
        public async Task<IEnumerable<Log>> Get()
        {
            return await ApplicationDbContext.Log.ToListAsync();
        }

        // GET: api/Logs/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            var any = await ApplicationDbContext.Log.AnyAsync(x => x.Id == id);

            if (any)
            {
                var log = await ApplicationDbContext.Log.FirstAsync(x => x.Id == id);
                return Ok(new {log = log});
            }
            else
            {
                return NotFound();
            }
        }
        
        //// POST: api/Logs
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}
        
        //// PUT: api/Logs/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}
        
        //// DELETE: api/ApiWithActions/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
