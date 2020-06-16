using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Services
{
    public class PerformService : IPerformService
    {
        // GET: api/Performs
        //  query string:   ?year=&projectIds=
        //      year        : year filter
        //      projectIds  : projects filter (is a comma separated string of Ids of projects
        const string ROUTE_PERFORM = "api/Performs";
        const string FORMAT_QUERYSTRING_PERFORMS = "year={0}&projectIds={1}";

        const string ROUTE_PERFORMTERMS = "api/PerformTerms";  // add query ?performId=

        // GET: api/PerformTerms/GetTasks?orgId=1&year=2020&month=2
        // 注意： 返回一个PerformTerms类型的数组，其中，每个元素的Perform, Terms, Terms.Project是有效的对象。
        const string ROUTE_ORGTASKS_PERIOD = "api/PerformTerms/GetTasks";
        const string FORMAT_QUERYSTRING_ORGTASKS_PERIOD = "orgId={0}&year={1}&month={2}";

        private readonly IApiConnector _apiConnector;
        public PerformService(IApiConnector apiConnector)
        {
            _apiConnector = apiConnector;
        }

        public async Task DeletePerformTerms(int itemId) =>
            await _apiConnector.HttpDeleteAsync<PerformTerms>(ROUTE_PERFORMTERMS + $"/{itemId}");

        public async Task<IEnumerable<PerformTerms>> GetPerformTerms(int performId) =>
            await _apiConnector.HttpGetAsync<IEnumerable<PerformTerms>>(ROUTE_PERFORMTERMS, $"performId={performId}");

        public async Task<IEnumerable<Perform>> GetYearPerformsOfProject(int year, int projectId)
        {
            var queryString = string.Format(FORMAT_QUERYSTRING_PERFORMS, year, projectId);
            return await _apiConnector.HttpGetAsync<IEnumerable<Perform>>(ROUTE_PERFORM, queryString);
        }

        public async Task<IEnumerable<Perform>> GetYearPerformsOfProjects(int year, IEnumerable<int> projectIds)
        {
            var queryString = string.Format(FORMAT_QUERYSTRING_PERFORMS, year, string.Join(',', projectIds));
            return await _apiConnector.HttpGetAsync<IEnumerable<Perform>>(ROUTE_PERFORM, queryString);
        }

        public async Task SavePerform(Perform perform)
        {
            if (perform.Id > 0)
                await _apiConnector.HttpPutAsync<Perform, StatusCodeResult>(
                    ROUTE_PERFORM + $"/{perform.Id}", perform);
            else
                await _apiConnector.HttpPostAsync<Perform, Perform>(
                    ROUTE_PERFORM, perform);
        }

        public async Task SavePerformTerms(PerformTerms item)
        {
            if (item.Id > 0)
            {
                await _apiConnector.HttpPutAsync<PerformTerms, IActionResult>(ROUTE_PERFORMTERMS + $"/{item.Id}", item);
            }
            else
            {
                await _apiConnector.HttpPostAsync<PerformTerms, PerformTerms>(ROUTE_PERFORMTERMS, item);
            }
        }
        public async Task<IEnumerable<PerformTerms>> GetMonthlyTasksOfOrg(int orgId, int year, int month) =>
            await _apiConnector.HttpGetAsync<IEnumerable<PerformTerms>>(ROUTE_ORGTASKS_PERIOD,
                string.Format(FORMAT_QUERYSTRING_ORGTASKS_PERIOD, orgId, year, month));
    }
}
