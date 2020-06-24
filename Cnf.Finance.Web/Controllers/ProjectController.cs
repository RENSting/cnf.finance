using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cnf.Finance.Web.Services;
using Cnf.Finance.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Cnf.Finance.Entity;
using Microsoft.AspNetCore.Authorization;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly ISystemService _systemService;
        private readonly IProjectService _projectService;

        public ProjectController(ISystemService systemService, IProjectService projectService)
        {
            _systemService = systemService;
            _projectService = projectService;
        }

        public async Task<IActionResult> Index(ProjectListViewModel model)
        {
            var allowAllOrgs = Helper.AllowAllOrgs(HttpContext, out int? allowedOrgId);
            if (!allowAllOrgs)
                model.SelectedOrgId = allowedOrgId;
            if (model.PageSize <= 0)
                model.PageSize = 5;

            var orgnizations = allowAllOrgs ? await _systemService.GetOrganizations() :
                                new Organization[] { await _systemService.FindOrganization(allowedOrgId.Value) };

            SearchResult<Project> searchResult = await _projectService.SearchProjects(
                        model.SelectedOrgId, model.SearchName, !model.IncludeInActive, model.PageIndex, model.PageSize);

            model.Pages = searchResult.Total == 0 ? 1
                                  : (searchResult.Total % model.PageSize == 0 ? searchResult.Total / model.PageSize
                                                        : searchResult.Total / model.PageSize + 1
                                    );

            model.Projects = searchResult.Records.Select(p => ProjectViewModel.Create(p, orgnizations));

            ViewBag.OrgList = new SelectList(orgnizations,
                nameof(Organization.OrganizationId), nameof(Organization.Name));
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            Project project;
            if (id.HasValue)
            {
                project = await _projectService.FindProject(id.Value);
            }
            else
            {
                project = new Project
                {
                    ActiveStatus = true,
                };
            }

            var allowAllOrgs = Helper.AllowAllOrgs(HttpContext, out int? allowedOrgId);
            if (!allowAllOrgs)
                project.OrganizationId = allowedOrgId.Value;

            var orgnizations = allowAllOrgs ? await _systemService.GetOrganizations() :
                                new Organization[] { await _systemService.FindOrganization(allowedOrgId.Value) };

            ViewBag.OrgList = new SelectList(orgnizations,
                nameof(Organization.OrganizationId), nameof(Organization.Name));
            return View(project);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Project model)
        {
            if (ModelState.IsValid)
            {
                if (id.HasValue)
                {
                    if (id.Value != model.ProjectId)
                    {
                        ModelState.AddModelError("", "提交的模型和要更改的项目不一致");
                        return View(model);
                    }
                    await _projectService.UpdateProject(model);
                }
                else
                {
                    await _projectService.CreateProject(model);
                }
                return RedirectToAction(nameof(Index));
            }

            var allowAllOrgs = Helper.AllowAllOrgs(HttpContext, out int? allowedOrgId);

            var orgnizations = allowAllOrgs ? await _systemService.GetOrganizations() :
                                new Organization[] { await _systemService.FindOrganization(allowedOrgId.Value) };

            ViewBag.OrgList = new SelectList(orgnizations,
                nameof(Organization.OrganizationId), nameof(Organization.Name));
            return View(model);
        }

        public async Task<IActionResult> Manage(int id, string tab)
        {
            var project = await _projectService.RetriveProjectWithDetails(id, ProjectApiContent.WithBalanceAndTerms);
            if (project == null || project.ProjectId <= 0)
                return NotFound();

            var model = ProjectManageViewModel.Create(project);

            if (string.IsNullOrWhiteSpace(tab))
                ViewBag.ActiveTabName = TermsCategory.Settle.ToString();
            else
                ViewBag.ActiveTabName = tab;

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBalance(ProjectManageViewModel model)
        {
            var projectId = model.EditingBalance.ProjectId;
            var year = model.EditingBalance.Year;
            var balances = await _projectService.GetAnnualBalances(projectId, year);
            foreach (var b in balances)
            {
                await _projectService.DeleteBalance(b.Id);
            }
            return RedirectToAction(nameof(Manage), new { id = projectId });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveBalance(ProjectManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var balances = await _projectService.GetAnnualBalances(model.EditingBalance.ProjectId, model.EditingBalance.Year);
                //1. 检查和保存incoming
                var incoming = balances.Where(b => b.BalanceCategory == (int)BalanceCategory.Incoming).FirstOrDefault();
                if (incoming == null || incoming.Id <= 0)
                {
                    //新增
                    incoming = new AnnualBalance
                    {
                        ProjectId = model.EditingBalance.ProjectId,
                        Year = model.EditingBalance.Year,
                        BalanceCategory = (int)BalanceCategory.Incoming,
                    };
                }
                incoming.Balance = model.EditingBalance.Incoming;

                await _projectService.SaveBalance(incoming);

                //2. 检查和保存settle
                var settle = balances.Where(b => b.BalanceCategory == (int)BalanceCategory.Settlement).FirstOrDefault();
                if (settle == null || settle.Id <= 0)
                {
                    //新增
                    settle = new AnnualBalance
                    {
                        ProjectId = model.EditingBalance.ProjectId,
                        Year = model.EditingBalance.Year,
                        BalanceCategory = (int)BalanceCategory.Settlement,
                    };
                }
                settle.Balance = model.EditingBalance.Settlement;

                await _projectService.SaveBalance(settle);

                //3. 检查和保存retrieve
                var retrieve = balances.Where(b => b.BalanceCategory == (int)BalanceCategory.Retrievable).FirstOrDefault();
                if (retrieve == null || retrieve.Id <= 0)
                {
                    //新增
                    retrieve = new AnnualBalance
                    {
                        ProjectId = model.EditingBalance.ProjectId,
                        Year = model.EditingBalance.Year,
                        BalanceCategory = (int)BalanceCategory.Retrievable,
                    };
                }
                retrieve.Balance = model.EditingBalance.Retrievable;

                await _projectService.SaveBalance(retrieve);

                return RedirectToAction(nameof(Manage), new { id = model.EditingBalance.ProjectId });
            }
            return StatusCode(500);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTerms(ProjectManageViewModel model)
        {
            var termsId = model.EditingTerms.TermsId;
            await _projectService.DeleteTerms(termsId);
            return RedirectToAction(nameof(Manage), new { id = model.EditingTerms.ProjectId, tab = model.EditingTerms.Category });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveTerms(ProjectManageViewModel model)
        {
            if (ModelState.IsValid)
            {
                Terms terms = new Terms
                {
                    Id = model.EditingTerms.TermsId,
                    ProjectId = model.EditingTerms.ProjectId,
                    Provision = model.EditingTerms.Provision,
                    Remarks = model.EditingTerms.Remarks,
                    TermsCategory = (int)model.EditingTerms.Category,
                    TargetAmount = model.EditingTerms.Amount,
                    TargetDate = model.EditingTerms.OnDate,
                };

                await _projectService.SaveTerms(terms);

                return RedirectToAction(nameof(Manage), new { id = model.EditingTerms.ProjectId, tab = model.EditingTerms.Category });
            }
            return StatusCode(500);

        }
    }
}