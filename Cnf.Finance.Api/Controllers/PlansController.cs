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
    public class PlansController : ControllerBase
    {
        private readonly FinanceContext _context;

        public PlansController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Plans
        //  query string:
        //      year        : year filter
        //      projectIds  : projects filter (is a comma separated string of Ids of projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Plan>>> GetPlan(int? year, string projectIds)
        {
            var ids = string.IsNullOrWhiteSpace(projectIds)?new List<int>()
                        : projectIds.Split(',').Select(s => int.Parse(s)).ToList();
            var query = from p in _context.Plan
                        where (year == null || year <= 0 || p.Year == year)
                            && (ids.Count == 0 || ids.Contains(p.ProjectId))
                        select p;

            return await query.ToListAsync();
        }

        // GET: api/Plans/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Plan>> GetPlan(int id)
        {
            var query = _context.Plan
                            .Include(p => p.Project).ThenInclude(p => p.Terms)
                            .Include(p => p.PlanTerms);

            var plan = await query.SingleOrDefaultAsync(p => p.Id == id);
           
            if (plan == null)
            {
                return NotFound();
            }

            //清除循环引用
            plan.Project.Plan = null;
            foreach (var term in plan.Project.Terms)
            {
                term.Project = null;
                term.PlanTerms = null;
            }
            foreach (var pt in plan.PlanTerms)
            {
                pt.Plan = null;
                pt.Terms = null;
            }

            return plan;
        }

        // PUT: api/Plans/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlan(int id, Plan plan)
        {
            if (id != plan.Id)
            {
                return BadRequest();
            }

            _context.Entry(plan).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlanExists(id))
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

        // POST: api/Plans
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Plan>> PostPlan(Plan plan)
        {
            _context.Plan.Add(plan);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlan", new { id = plan.Id }, plan);
        }

        // DELETE: api/Plans/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Plan>> DeletePlan(int id)
        {
            var plan = await _context.Plan.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            _context.Plan.Remove(plan);
            await _context.SaveChangesAsync();

            return plan;
        }

        private bool PlanExists(int id)
        {
            return _context.Plan.Any(e => e.Id == id);
        }
    }
}
