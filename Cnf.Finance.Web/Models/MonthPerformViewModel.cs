using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public class MonthPerformViewModel
    {
        public int ProjectId { get; set; }

        [Display(Name = "项目名称")]
        public string ProjectName { get; set; }

        [Display(Name = "预计总收入（万元）")]
        [DisplayFormat(DataFormatString ="{0:#.####}")]
        public decimal ProjectIncoming { get; set; }

        [Display(Name = "预计毛利率（%）")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public double? EstimatingProfit { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        [Display(Name = "上年结转收入")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? IncomingBalance { get; set; }

        [Display(Name = "上年结转结算")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? SettlementBalance { get; set; }

        [Display(Name = "上年结转回款")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? RetrievableBalance { get; set; }

        [Display(Name = "收入计划累计")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? TotalPlanIncoming { get; set; }

        [Display(Name = "结算计划累计")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? TotalPlanSettlement { get; set; }

        [Display(Name = "回款计划累计")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? TotalPlanRetrievable { get; set; }

        [Display(Name = "收入完成累计")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? TotalIncoming { get; set; }

        [Display(Name = "结算完成累计")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? TotalSettlement { get; set; }

        [Display(Name = "回款完成累计")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? TotalRetrievalbe { get; set; }

        [Display(Name = "当月计划收入")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? PlanIncoming { get; set; }

        [Display(Name = "当月计划结算")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? PlanSettlement { get; set; }

        [Display(Name = "当月计划回款")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        public decimal? PlanRetrievalbe { get; set; }

        [Display(Name = "当月实际收入")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        [Required(ErrorMessage = "必须输入收入，如无请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        public decimal? Incoming { get; set; }

        [Display(Name = "当月实际结算")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        [Required(ErrorMessage = "必须输入结算，如无请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        public decimal? Settlement { get; set; }

        [Display(Name = "当月实际回款")]
        [DisplayFormat(DataFormatString = "{0:#.####}")]
        [Required(ErrorMessage = "必须输入回款，如无请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        public decimal? Retrievable { get; set; }

        public static MonthPerformViewModel Create(Project project, int year, int month,
            BalanceViewModel balance, IEnumerable<Plan> plans, IEnumerable<Perform> performs)
        {
            var model = new MonthPerformViewModel
            {
                ProjectId = project.ProjectId,
                ProjectName = project.Name,
                ProjectIncoming = project.ContractAmount,
                EstimatingProfit = project.EstimatingProfit,
                Year = year,
                Month = month,
                IncomingBalance = balance?.Incoming,
                SettlementBalance = balance?.Settlement,
                RetrievableBalance = balance?.Retrievable,
                PlanIncoming = (from p in plans where p.Month == month
                                select p).SingleOrDefault()?.Incoming,
                PlanSettlement = (from p in plans
                                  where p.Month == month
                                  select p).SingleOrDefault()?.Settlement,
                PlanRetrievalbe = (from p in plans
                                   where p.Month == month
                                   select p).SingleOrDefault()?.Retrieve,
                TotalPlanIncoming = (from p in plans where p.Month < month
                                     select p.Incoming).Sum(),
                TotalPlanSettlement = (from p in plans
                                       where p.Month < month
                                       select p.Settlement).Sum(),
                TotalPlanRetrievable = (from p in plans
                                        where p.Month < month
                                        select p.Retrieve).Sum(),
                TotalIncoming = (from p in performs
                                 where p.Month < month
                                 select p.Incoming).Sum(),
                TotalSettlement = (from p in performs
                                   where p.Month < month
                                   select p.Settlement).Sum(),
                TotalRetrievalbe = (from p in performs
                                    where p.Month < month
                                    select p.Retrieve).Sum(),
                Incoming = (from p in performs
                            where p.Month == month
                            select p).SingleOrDefault()?.Incoming,
                Settlement = (from p in performs
                              where p.Month == month
                              select p).SingleOrDefault()?.Settlement,
                Retrievable = (from p in performs
                              where p.Month == month
                              select p).SingleOrDefault()?.Retrieve,
            };

            return model;
        }
    }
}
