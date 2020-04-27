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
    public class TermsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public TermsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Terms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Terms>>> GetTerms()
        {
            return await _context.Terms.ToListAsync();
        }

        // GET: api/Terms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Terms>> GetTerms(int id)
        {
            var terms = await _context.Terms.FindAsync(id);

            if (terms == null)
            {
                return NotFound();
            }

            return terms;
        }

        // PUT: api/Terms/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerms(int id, Terms terms)
        {
            if (id != terms.Id)
            {
                return BadRequest();
            }

            _context.Entry(terms).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TermsExists(id))
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

        // POST: api/Terms
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Terms>> PostTerms(Terms terms)
        {
            _context.Terms.Add(terms);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTerms", new { id = terms.Id }, terms);
        }

        // DELETE: api/Terms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Terms>> DeleteTerms(int id)
        {
            var terms = await _context.Terms.FindAsync(id);
            if (terms == null)
            {
                return NotFound();
            }

            _context.Terms.Remove(terms);
            await _context.SaveChangesAsync();

            return terms;
        }

        private bool TermsExists(int id)
        {
            return _context.Terms.Any(e => e.Id == id);
        }
    }
}
