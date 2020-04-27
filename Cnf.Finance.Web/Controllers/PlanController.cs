using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cnf.Finance.Web.Services;
using Cnf.Finance.Web.Models;
using Cnf.Finance.Entity;
using Microsoft.AspNetCore.Authorization;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        private readonly ISystemService _systemService;
        private readonly IProjectService _projectService;
        private readonly IPlanService _planService;

        public PlanController(ISystemService systemService, IProjectService projectService, IPlanService planService)
        {
            _systemService = systemService;
            _projectService = projectService;
            _planService = planService;
        }

        public async Task<IActionResult> Index(ProjectYearViewModel model)
        {
            //if (model == null)
            //{
            //    model = new ProjectYearViewModel
            //    {
            //        //TODO: 指定当前用户所属单位自动设定
            //        SelectedOrgId = null,
            //        IncludeInActive = false,
            //        Year = DateTime.Today.Year,    //事实上，这个可以不初始化，因为Calc扩展方法会重写该属性。
            //    };
            //}
            if (model.Year <= 0)
                model.Year = DateTime.Today.Year;

            var allowAllOrgs = Helper.AllowAllOrgs(HttpContext, out int? allowedOrgId);
            if (!allowAllOrgs)
                model.SelectedOrgId = allowedOrgId;

            //TODO: 帅选满足条件的项目
            var projects = await _projectService.SearchProjects(model.SelectedOrgId, model.SearchName, !model.IncludeInActive);

            await model.CalculatePlans(model.Year, projects, _projectService, _planService);

            foreach(var proj in model.ProjectRowsDic.Values)
            {
                if (proj.Balance != null)
                    proj.Balance.ClearZeroProperties();

                foreach (var m in proj.MonthDataDic.Values)
                    if (m != null)
                        m.ClearZeroProperties();
            }

            //年度列表
            var years = new List<int>();
            for (var i = 2019; i < 2030; i++)
                years.Add(i);
            ViewBag.YearList = new SelectList(years);

            var orgnizations = allowAllOrgs ? await _systemService.GetOrganizations() :
                    new Organization[] { await _systemService.FindOrganization(allowedOrgId.Value) };
            //组织列表
            ViewBag.OrgList = new SelectList(orgnizations,
                nameof(Organization.OrganizationId), nameof(Organization.Name));

            return View(model);
        }

        public async Task<IActionResult> Edit(int projectId, int year)
        {
            var project = await _projectService.FindProject(projectId);
            if(project == null || project.ProjectId<=0 || year < 2019)
            {
                return NotFound();
            }

            var projectYear = new ProjectYearViewModel();
            await projectYear.CalculatePlans(year, new Project[] { project }, _projectService, _planService);

            var model = projectYear.ProjectRowsDic[project.ProjectId];
            for(var month = 1; month < 13; month++)
            {
                if(model.MonthDataDic[month] == null)
                {
                    model.MonthDataDic[month] = new MonthDataViewModel
                    {
                        Month = month,
                    };
                }
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(YearRowViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existPlans = await _planService.GetYearPlansOfProject(model.Year, model.ProjectId);

                // TODO:
                foreach(var monthData in model.MonthDataDic.Values)
                {
                    var plan = (from p in existPlans
                               where p.Month == monthData.Month
                               select p).SingleOrDefault();
                    if (plan == null)
                    {
                        plan = new Plan
                        {
                            ProjectId = model.ProjectId,
                            Year = model.Year,
                            Month = monthData.Month,
                        };
                    }
                    plan.Incoming = monthData.Incoming;
                    plan.Settlement = monthData.Settlement;
                    plan.Retrieve = monthData.Retievable;

                    await _planService.SavePlan(plan);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }
    }
}