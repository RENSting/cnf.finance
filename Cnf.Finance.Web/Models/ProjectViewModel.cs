using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Models
{
    public enum ProjectStatus
    {
        [Display(Name = "在建工程")]
        Processing = 1,
        [Display(Name = "已完工未结算")]
        WaitForSettle = 2,
        [Display(Name = "已完工已结算")]
        SettleCompleted = 3,
        [Display(Name = "停工")]
        Stopped = 4
    }

    public enum ProjectApiContent
    {
        // Querystring Parameters:
        //      content =   null|0      : base only;
        //                  1           : has balance and terms
        //                  2           : has plans and performs
        //                  3           : has all
        ProjectOnly = 0,
        WithBalanceAndTerms = 1,
        WithPlanAndPerform = 2,
        RetrieveAll = 3
    }

    public class ProjectViewModel : Project
    {
        public static ProjectViewModel Create(Project project, IEnumerable<Organization> organizations)
        {
            var viewModel = JsonConvert.DeserializeObject<ProjectViewModel>(JsonConvert.SerializeObject(project));
            viewModel.OrganizationName = organizations.First(o => o.OrganizationId == viewModel.OrganizationId).Name;
            viewModel.ProjectStatus = GetProjectStatus()[viewModel.Status];
            return viewModel;
        }

        [Display(Name = "所属单位")]
        public string OrganizationName { get; set; }

        [Display(Name = "状态")]
        public string ProjectStatus { get; set; }

        /// <summary>
        /// 返回一个项目状态的字典，其Key对应于项目的Status属性
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, string> GetProjectStatus() =>
            new Dictionary<int, string>
            {
                {1, "在建工程" },
                {2, "已完工未结算" },
                {3, "已完工已结算" },
                {4, "停工" },
            };
    }
}
