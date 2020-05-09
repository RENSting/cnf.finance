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
    public class ProjectsController : ControllerBase
    {
        private readonly FinanceContext _context;

        public ProjectsController(FinanceContext context)
        {
            _context = context;
        }

        // GET: api/Projects
        //  query string : ?orgId=&searchName=&activeOnly=
        //      orgId       :   Organization filter
        //      searchName  :   Project Name filter
        //      activeOnly  :   bool
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProject(int? orgId = default, string searchName = default, bool? activeOnly = default)
        {
            activeOnly = activeOnly == null ? true : activeOnly.Value;
            var query = from p in _context.Project
                        where (orgId == null || orgId.Value <= 0 || p.OrganizationId == orgId.Value)
                            && (string.IsNullOrWhiteSpace(searchName) || p.Name.Contains(searchName))
                            && (activeOnly.Value == false || p.ActiveStatus == true)
                        select p;

            return await query.ToListAsync();
        }

        // GET: api/Projects/5?content=null|0|1|2|3 
        // Querystring Parameters:
        //      content =   null|0      : base only;
        //                  1           : has balance and terms
        //                  2           : has plans and performs
        //                  3           : has all
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id, int? content)
        {
            Project project = (content == null || content.Value <= 0) ? await _context.Project.FindAsync(id)  //base only
                : content.Value switch
                {
                    1 => await _context.Project
                            .Include(p => p.AnnualBalance)
                            .Include(p => p.Terms).Where(p => p.ProjectId == id).SingleOrDefaultAsync(), //contains balances and terms
                    2 => await _context.Project
                            .Include(p => p.Plan)
                            .Include(p => p.Perform).Where(p => p.ProjectId == id).SingleOrDefaultAsync(), //contains plans and performs
                    _ => await _context.Project
                            .Include(p => p.AnnualBalance)
                            .Include(p => p.Terms)
                            .Include(p => p.Plan)
                            .Include(p => p.Perform).Where(p => p.ProjectId == id).SingleOrDefaultAsync()   //contains all
                };

            if (project == null)
            {
                return NotFound();
            }

            //为了不造成循环引用，我们处理掉navigation对象的引用
            foreach (var b in project.AnnualBalance)
                b.Project = null;

            foreach (var t in project.Terms)
                t.Project = null;

            foreach (var p in project.Plan)
                p.Project = null;

            foreach (var p in project.Perform)
                p.Project = null;

            return project;
        }

        // GET: api/Projects/YearGroupReport?orgId=&year=month
        //      query string parameters:
        //          orgId   =   1       : group projects of the organization
        //          year    =   2020    : report of the year
        //          month   =   4       : show the month
        [HttpGet("YearGroupReport")]
        public async Task<ActionResult<IEnumerable<Entity.YearGroupRecord>>> YearGroupReport(int orgId, int year, int month)
        {
            var projects = await _context.Project
                                .Where(p => p.OrganizationId == orgId)
                                .ToListAsync();
            var projectIds = projects.Select(p => p.ProjectId);

            var lastBalance = await _context.AnnualBalance
                                .Where(b => projectIds.Contains(b.ProjectId) && b.Year == year - 1)
                                .ToListAsync();

            var plan = await _context.Plan
                                .Where(p => projectIds.Contains(p.ProjectId) && p.Year == year)
                                .ToListAsync();

            var perform = await _context.Perform
                                .Where(p => projectIds.Contains(p.ProjectId) && p.Year == year)
                                .ToListAsync();

            var result = new List<Entity.YearGroupRecord>();
            foreach (var proj in projects)
            {
                var report = Entity.YearGroupRecord.Create(proj, year, month);
                report.BadgeHtml = (proj.HasProblem ? "<span class=\"badge badge-danger\">涉诉</span>" : string.Empty)
                    + (proj.Status switch
                    {
                        1 => "<span class=\"badge badge-info\">在建工程</span>",
                        2 => "<span class=\"badge badge-warning\">已完工未结算</span>",
                        3 => "<span class=\"badge badge-success\">已完工已结算</span>",
                        4 => "<span class=\"badge badge-stop\">停工缓建</span>",
                        _ => "未知的状态"
                    });

                //取上年结转
                report.AnnualBalance = new YearReportBalance
                {
                    Year = year - 1,
                    Incoming = proj.AnnualBalance.Where(b => b.BalanceCategory == 0).Sum(b => b.Balance),
                    Settlement = proj.AnnualBalance.Where(b => b.BalanceCategory == 1).Sum(b => b.Balance),
                    Retrievable = proj.AnnualBalance.Where(b => b.BalanceCategory == 2).Sum(b => b.Balance),
                };
                report.AnnualBalance.Tax = (decimal)proj.TaxRate * report.AnnualBalance.Settlement;

                report.Accumulation = new ReportItem();
                report.CurrentMonth = new ReportItem();
                report.Accumulation.Plan.Incoming = proj.Plan.Sum(p => p.Incoming);
                report.Accumulation.Plan.Settlement = proj.Plan.Sum(p => p.Settlement);
                report.Accumulation.Plan.Retrievable = proj.Plan.Sum(p => p.Retrieve);
                report.Accumulation.Plan.Tax = (decimal)proj.TaxRate * report.Accumulation.Plan.Settlement;
                report.CurrentMonth.Plan.Incoming = proj.Plan.Where(p => p.Month == month).Sum(p => p.Incoming);
                report.CurrentMonth.Plan.Settlement = proj.Plan.Where(p => p.Month == month).Sum(p => p.Settlement);
                report.CurrentMonth.Plan.Retrievable = proj.Plan.Where(p => p.Month == month).Sum(p => p.Retrieve);
                report.CurrentMonth.Plan.Tax = (decimal)proj.TaxRate * report.CurrentMonth.Plan.Settlement;

                report.Accumulation.Perform.Incoming = proj.Perform.Sum(p => p.Incoming);
                report.Accumulation.Perform.Settlement = proj.Perform.Sum(p => p.Settlement);
                report.Accumulation.Perform.Retrievable = proj.Perform.Sum(p => p.Retrieve);
                report.Accumulation.Perform.Tax = (decimal)proj.TaxRate * report.Accumulation.Perform.Settlement;
                report.CurrentMonth.Perform.Incoming = proj.Perform.Where(p => p.Month == month).Sum(p => p.Incoming);
                report.CurrentMonth.Perform.Settlement = proj.Perform.Where(p => p.Month == month).Sum(p => p.Settlement);
                report.CurrentMonth.Perform.Retrievable = proj.Perform.Where(p => p.Month == month).Sum(p => p.Retrieve);
                report.CurrentMonth.Perform.Tax = (decimal)proj.TaxRate * report.CurrentMonth.Perform.Settlement;

                result.Add(report);
            }

            return result;
        }


        // PUT: api/Projects/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // POST: api/Projects
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.ProjectId }, project);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Project.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ProjectId == id);
        }
    }
}
