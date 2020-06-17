using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        public int TermsId { get; set; }

        public TermsCategory TermsCategory { get; set; }

        public string Provision { get; set; }

        [DisplayFormat(DataFormatString ="{0:#.####}")]
        public decimal? TargetAmount { get; set; }

        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}")]
        public DateTime? TargetDate { get; set; }

        public string Comments { get; set; }

        public static TaskViewModel Create(PlanTerms planTerms, Terms terms) =>
            new TaskViewModel
            {
                Id = planTerms.Id,
                Comments = planTerms.Comments,
                Provision = terms.Provision,
                TargetAmount = terms.TargetAmount,
                TargetDate = terms.TargetDate,
                TermsCategory = (TermsCategory)terms.TermsCategory,
                TermsId = terms.Id,
            };

        public static TaskViewModel Create(PerformTerms performTerms, Terms terms) =>
            new TaskViewModel
            {
                Id = performTerms.Id,
                Comments = performTerms.Comments,
                Provision = terms.Provision,
                TargetAmount = terms.TargetAmount,
                TargetDate = terms.TargetDate,
                TermsCategory = (TermsCategory)terms.TermsCategory,
                TermsId = terms.Id,
            };
    }

    public class PlanManageViewModel
    {
        public int PlanId { get; set; }
        public int? PreviousId { get; set; }
        public int? NextId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectManager { get; set; }
        public decimal ContractAmount { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public MonthDataViewModel Data { get; set; }

        public IEnumerable<TermsViewModel> Terms { get; set; }

        public int? EditingTaskId { get; set; }

        [Required(ErrorMessage ="必须选择任务依据的合同条款")]
        public int? SelectedTermsId { get; set; }

        [Display(Name ="任务摘要")]
        [Required(ErrorMessage = "必须输入任务摘要，如果无需展开，可以把条款内容复制过来")]
        public string TaskComments { get; set; }

        public IEnumerable<TaskViewModel> Tasks { get; set; }

        public static implicit operator PlanManageViewModel(Plan plan)=>
            new PlanManageViewModel
            {
                PlanId = plan.Id,
                ContractAmount = plan.Project.ContractAmount,
                Data = (MonthDataViewModel)plan,
                Month = plan.Month,
                ProjectId = plan.ProjectId,
                ProjectManager = plan.Project.ProjectManager,
                ProjectName = plan.Project.Name,
                Tasks = plan.PlanTerms.Select(p=>TaskViewModel.Create(p, plan.Project.Terms.SingleOrDefault(t=>t.Id==p.TermsId))),
                Terms = plan.Project.Terms.OrderBy(p=>p.TermsCategory).ThenBy(p=>p.TargetDate).Select(p => (TermsViewModel)p),
                Year = plan.Year,
                EditingTaskId = default,
                SelectedTermsId = default,
                TaskComments = default,
            };
    }
}
