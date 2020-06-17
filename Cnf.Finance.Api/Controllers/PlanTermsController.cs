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
        public async Task<ActionResult<IEnumerable<PlanTerms>>> GetPlanTerms(int? planId)
        {
            var query = from t in _context.PlanTerms
                        where planId == null || planId.Value == t.PlanId
                        select t;

            return await query.ToListAsync();
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

        // GET: api/PlanTerms/GetTasks?orgId=1&year=2020&month=2
        // 注意： 返回一个PlanTerms类型的数组，其中，每个元素的Plan, Terms, Terms.Project是有效的对象。
        [HttpGet("GetTasks")]
        public async Task<ActionResult<IEnumerable<PlanTerms>>> GetPlanTerms(int orgId, int year, int month)
        {
            var planTermsQuery = from t in _context.PlanTerms
                                                .Include(t => t.Plan)
                                                .Include(t => t.Terms)
                                                    .ThenInclude(t => t.Project)
                                 where t.Terms.Project.OrganizationId == orgId
                                     && t.Plan.Year == year
                                     && t.Plan.Month == month
                                 select t;

            var planTerms = await planTermsQuery.ToListAsync();

            foreach (var t in planTerms)
            {
                t.Plan.PlanTerms = null;
                t.Plan.Project = null;
                t.Terms.PerformTerms = null;
                t.Terms.PlanTerms = null;
                t.Terms.Project.Plan = null;
                t.Terms.Project.AnnualBalance = null;
                t.Terms.Project.Organization = null;
                t.Terms.Project.Perform = null;
                t.Terms.Project.Terms = null;
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
