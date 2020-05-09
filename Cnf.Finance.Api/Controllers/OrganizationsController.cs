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
    public class OrganizationsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public OrganizationsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Organizations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organization>>> GetOrganization()
        {
            return await _context.Organization.ToListAsync();
        }

        // GET: api/Organizations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetOrganization(int id)
        {
            var organization = await _context.Organization.FindAsync(id);

            if (organization == null)
            {
                return NotFound();
            }

            return organization;
        }

        // GET: api/Organizations/YearGroupReport?year=&month=
        [HttpGet("YearGroupReport")]
        public async Task<ActionResult<IEnumerable<Entity.YearGroupRecord>>> YearGroupReport(int year, int month)
        {
            var orgnizations = await _context.Organization.ToListAsync();

            var result = new List<Entity.YearGroupRecord>();
            foreach(var org in orgnizations)
            {
                var report = Entity.YearGroupRecord.Create(org, year, month);
                IEnumerable<int> projectIds = from p in _context.Project
                                              where p.OrganizationId == org.OrganizationId
                                              select p.ProjectId;
                //取上年结转
                var balanceQuery = _context.AnnualBalance.Include(b => b.Project)
                                    .Where(b => b.Year == year - 1 && projectIds.Contains(b.ProjectId));

                var balances = await balanceQuery.ToListAsync(); // await (from b in _context.AnnualBalance
                                 //  where b.Year == year - 1 && projectIds.Contains(b.ProjectId)
                                   //select b).ToListAsync();
                report.AnnualBalance = new YearReportBalance();
                report.AnnualBalance.Year = year - 1;
                report.AnnualBalance.Incoming = balances.Where(b => b.BalanceCategory == 0).Sum(b => b.Balance);
                report.AnnualBalance.Settlement = balances.Where(b => b.BalanceCategory == 1).Sum(b => b.Balance);
                report.AnnualBalance.Retrievable = balances.Where(b => b.BalanceCategory == 2).Sum(b => b.Balance);
                report.AnnualBalance.Tax = balances.Where(b => b.BalanceCategory == 1).Sum(b => ((decimal)b.Project.TaxRate * b.Balance));

                report.Accumulation = new ReportItem();
                report.CurrentMonth = new ReportItem();
                var planQuery = _context.Plan.Include(p => p.Project)
                                .Where(p => p.Year == year && projectIds.Contains(p.ProjectId));
                var plans = await planQuery.ToListAsync(); //await (from p in _context.Plan
                              //     where p.Year == year && projectIds.Contains(p.ProjectId)
                                //   select p).ToListAsync();
                report.Accumulation.Plan.Incoming = plans.Sum(p => p.Incoming);
                report.Accumulation.Plan.Settlement = plans.Sum(p => p.Settlement);
                report.Accumulation.Plan.Retrievable = plans.Sum(p => p.Retrieve);
                report.Accumulation.Plan.Tax = plans.Sum(p => (decimal)p.Project.TaxRate * p.Settlement);

                report.CurrentMonth.Plan.Incoming = plans.Where(p => p.Month == month).Sum(p => p.Incoming);
                report.CurrentMonth.Plan.Settlement = plans.Where(p => p.Month == month).Sum(p => p.Settlement);
                report.CurrentMonth.Plan.Retrievable = plans.Where(p => p.Month == month).Sum(p => p.Retrieve);
                report.CurrentMonth.Plan.Tax = plans.Where(p => p.Month == month).Sum(p => (decimal)p.Project.TaxRate * p.Settlement);

                var performQuery = _context.Perform.Include(p=>p.Project)
                                    .Where(p => p.Year == year && projectIds.Contains(p.ProjectId));
                var performs = await performQuery.ToListAsync(); //await (from p in _context.Perform
                                 //  where p.Year == year && projectIds.Contains(p.ProjectId)
                                   //select p).ToListAsync();
                report.Accumulation.Perform.Incoming = performs.Sum(p => p.Incoming);
                report.Accumulation.Perform.Settlement = performs.Sum(p => p.Settlement);
                report.Accumulation.Perform.Retrievable = performs.Sum(p => p.Retrieve);
                report.Accumulation.Perform.Tax = performs.Sum(p => (decimal)p.Project.TaxRate * p.Settlement);

                report.CurrentMonth.Perform.Incoming = performs.Where(p => p.Month == month).Sum(p => p.Incoming);
                report.CurrentMonth.Perform.Settlement = performs.Where(p => p.Month == month).Sum(p => p.Settlement);
                report.CurrentMonth.Perform.Retrievable = performs.Where(p => p.Month == month).Sum(p => p.Retrieve);
                report.CurrentMonth.Perform.Tax = performs.Where(p => p.Month == month).Sum(p => (decimal)p.Project.TaxRate * p.Settlement);

                result.Add(report);
            }

            return result;
        }

        // PUT: api/Organizations/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrganization(int id, Organization organization)
        {
            if (id != organization.OrganizationId)
            {
                return BadRequest();
            }

            _context.Entry(organization).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizationExists(id))
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

        // POST: api/Organizations
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Organization>> PostOrganization(Organization organization)
        {
            _context.Organization.Add(organization);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrganization", new { id = organization.OrganizationId }, organization);
        }

        // DELETE: api/Organizations/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Organization>> DeleteOrganization(int id)
        {
            var organization = await _context.Organization.FindAsync(id);
            if (organization == null)
            {
                return NotFound();
            }

            _context.Organization.Remove(organization);
            await _context.SaveChangesAsync();

            return organization;
        }

        private bool OrganizationExists(int id)
        {
            return _context.Organization.Any(e => e.OrganizationId == id);
        }
    }
}
