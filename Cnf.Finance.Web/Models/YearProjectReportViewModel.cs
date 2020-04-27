using Cnf.Finance.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Models
{
    public class YearProjectReportViewModel
    {
        [Display(Name = "年度")]
        public int Year { get; set; }

        [Display(Name = "月份")]
        public int Month { get; set; }

        /// <summary>
        /// Organization ID
        /// </summary>
        public int OrgId { get; set; }

        public Project Project { get; set; }

        public ProjectStatus GetProjectStatus()
        {
            if (Project == null)
                return ProjectStatus.Processing;
            else
                return (ProjectStatus)Project.Status;
        }
    }
}
