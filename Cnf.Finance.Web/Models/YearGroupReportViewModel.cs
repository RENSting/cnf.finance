using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public enum GroupHirarchy
    {
        [Display(Name = "单位")] Organization,
        [Display(Name = "项目")] Project,
    }
    public class YearGroupReportViewModel
    {
        [Display(Name = "年度")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        /// <summary>
        /// 定义了用于穿透的层级：单位=>项目
        /// </summary>
        public GroupHirarchy? Hirarchy { get; set; }

        /// <summary>
        /// 用于穿透后存放父级对象的Id
        /// </summary>
        public int? GroupId { get; set; }
        /// <summary>
        /// 用于穿透后存放父级对象的名称
        /// </summary>
        public string GroupName{get;set;}

        public IEnumerable<YearGroupRecord> GroupRecord { get; set; }
    }
}
