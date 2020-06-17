using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public enum TermsCategory
    {
        [Display(Name ="结算条款")]
        Settle,
        [Display(Name ="收款条件")]
        Retrieve,
        [Display(Name ="综合条款")]
        Others
    }

    public class TermsViewModel
    {
        public int TermsId { get; set; }
        public int ProjectId { get; set; }

        [Display(Name ="类型")]
        public TermsCategory Category { get; set; }

        [Display(Name ="条件")]
        [Required(ErrorMessage ="必须输入合同定义需要满足的条件")]
        public string Provision { get; set; }

        [Display(Name ="金额(万)")]
        [Required(ErrorMessage ="必须输入目标金额，没有请输入0")]
        [RegularExpression(@"^(\-|\+)?\d+(\.\d+)?$", ErrorMessage = "必须输入数字")]
        public decimal Amount { get; set; }

        [Display(Name ="触发日期")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}")]
        [Required(ErrorMessage ="必须选择条件触发日期")]
        public DateTime OnDate { get; set; }

        [Display(Name ="附注")]
        public string Remarks { get; set; }

        public static implicit operator TermsViewModel(Terms terms)=>
            new TermsViewModel
            {
                ProjectId = terms.ProjectId,
                Amount = terms.TargetAmount.Value,
                Category = (TermsCategory)terms.TermsCategory,
                OnDate = terms.TargetDate.Value,
                Provision = terms.Provision,
                Remarks = terms.Remarks,
                TermsId = terms.Id
            };
    }
}
