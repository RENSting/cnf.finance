using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cnf.Finance.Api.Models;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerformsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public PerformsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Performs
        //  query string:
        //      year        : year filter
        //      projectIds  : projects filter (is a comma separated string of Ids of projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Perform>>> GetPerform(int? year, string projectIds)
        {
            var ids = string.IsNullOrWhiteSpace(projectIds) ? new List<int>()
               : projectIds.Split(',').Select(s => int.Parse(s)).ToList();
            var query = from p in _context.Perform
                        where (year == null || year <= 0 || p.Year == year)
                            && (ids.Count == 0 || ids.Contains(p.ProjectId))
                        select p;

            return await query.ToListAsync();
        }

        // GET: api/Performs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Perform>> GetPerform(int id)
        {
            var perform = await _context.Perform.FindAsync(id);

            if (perform == null)
            {
                return NotFound();
            }

            return perform;
        }

        // PUT: api/Performs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerform(int id, Perform perform)
        {
            if (id != perform.Id)
            {
                return BadRequest();
            }

            _context.Entry(perform).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Performs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Perform>> PostPerform(Perform perform)
        {
            _context.Perform.Add(perform);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerform", new { id = perform.Id }, perform);
        }

        // DELETE: api/Performs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Perform>> DeletePerform(int id)
        {
            var perform = await _context.Perform.FindAsync(id);
            if (perform == null)
            {
                return NotFound();
            }

            _context.Perform.Remove(perform);
            await _context.SaveChangesAsync();

            return perform;
        }

        private bool PerformExists(int id)
        {
            return _context.Perform.Any(e => e.Id == id);
        }
    }
}
