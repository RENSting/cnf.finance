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

        const string ROUTE_PLANTERMS = "api/PlanTerms";

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

        public async Task<Plan> FindPlan(int id) =>
            await _apiConnector.HttpGetAsync<Plan>(ROUTE_PLAN + $"/{id}");

        public async Task SavePlanTerms(PlanTerms task)
        {
            if (task.Id > 0)
            {
                await _apiConnector.HttpPutAsync<PlanTerms, IActionResult>(ROUTE_PLANTERMS + $"/{task.Id}", task);
            }
            else
            {
                await _apiConnector.HttpPostAsync<PlanTerms, PlanTerms>(ROUTE_PLANTERMS, task);
            }
        }

        public async Task DeletePlanTerms(int planTermsId) =>
            await _apiConnector.HttpDeleteAsync<PlanTerms>(ROUTE_PLANTERMS + $"/{planTermsId}");

        public async Task<IEnumerable<PlanTerms>> GetPlanTerms(int planId) =>
            await _apiConnector.HttpGetAsync<IEnumerable<PlanTerms>>(ROUTE_PLANTERMS, $"planId={planId}");
    }
}
