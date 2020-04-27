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
    public class AnnualBalancesController : ControllerBase
    {
        private readonly FinanceContext _context;

        public AnnualBalancesController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/AnnualBalances?projectId=&year=&porjectsIds
        //  query string:
        //      projectId   : unique project filter (int)
        //      year        : year filter
        //      projectIds  : projects filter (is a comma separated string of Ids of projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnualBalance>>> GetAnnualBalance(int? projectId, int? year, string projectIds)
        {
            var ids = string.IsNullOrWhiteSpace(projectIds)? new List<int>()
                        :projectIds.Split(',').Select(s => int.Parse(s)).ToList();
            if (projectId.HasValue)
            {
                if (ids == null)
                    ids = new List<int>(new int[] { projectId.Value });
                else
                    ids.Add(projectId.Value);
            }

            var query = from b in _context.AnnualBalance
                        where (year == null || year <= 0 || b.Year == year.Value)
                            && (ids.Count == 0 || ids.Contains(b.ProjectId))
                        select b;

            return await query.ToListAsync();
        }

        // GET: api/AnnualBalances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AnnualBalance>> GetAnnualBalance(int id)
        {
            var annualBalance = await _context.AnnualBalance.FindAsync(id);

            if (annualBalance == null)
            {
                return NotFound();
            }

            return annualBalance;
        }

        // PUT: api/AnnualBalances/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnnualBalance(int id, AnnualBalance annualBalance)
        {
            if (id != annualBalance.Id)
            {
                return BadRequest();
            }

            _context.Entry(annualBalance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnnualBalanceExists(id))
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

        // POST: api/AnnualBalances
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<AnnualBalance>> PostAnnualBalance(AnnualBalance annualBalance)
        {
            _context.AnnualBalance.Add(annualBalance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnnualBalance", new { id = annualBalance.Id }, annualBalance);
        }

        // DELETE: api/AnnualBalances/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AnnualBalance>> DeleteAnnualBalance(int id)
        {
            var annualBalance = await _context.AnnualBalance.FindAsync(id);
            if (annualBalance == null)
            {
                return NotFound();
            }

            _context.AnnualBalance.Remove(annualBalance);
            await _context.SaveChangesAsync();

            return annualBalance;
        }

        private bool AnnualBalanceExists(int id)
        {
            return _context.AnnualBalance.Any(e => e.Id == id);
        }
    }
}
