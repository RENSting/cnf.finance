using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cnf.Finance.Web.Models;
using Cnf.Finance.Web.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cnf.Finance.Web.Controllers
{
    public class AnalysisController : Controller
    {
        readonly IAnalysisService _analysisService;
        readonly ISystemService _systemService;
        readonly IProjectService _projectService;

        public AnalysisController(IAnalysisService analysisService, ISystemService systemService, IProjectService projectService)
        {
            _analysisService = analysisService;
            _systemService = systemService;
            _projectService = projectService;
        }

        public async Task<IActionResult> YearProjectReport(int id, int year, int month, int orgId)
        {
            var model = new YearProjectReportViewModel
            {
                OrgId = orgId,
                Year = year,
                Month = month,
                Project = await _projectService.RetriveProjectWithDetails(id, ProjectApiContent.RetrieveAll)
            };

            return View(model);
        }

        public async Task<IActionResult> YearGroupReport(YearGroupReportViewModel model)
        {
            if(model == null || model.Year < 2020)
            {
                model.Year = 2020;
                model.Month = DateTime.Today.Month;
            }
            if(model.Hirarchy == null && model.GroupId == null)
            {
                model.Hirarchy = GroupHirarchy.Organization;
                model.GroupId = null;
                model.GroupName = null;
            }
            else
            {
                if (model.GroupId.HasValue)
                    model.Hirarchy = GroupHirarchy.Project; //目前仅支持单位-项目两级穿透
                else
                    return StatusCode(500);
            }

            switch (model.Hirarchy)
            {
                case GroupHirarchy.Organization:
                    model.GroupRecord = await _analysisService.GetYearGroupReport(model.Year, model.Month);
                    break;
                case GroupHirarchy.Project:
                    model.GroupRecord = await _analysisService.GetYearOrgReport(model.GroupId.Value, model.Year, model.Month);
                    var org = await _systemService.FindOrganization(model.GroupId.Value);
                    model.GroupName = org.Name;
                    break;
                default:
                    break;
            }

            var years = new List<int>();
            for (var i = 2020; i < 2030; i++)
                years.Add(i);
            ViewBag.YearList = new SelectList(years);

            var months = new List<int>();
            for (var i = 1; i < 13; i++)
                months.Add(i);
            ViewBag.MonthList = new SelectList(months);

            return View(model);
        }
    }
}