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
using Microsoft.Extensions.Logging;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class PerformController : Controller
    {
        private readonly ISystemService _systemService;
        private readonly IProjectService _projectService;
        private readonly IPerformService _performService;
        private readonly IPlanService _planService;
        private readonly ILogger<PerformController> _logger;

        public PerformController(ISystemService systemService, IProjectService projectService, 
            IPerformService performService, IPlanService planService, ILogger<PerformController> logger)
        {
            _systemService = systemService;
            _projectService = projectService;
            _performService = performService;
            _planService = planService;
            _logger = logger;
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

        public async Task<IActionResult> Edit(int projectId, int year, int month, int? flag)
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

            if (flag==null)
            {
                ViewBag.Message = string.Empty;
            }
            else if(flag.Value == 0)
            {
                ViewBag.Message = "月报数据保存成功，请继续填写，或者点击‘返回’返回查看页面";
            }
            else
            {
                ViewBag.Message = "保存月报数据数据出错，请记录现在的时间并联系管理员查看日志进行处理";
            }

            return View(model);

        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePerform(MonthPerformViewModel model)
        {
            int? flag = null;
            try
            {
                var performs = await _performService.GetYearPerformsOfProject(model.Year, model.ProjectId);
                var p = performs.FirstOrDefault(p => p.Month == model.Month);
                if (p == null)
                {
                    //create new
                    p = new Perform
                    {
                        ProjectId = model.ProjectId,
                        Year = model.Year,
                        Month = model.Month,
                    };
                }
                p.Incoming = model.PerformData.Incoming;
                p.Settlement = model.PerformData.Settlement;
                p.Retrieve = model.PerformData.Retrievable;

                await _performService.SavePerform(p);
                flag = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                flag = 1;
            }
            return RedirectToAction(nameof(Edit), new { projectId = model.ProjectId, year = model.Year, month = model.Month, flag = flag });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePerformItem(MonthPerformViewModel model)
        {
            if (ModelState.IsValid)
            {
                var performItem = new PerformTerms
                {
                    PerformId = model.PerformId ?? 0,
                    TermsId = model.SelectedTermsId.Value,
                    Comments = model.EditingComments,
                };
                if (model.EditingItemId.HasValue && model.EditingItemId.Value > 0)
                {
                    performItem.Id = model.EditingItemId.Value;
                }
                await _performService.SavePerformTerms(performItem);

                return RedirectToAction(nameof(Edit), new { projectId = model.ProjectId, year = model.Year, month = model.Month });
            }

            return StatusCode(500, ModelState);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePerformItem(MonthPerformViewModel model)
        {
            if (model.EditingItemId == null)
            {
                return StatusCode(500, "没有发送要删除的完成情况Id");
            }

            await _performService.DeletePerformTerms(model.EditingItemId.Value);

            return RedirectToAction(nameof(Edit), new { projectId = model.ProjectId, year = model.Year, month = model.Month });
        }
    }
}