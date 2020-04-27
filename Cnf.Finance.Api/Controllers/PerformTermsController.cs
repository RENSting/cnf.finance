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
    public class PerformTermsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public PerformTermsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/PerformTerms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PerformTerms>>> GetPerformTerms()
        {
            return await _context.PerformTerms.ToListAsync();
        }

        // GET: api/PerformTerms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PerformTerms>> GetPerformTerms(int id)
        {
            var performTerms = await _context.PerformTerms.FindAsync(id);

            if (performTerms == null)
            {
                return NotFound();
            }

            return performTerms;
        }

        // PUT: api/PerformTerms/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPerformTerms(int id, PerformTerms performTerms)
        {
            if (id != performTerms.Id)
            {
                return BadRequest();
            }

            _context.Entry(performTerms).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PerformTermsExists(id))
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

        // POST: api/PerformTerms
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PerformTerms>> PostPerformTerms(PerformTerms performTerms)
        {
            _context.PerformTerms.Add(performTerms);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPerformTerms", new { id = performTerms.Id }, performTerms);
        }

        // DELETE: api/PerformTerms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PerformTerms>> DeletePerformTerms(int id)
        {
            var performTerms = await _context.PerformTerms.FindAsync(id);
            if (performTerms == null)
            {
                return NotFound();
            }

            _context.PerformTerms.Remove(performTerms);
            await _context.SaveChangesAsync();

            return performTerms;
        }

        private bool PerformTermsExists(int id)
        {
            return _context.PerformTerms.Any(e => e.Id == id);
        }
    }
}
