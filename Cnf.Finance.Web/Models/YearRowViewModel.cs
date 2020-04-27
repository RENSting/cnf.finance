using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public class YearRowViewModel
    {
        public YearRowViewModel()
        {
            MonthDataDic = new Dictionary<int, MonthDataViewModel>
            {
                {1, null },{2, null },{3, null },{4, null },
                {5, null },{6, null },{7, null },{8, null },
                {9, null },{10, null },{11, null },{12, null },
            };
        }

        [Display(Name = "年度")]
        public int Year { get; set; }

        [Display(Name = "项目ID")]
        public int ProjectId { get; set; }

        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name ="预计总收入(万元)")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal TotalIncoming { get; set; }

        [Display(Name ="预计毛利率（%）")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double? EstimatingProfit { get; set; }

        public ProjectStatus ProjectStatus { get; set; }

        public bool HasProblem { get; set; }

        /// <summary>
        /// 上年结转
        /// </summary>
        public BalanceViewModel Balance { get; set; }

        /// <summary>
        /// 今年每个月的计划，如果尚未指定，那么该月份就是null
        /// </summary>
        public IDictionary<int, MonthDataViewModel> MonthDataDic { get; set; }

        private static YearRowViewModel Create(Project project, int year, IEnumerable<AnnualBalance> annualBalances) =>
            new YearRowViewModel
            {
                ProjectId = project.ProjectId,
                ProjectName = project.ShortName,
                TotalIncoming = project.ContractAmount,
                EstimatingProfit = project.EstimatingProfit,
                ProjectStatus = (ProjectStatus)project.Status,
                HasProblem = project.HasProblem,
                Year = year,
                Balance = BalanceViewModel.Create((from b in annualBalances
                                                   where b.ProjectId == project.ProjectId
                                                        && b.Year == (year - 1)     //结转记录中的年份是前一年份
                                                   select b).ToList()),
            };

        /// <summary>
        /// 利用相关的年度计划和历年结转，生成一个项目年度计划数据行的实例并返回
        /// </summary>
        /// <param name="project"></param>
        /// <param name="plans"></param>
        /// <returns></returns>
        internal static YearRowViewModel Create(Project project, int year,
            IEnumerable<AnnualBalance> annualBalances, IEnumerable<Plan> plans)
        {
            var model = YearRowViewModel.Create(project, year, annualBalances);

            var projectPlans = from p in plans
                               where p.ProjectId == project.ProjectId
                                    && p.Year == year
                               select p;

            foreach (var plan in projectPlans)
                model.MonthDataDic[plan.Month] = plan;

            return model;
        }

        /// <summary>
        /// 利用相关的年度每月完成月报和历年结转，生成一个项目年度月报数据行的实例并返回
        /// </summary>
        /// <param name="project"></param>
        /// <param name="performs"></param>
        /// <returns></returns>
        internal static YearRowViewModel Create(Project project, int year,
            IEnumerable<AnnualBalance> annualBalances, IEnumerable<Perform> performs)
        {
            var model = YearRowViewModel.Create(project, year, annualBalances);

            var projectPerforms = from p in performs
                                  where p.ProjectId == project.ProjectId
                                       && p.Year == year
                                  select p;

            foreach (var perform in projectPerforms)
                model.MonthDataDic[perform.Month] = perform;

            return model;
        }
    }
}
