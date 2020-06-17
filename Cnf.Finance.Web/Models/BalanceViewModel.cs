using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public enum BalanceCategory
    {
        [Display(Name ="收入")]
        Incoming = 0,
        [Display(Name ="结算")]
        Settlement = 1,
        [Display(Name ="回款")]
        Retrievable = 2,
    }

    public class BalanceViewModel
    {
        public int ProjectId { get; set; }

        [Display(Name ="年度")]
        [Range(2019, 2050, ErrorMessage ="必须在2019~2050之间")]
        public int Year { get; set; }

        [Display(Name ="结转收入")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage ="必须输入数字")]
        [Required(ErrorMessage ="必须输入结转金额，没有请输入0")]
        [DisplayFormat(DataFormatString ="{0:####}")]
        public decimal Incoming { get; set; }

        [Display(Name = "结转结算")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        [Required(ErrorMessage = "必须输入结转金额，没有请输入0")]
        [DisplayFormat(DataFormatString = "{0:####}")]
        public decimal Settlement { get; set; }

        [Display(Name = "结转回款")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        [Required(ErrorMessage = "必须输入结转金额，没有请输入0")]
        [DisplayFormat(DataFormatString = "{0:####}")]
        public decimal Retrievable { get; set; }

        /// <summary>
        /// 类工厂使用一个集合，该集合是指定项目指定年份的结转余额
        /// </summary>
        /// <param name="projectYearBalances"></param>
        /// <returns></returns>
        public static BalanceViewModel Create(IEnumerable<AnnualBalance> projectYearBalances)
        {
            if (projectYearBalances == null || projectYearBalances.Count() == 0)
                return null;

            var model = new BalanceViewModel
            {
                ProjectId = projectYearBalances.FirstOrDefault().ProjectId,
                Year = projectYearBalances.FirstOrDefault().Year
            };
            var incoming = projectYearBalances.FirstOrDefault(b => b.BalanceCategory == (int)BalanceCategory.Incoming);
            if (incoming != null)
                model.Incoming = incoming.Balance;
            var settlement = projectYearBalances.FirstOrDefault(b => b.BalanceCategory == (int)BalanceCategory.Settlement);
            if (settlement != null)
                model.Settlement = settlement.Balance;
            var retrievable = projectYearBalances.FirstOrDefault(b => b.BalanceCategory == (int)BalanceCategory.Retrievable);
            if (retrievable != null)
                model.Retrievable = retrievable.Balance;

            return model;
        }
    }
}
