using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public class ProjectManageViewModel
    {
        public int ProjectId { get; set; }

        [Display(Name ="项目名称")]
        public string ProjectName { get; set; }

        public string ShortName { get; set; }

        [Display(Name ="预计总收入(万)")]
        public decimal Amount { get; set; }

        public bool ActiveStatus { get; set; }

        /// <summary>
        /// 在页面上进行结转时可执行的年份。
        /// 该年份的计算依赖于系统中当前项目已经具备的结转年份、以及当前年份
        /// 其初始值等于：DateTime.Today.Year - 1，但如果该项目已经有了这一初始值年份的结转余额，那么这个值就是0
        /// </summary>
        public int NextBalanceYear { get; set; }

        /// <summary>
        /// 正在编辑中的结转余额
        /// </summary>
        public BalanceViewModel EditingBalance { get; set; }

        /// <summary>
        /// 正在编辑中的合同条款
        /// </summary>
        public TermsViewModel EditingTerms { get; set; }

        public IEnumerable<TermsViewModel> SettleTerms { get; set; }

        public IEnumerable<TermsViewModel> RetrieveTerms { get; set; }

        public IEnumerable<TermsViewModel> OtherTerms { get; set; }

        public IEnumerable<BalanceViewModel> AnnualBalances { get; set; }

        /// <summary>
        /// 传递的project需要使用IProjectService.RetriveProjectWithDetails获取。
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static ProjectManageViewModel Create(Project project)
        {
            var viewModel = new ProjectManageViewModel
            {
                ActiveStatus = project.ActiveStatus,
                Amount = project.ContractAmount,
                ProjectId = project.ProjectId,
                ShortName = project.ShortName,
                ProjectName = project.Name,
                OtherTerms = from t in project.Terms
                             where t.TermsCategory == (int)TermsCategory.Others
                             orderby t.TargetDate
                             select (TermsViewModel)t,
                RetrieveTerms = from t in project.Terms
                                where t.TermsCategory == (int)TermsCategory.Retrieve
                                orderby t.TargetDate
                                select (TermsViewModel)t,
                SettleTerms = from t in project.Terms
                              where t.TermsCategory == (int)TermsCategory.Settle
                              orderby t.TargetDate
                              select (TermsViewModel)t,
            };
            if(project.AnnualBalance == null || project.AnnualBalance.Count == 0)
            {
                viewModel.NextBalanceYear = DateTime.Today.Year - 1;
            }
            else
            {
                IEnumerable<int> years = project.AnnualBalance.Select(b => b.Year).Distinct().OrderBy(y => y);
                List<BalanceViewModel> balanceList = new List<BalanceViewModel>();
                foreach(int y in years)
                {
                    var yearBalances = project.AnnualBalance.Where(b => b.Year == y).ToList();
                    var balanceViewModel = BalanceViewModel.Create(yearBalances);
                    balanceList.Add(balanceViewModel);
                }
                if (balanceList.Count > 0)
                    viewModel.AnnualBalances = balanceList;
                var maxYear = years.Max();
                if (maxYear < DateTime.Today.Year - 1)
                    viewModel.NextBalanceYear = DateTime.Today.Year - 1;
                else
                    viewModel.NextBalanceYear = 0;
            }
            return viewModel;
        }
    }
}
