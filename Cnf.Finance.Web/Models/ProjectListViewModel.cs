using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public class ProjectListViewModel
    {
        [Display(Name ="所属单位")]
        public int? SelectedOrgId { get; set; }
        
        [Display(Name ="名称")]
        public string SearchName { get; set; }

        [Display(Name ="包含已关闭")]
        public bool IncludeInActive { get; set; }

        public int Pages { get; set; }

        public int PageIndex { get; set; }

        [Display(Name ="每页条数")]
        public int PageSize { get; set; }

        public IEnumerable<ProjectViewModel> Projects { get; set; }
    }
}
