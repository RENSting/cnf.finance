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
    public class PlanTermsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public PlanTermsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/PlanTerms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanTerms>>> GetPlanTerms()
        {
            return await _context.PlanTerms.ToListAsync();
        }

        // GET: api/PlanTerms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanTerms>> GetPlanTerms(int id)
        {
            var planTerms = await _context.PlanTerms.FindAsync(id);

            if (planTerms == null)
            {
                return NotFound();
            }

            return planTerms;
        }

        // PUT: api/PlanTerms/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlanTerms(int id, PlanTerms planTerms)
        {
            if (id != planTerms.Id)
            {
                return BadRequest();
            }

            _context.Entry(planTerms).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanTermsExists(id))
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

        // POST: api/PlanTerms
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PlanTerms>> PostPlanTerms(PlanTerms planTerms)
        {
            _context.PlanTerms.Add(planTerms);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlanTerms", new { id = planTerms.Id }, planTerms);
        }

        // DELETE: api/PlanTerms/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PlanTerms>> DeletePlanTerms(int id)
        {
            var planTerms = await _context.PlanTerms.FindAsync(id);
            if (planTerms == null)
            {
                return NotFound();
            }

            _context.PlanTerms.Remove(planTerms);
            await _context.SaveChangesAsync();

            return planTerms;
        }

        private bool PlanTermsExists(int id)
        {
            return _context.PlanTerms.Any(e => e.Id == id);
        }
    }
}
