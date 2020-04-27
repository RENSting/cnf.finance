using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cnf.Finance.Entity
{
    public partial class Project
    {
        public Project()
        {
            //AnnualBalance = new List<AnnualBalance>(); // new HashSet<AnnualBalance>();
            //Perform = new List<Perform>();  // new HashSet<Perform>();
            //Plan = new List<Plan>();    // new HashSet<Plan>();
            //Terms = new List<Terms>();  // new HashSet<Terms>();
            AnnualBalance = new HashSet<AnnualBalance>();
            Perform = new HashSet<Perform>();
            Plan = new HashSet<Plan>();
            Terms = new HashSet<Terms>();
        }

        [Display(Name = "项目ID")]
        public int ProjectId { get; set; }

        [Display(Name = "项目名称")]
        [Required(ErrorMessage = "必须输入名称")]
        public string Name { get; set; }

        [Display(Name = "简称")]
        [Required(ErrorMessage = "必须输入简称")]
        [MaxLength(10, ErrorMessage = "项目简称不能超过10个汉字")]
        public string ShortName { get; set; }

        [Display(Name = "主合同编号")]
        public string ContractCode { get; set; }

        [Display(Name="预计总收入(万)")]
        [DisplayFormat(DataFormatString ="{0:#.####}")]
        public decimal ContractAmount { get; set; }

        [Display(Name="预计毛利率(%)")]
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double? EstimatingProfit { get; set; }

        [Display(Name="项目经理")]
        public string ProjectManager { get; set; }

        [Display(Name ="所属单位")]
        public int OrganizationId { get; set; }

        [Display(Name="开工日期"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "完工日期"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? EndDate { get; set; }

        [Display(Name="项目状态")]
        public int Status { get; set; }

        [Display(Name = "是否涉诉")]
        public bool HasProblem { get; set; }

        [Display(Name="执行中")]
        public bool ActiveStatus { get; set; }

        [Display(Name = "关闭日期"), DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? CloseDate { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual ICollection<AnnualBalance> AnnualBalance { get; set; }
        public virtual ICollection<Terms> Terms { get; set; }
        public virtual ICollection<Perform> Perform { get; set; }
        public virtual ICollection<Plan> Plan { get; set; }
    }
}
