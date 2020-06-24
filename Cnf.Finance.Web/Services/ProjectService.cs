using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Cnf.Finance.Web.Services
{
    public class ProjectService : IProjectService
    {
        // GET: api/Projects
        // GET: api/Projects/5?content=null|0|1|2|3 
        //    Querystring Parameters:
        //      content =   null|0      : base only;
        //                  1           : has balance and terms
        //                  2           : has plans and performs
        //                  3           : has all
        // PUT:  api/Projects/5
        // POST: api/Projects
        // DELETE: api/Projects/5
        const string ROUTE_PROJECT = "api/Projects";
        //  query string : ?orgId=&searchName=&activeOnly=
        //      orgId       :   Organization filter
        //      searchName  :   Project Name filter
        //      activeOnly  :   bool
        const string FORMAT_QUERYSTRING_SEARCH_PROJECT = "orgId={0}&searchName={1}&activeOnly={2}";

        // GET: api/AnnualBalances?projectId=&year=&porjectsIds
        //  query string:
        //      projectId   : unique project filter (int)
        //      year        : year filter
        //      projectIds  : projects filter (is a comma separated string of Ids of projects
        // GET: api/AnnualBalances/5
        // PUT: api/AnnualBalances/5
        // POST: api/AnnualBalances
        // DELETE: api/AnnualBalances/5
        const string ROUTE_BALANCE = "api/AnnualBalances"; // can add a querystring ?projectId={0}&year={1}";
        const string FORMAT_QUERYSTRING_GET_BALANCE = "projectId={0}&year={1}";
        const string FORMAT_QUERYSTRING_GET_BALANCE_PRJs = "year={0}&projectIds={1}";

        // GET: api/Projects/SearchProject
        //  query string : ?orgId=&searchName=&activeOnly=&pageIndex=&pageSize=
        //      orgId       :   Organization filter
        //      searchName  :   Project Name filter
        //      activeOnly  :   bool
        //      pageIndex   :   int based=0
        //      pageSize    :   int default=10
        const string ROUTE_PAGED_PROJECTS = "api/Projects/SearchProject";
        const string FORMAT_QUERYSTRING_PAGED_PROJECTS = "orgId={0}&searchName={1}&activeOnly={2}&pageIndex={3}&pageSize={4}";


        // GET: api/Terms
        // GET: api/Terms/5
        // PUT: api/Terms/5
        // POST: api/Terms
        // DELETE: api/Terms/5
        const string ROUTE_TERMS = "api/Terms";

        private readonly IApiConnector _apiConnector;
        public ProjectService(IApiConnector apiConnector)
        {
            _apiConnector = apiConnector;
        }

        public async Task<IEnumerable<Project>> SearchProjectsWithoutPager(
            int? orgId = default, string searchName = default, bool? activeOnly = default) =>
            await _apiConnector.HttpGetAsync<IEnumerable<Project>>(ROUTE_PROJECT,
                    string.Format(FORMAT_QUERYSTRING_SEARCH_PROJECT, orgId, HttpUtility.UrlEncode(searchName), activeOnly));

        public async Task<SearchResult<Project>> SearchProjects(
            int? orgId = default, string searchName = default, bool? activeOnly = default,
            int pageIndex = 0, int pageSize = 10) =>
            await _apiConnector.HttpGetAsync<SearchResult<Project>>(ROUTE_PAGED_PROJECTS,
                    string.Format(FORMAT_QUERYSTRING_PAGED_PROJECTS, orgId, HttpUtility.UrlEncode(searchName), activeOnly, pageIndex, pageSize));

        public async Task<Project> FindProject(int projectId) => 
            await _apiConnector.HttpGetAsync<Project>(ROUTE_PROJECT + $"/{projectId}");

        public async Task CreateProject(Project project)
        {
            await _apiConnector.HttpPostAsync<Project, Project>(ROUTE_PROJECT, project);
        }

        public async Task UpdateProject(Project project)
        {
            if (project.ProjectId <= 0)
                throw new Exception("不能提交保存尚不存在的项目");

            await _apiConnector.HttpPutAsync<Project, StatusCodeResult>(ROUTE_PROJECT + $"/{project.ProjectId}", project);
        }

        public async Task<Project> RetriveProjectWithDetails(int projectId, ProjectApiContent content) =>
            await _apiConnector.HttpGetAsync<Project>(ROUTE_PROJECT + $"/{projectId}", $"content={(int)content}");

        public async Task<IEnumerable<AnnualBalance>> GetAnnualBalances(int? projectId, int? year) => 
            await _apiConnector.HttpGetAsync<IEnumerable<AnnualBalance>>(ROUTE_BALANCE, string.Format(FORMAT_QUERYSTRING_GET_BALANCE, projectId, year));

        public async Task<IEnumerable<AnnualBalance>> GetAnnualBalancesOfProjects(int year, IEnumerable<int> projectIds)
        {
            var queryString = string.Format(FORMAT_QUERYSTRING_GET_BALANCE_PRJs, year, string.Join(',', projectIds));
            return await _apiConnector.HttpGetAsync<IEnumerable<AnnualBalance>>(ROUTE_BALANCE, queryString);
        }

        public async Task SaveBalance(AnnualBalance balance)
        {
            if (balance.Id > 0)
                await _apiConnector.HttpPutAsync<AnnualBalance, StatusCodeResult>(
                    ROUTE_BALANCE + $"/{balance.Id}", balance);
            else
                await _apiConnector.HttpPostAsync<AnnualBalance, AnnualBalance>(
                    ROUTE_BALANCE, balance);
        }

        public async Task DeleteBalance(int annualBalanceId) =>
            await _apiConnector.HttpDeleteAsync<AnnualBalance>(ROUTE_BALANCE + $"/{annualBalanceId}");

        public async Task SaveTerms(Terms terms)
        {
            if (terms.Id > 0)
                await _apiConnector.HttpPutAsync<Terms, StatusCodeResult>(
                    ROUTE_TERMS + $"/{terms.Id}", terms);
            else
                await _apiConnector.HttpPostAsync<Terms, Terms>(
                    ROUTE_TERMS, terms);
        }

        public async Task DeleteTerms(int termsId) =>
            await _apiConnector.HttpDeleteAsync<Terms>(ROUTE_TERMS + $"/{termsId}");
    }
}
