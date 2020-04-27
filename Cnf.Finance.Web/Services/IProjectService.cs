using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;

namespace Cnf.Finance.Web.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> SearchProjects(int? orgId = default, string searchName = default, bool? activeOnly = default);
        Task CreateProject(Project project);
        Task<Project> FindProject(int projectId);
        /// <summary>
        /// 读取项目的同时，填充它的导航子集：Terms和AnnualBalance
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        Task<Project> RetriveProjectWithDetails(int projectId, ProjectApiContent content);
        Task UpdateProject(Project project);

        /// <summary>
        /// 返回结转余额数据（可根据某一项目、某一年度进行筛选）
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        Task<IEnumerable<AnnualBalance>> GetAnnualBalances(int? projectId, int? year);

        /// <summary>
        /// 同时返回一批项目的指定年度的结转余额
        /// </summary>
        /// <param name="year"></param>
        /// <param name="projectIds"></param>
        /// <returns></returns>
        Task<IEnumerable<AnnualBalance>> GetAnnualBalancesOfProjects(int year, IEnumerable<int> projectIds);

        Task SaveBalance(AnnualBalance balance);

        Task DeleteBalance(int annualBalanceId);
        Task SaveTerms(Terms terms);
        Task DeleteTerms(int termsId);
    }
}
