using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Cnf.Finance.Entity
{
    public class YearGroupRecord
    {
        public int Id { get; set; }
        public string NameHtml { get; set; }
        /// <summary>
        /// 用来作为徽章的标记，往往用来说明此记录的特点
        /// </summary>
        public string BadgeHtml { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public virtual YearReportBalance AnnualBalance { get; set; }

        /// <summary>
        /// 累计数
        /// </summary>
        public virtual ReportItem Accumulation { get; set; }

        /// <summary>
        /// 当月数
        /// </summary>
        public virtual ReportItem CurrentMonth { get; set; }

        public static YearGroupRecord Create(Organization organization, int year, int month) =>
            new YearGroupRecord
            {
                Id = organization.OrganizationId,
                NameHtml = organization.Name,
                Year = year,
                Month = month,
            };

        public static YearGroupRecord Create(Project project, int year, int month) =>
            new YearGroupRecord
            {
                Id = project.ProjectId,
                NameHtml = $"<span>{project.ShortName}</span>",
                Year = year,
                Month = month,
            };
    }
}
