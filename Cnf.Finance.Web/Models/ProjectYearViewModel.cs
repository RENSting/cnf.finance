using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;
using Cnf.Finance.Web.Services;

namespace Cnf.Finance.Web.Models
{
    public class ProjectYearViewModel
    {
        public ProjectYearViewModel()
        {
            ProjectRowsDic = new Dictionary<int, YearRowViewModel>();
        }

        [Display(Name = "所属单位")]
        public int? SelectedOrgId { get; set; }

        [Display(Name = "名称")]
        public string SearchName { get; set; }

        [Display(Name = "包含已关闭")]
        public bool IncludeInActive { get; set; }

        [Display(Name ="年度")]
        [Required(ErrorMessage ="必须输入年度")]
        [Range(2019, 2030, ErrorMessage ="年度必须在2019~2030年之间")]
        public int Year { get; set; }

        /// <summary>
        /// Key：ProjectId of 项目
        /// Value：该项目上年结算及当年月计划
        /// </summary>
        public IDictionary<int, YearRowViewModel> ProjectRowsDic { get; set; }

    }

    public static class ProjectYearViewModelExtension
    {
        /// <summary>
        /// 扩展方法，计算项目列表projects的某年度year的两金压降工作计划，并填充viewmodel
        /// </summary>
        /// <param name="model"></param>
        /// <param name="projects"></param>
        public static async Task CalculatePlans(this ProjectYearViewModel model, int year, IEnumerable<Project> projects, 
            IProjectService projectService, IPlanService planService)
        {
            var projectIds = from p in projects select p.ProjectId;
            var plans = await planService.GetYearPlansOfProjects(year, projectIds);
            var balances = await projectService.GetAnnualBalancesOfProjects(year - 1, projectIds); //取上一年的结转
            model.Year = year;
            model.ProjectRowsDic.Clear();
            foreach(var proj in projects)
            {
                model.ProjectRowsDic.Add(proj.ProjectId, YearRowViewModel.Create(proj, year, balances, plans));
            }
        }

        /// <summary>
        /// 扩展方法，计算项目列表projects的某年度year的两金压降工作月报，并填充viewmodel
        /// </summary>
        /// <param name="model"></param>
        /// <param name="projects"></param>
        public static async Task CalculatePerforms(this ProjectYearViewModel model, int year, IEnumerable<Project> projects,
            IProjectService projectService, IPerformService performService)
        {
            var projectIds = from p in projects select p.ProjectId;
            var performs = await performService.GetYearPerformsOfProjects(year, projectIds);
            var balances = await projectService.GetAnnualBalancesOfProjects(year - 1, projectIds); //取上一年的结转
            model.Year = year;
            model.ProjectRowsDic.Clear();
            foreach (var proj in projects)
            {
                model.ProjectRowsDic.Add(proj.ProjectId, YearRowViewModel.Create(proj, year, balances, performs));
            }
        }
    }
}
