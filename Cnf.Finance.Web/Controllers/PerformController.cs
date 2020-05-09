using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cnf.Finance.Web.Models;
using Cnf.Finance.Web.Services;
using Cnf.Finance.Entity;
using Microsoft.AspNetCore.Authorization;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class PerformController : Controller
    {
        private readonly ISystemService _systemService;
        private readonly IProjectService _projectService;
        private readonly IPerformService _performService;
        private readonly IPlanService _planService;

        public PerformController(ISystemService systemService, IProjectService projectService, 
            IPerformService performService, IPlanService planService)
        {
            _systemService = systemService;
            _projectService = projectService;
            _performService = performService;
            _planService = planService;
        }

        public async Task<IActionResult> Index(ProjectYearViewModel model)
        {
            if (model.Year <= 0)
                model.Year = DateTime.Today.Year;

            var allowAllOrgs = Helper.AllowAllOrgs(HttpContext, out int? allowedOrgId);
            if (!allowAllOrgs)
                model.SelectedOrgId = allowedOrgId;


            //TODO: 帅选满足条件的项目
            var projects = await _projectService.SearchProjects(model.SelectedOrgId, model.SearchName, !model.IncludeInActive);

            await model.CalculatePerforms(model.Year, projects, _projectService, _performService);

            foreach (var proj in model.ProjectRowsDic.Values)
            {
                if (proj.Balance != null)
                    proj.Balance.ClearZeroProperties();

                foreach (var m in proj.MonthDataDic.Values)
                    if (m != null)
                        m.ClearZeroProperties();
            }

            //年度列表
            var years = new List<int>();
            for (var i = 2020; i < 2030; i++)
                years.Add(i);
            ViewBag.YearList = new SelectList(years);

            var orgnizations = allowAllOrgs ? await _systemService.GetOrganizations() :
                    new Organization[] { await _systemService.FindOrganization(allowedOrgId.Value) };

            //组织列表
            ViewBag.OrgList = new SelectList(orgnizations,
                nameof(Organization.OrganizationId), nameof(Organization.Name));

            return View(model);
        }

        public async Task<IActionResult> Edit(int projectId, int year, int month)
        {
            var project = await _projectService.RetriveProjectWithDetails(projectId, ProjectApiContent.RetrieveAll);
            if (project == null || project.ProjectId <= 0 || year < 2020 || month < 1 || month > 12)
            {
                return NotFound();
            }

            //上一年结转
            var balance = BalanceViewModel.Create(project.AnnualBalance.Where(b => b.Year == year - 1));

            //当年全部计划
            var plans = project.Plan.Where(p => p.Year == year);
            var planId = plans.Where(p => p.Month == month).SingleOrDefault()?.Id;
            //当年全部月报
            var performs = project.Perform.Where(p => p.Year == year);
            var performId = performs.Where(p => p.Month == month).SingleOrDefault()?.Id;

            var model = MonthPerformViewModel.Create(project, year, month, balance, plans, performs);

            model.Terms = project.Terms.OrderBy(p => p.TermsCategory).ThenBy(p => p.TargetDate).Select(p => (TermsViewModel)p);

            //当月全部计划任务
            if (planId.HasValue)
            {
                model.PlanTerms = (await _planService.GetPlanTerms(planId.Value))
                    .Select(p => TaskViewModel.Create(p, project.Terms.SingleOrDefault(t=>t.Id == p.TermsId)));
            }

            //当月全部完成任务
            if(performId.HasValue)
            {
                model.PerformTerms = (await _performService.GetPerformTerms(performId.Value))
                    .Select(p => TaskViewModel.Create(p, project.Terms.SingleOrDefault(t => t.Id == p.TermsId)));
            }
           
            model.ClearZeroProperties();
            
            return View(model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MonthPerformViewModel model)
        {
            if(ModelState.IsValid)
            {
                var performs = await _performService.GetYearPerformsOfProject(model.Year, model.ProjectId);
                var p = performs.FirstOrDefault(p => p.Month == model.Month);
                if(p == null)
                {
                    //create new
                    p = new Perform
                    {
                        ProjectId = model.ProjectId,
                        Year = model.Year,
                        Month = model.Month,
                    };
                }
                p.Incoming = model.Incoming;
                p.Settlement = model.Settlement;
                p.Retrieve = model.Retrievable;

                await _performService.SavePerform(p);

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}