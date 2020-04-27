using Cnf.Finance.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cnf.Finance.Web.Services
{
    public class PlanService:IPlanService
    {
        // GET: api/Plans
        //  query string:   ?year=&projectIds=
        //      year        : year filter
        //      projectIds  : projects filter (is a comma separated string of Ids of projects
        const string ROUTE_PLAN = "api/Plans";
        const string FORMAT_QUERYSTRING_PLANS = "year={0}&projectIds={1}";

        private readonly IApiConnector _apiConnector;
        public PlanService(IApiConnector apiConnector)
        {
            _apiConnector = apiConnector;
        }

        public async Task<IEnumerable<Plan>> GetYearPlansOfProjects(int year, IEnumerable<int> projectIds)
        {
            var queryString = string.Format(FORMAT_QUERYSTRING_PLANS, year, string.Join(',', projectIds));
            return await _apiConnector.HttpGetAsync<IEnumerable<Plan>>(ROUTE_PLAN, queryString);
        }

        public async Task<IEnumerable<Plan>> GetYearPlansOfProject(int year, int projectId)
        {
            var queryString = string.Format(FORMAT_QUERYSTRING_PLANS, year, projectId);
            return await _apiConnector.HttpGetAsync<IEnumerable<Plan>>(ROUTE_PLAN, queryString);
        }

        public async Task SavePlan(Plan plan)
        {
            if (plan.Id > 0)
                await _apiConnector.HttpPutAsync<Plan, StatusCodeResult>(
                    ROUTE_PLAN + $"/{plan.Id}", plan);
            else
                await _apiConnector.HttpPostAsync<Plan, Plan>(
                    ROUTE_PLAN, plan);
        }
    }
}
