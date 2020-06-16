using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;
using System.Security.Permissions;

namespace Cnf.Finance.Web.Models
{
    /// <summary>
    /// View Model Class was used by Home/Index page
    /// </summary>
    public class TaskIndexViewModel
    {
        [Display(Name ="单位")]
        public int OrganizationId { get; set; }

        [Display(Name ="年度")]
        public int Year { get; set; }

        [Display(Name ="月份")]
        public int Month { get; set; }

        public IEnumerable<TaskRowViewModel> PlanTasks { get; set; }

        public IEnumerable<TaskRowViewModel> PerformTasks { get; set; }

        public void BindPlanTasks(IEnumerable<PlanTerms> planTerms)
        {
            PlanTasks = new List<TaskRowViewModel>();
            foreach(var t in planTerms)
            {
                ((List<TaskRowViewModel>)PlanTasks).Add(t);
            }
        }

        public void BindPerformTasks(IEnumerable<PerformTerms> performTerms)
        {
            PerformTasks = new List<TaskRowViewModel>();
            foreach (var t in performTerms)
                ((List<TaskRowViewModel>)PerformTasks).Add(t);
        }
    }

    public class TaskRowViewModel
    {
        public int OrganizationId { get; set; }
        public int ProjectId { get; set; }

        [Display(Name ="项目名称")]
        public string ProjectName { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public bool ProjectHasProblem { get; set; }

        public int TermsId { get; set; }
        public TermsCategory TermsCategory { get; set; }
        [Display(Name ="合同条款")]
        public string TermsContent { get; set; }
        public DateTime? TermsDate { get; set; }
        public decimal? TermsAmount { get; set; }
        public int TaskId { get; set; }
        public decimal? Incoming { get; set; }
        public decimal? Settlement { get; set; }
        public decimal? Retrieved { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        [Display(Name ="任务摘要")]
        public string TaskContent { get; set; }

        public static implicit operator TaskRowViewModel(PlanTerms planTerms)=>
            new TaskRowViewModel
            {
                OrganizationId = planTerms.Terms.Project.OrganizationId,
                ProjectId = planTerms.Terms.ProjectId,
                ProjectName = planTerms.Terms.Project.ShortName,
                ProjectStatus = (ProjectStatus)planTerms.Terms.Project.Status,
                ProjectHasProblem = planTerms.Terms.Project.HasProblem,
                TermsId = planTerms.Terms.Id,
                TermsCategory = (TermsCategory)planTerms.Terms.TermsCategory,
                TermsContent = planTerms.Terms.Provision,
                TermsDate = planTerms.Terms.TargetDate,
                TermsAmount = planTerms.Terms.TargetAmount,
                TaskId = planTerms.Plan.Id,
                Incoming = planTerms.Plan.Incoming,
                Settlement = planTerms.Plan.Settlement,
                Retrieved = planTerms.Plan.Retrieve,
                Year = planTerms.Plan.Year,
                Month = planTerms.Plan.Month,
                TaskContent = planTerms.Comments,
            };

        public static implicit operator TaskRowViewModel(PerformTerms performTerms) =>
            new TaskRowViewModel
            {
                OrganizationId = performTerms.Terms.Project.OrganizationId,
                ProjectId = performTerms.Terms.ProjectId,
                ProjectName = performTerms.Terms.Project.ShortName,
                ProjectStatus = (ProjectStatus)performTerms.Terms.Project.Status,
                ProjectHasProblem = performTerms.Terms.Project.HasProblem,
                TermsId = performTerms.Terms.Id,
                TermsCategory = (TermsCategory)performTerms.Terms.TermsCategory,
                TermsContent = performTerms.Terms.Provision,
                TermsDate = performTerms.Terms.TargetDate,
                TermsAmount = performTerms.Terms.TargetAmount,
                TaskId = performTerms.Perform.Id,
                Incoming = performTerms.Perform.Incoming,
                Settlement = performTerms.Perform.Settlement,
                Retrieved = performTerms.Perform.Retrieve,
                Year = performTerms.Perform.Year,
                Month = performTerms.Perform.Month,
                TaskContent = performTerms.Comments,
            };
    }
}
