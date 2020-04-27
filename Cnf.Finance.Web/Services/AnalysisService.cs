using Cnf.Finance.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Services
{
    public class AnalysisService : IAnalysisService
    {
        private readonly IApiConnector _apiConnector;

        public AnalysisService(IApiConnector apiConnector)
        {
            _apiConnector = apiConnector;
        }

        // GET: api/Organizations/YearGroupReport?year=&month=
        const string ROUTE_YEAR_GROUP_REPORT = "api/Organizations/YearGroupReport";
        const string FORMAT_QUERY_YEAR_MONTH = "year={0}&month={1}";

        // GET: api/Projects/YearGroupReport?orgId=&year=month
        //      query string parameters:
        //          orgId   =   1       : group projects of the organization
        //          year    =   2020    : report of the year
        //          month   =   4       : show the month
        const string ROUTE_YEAR_ORG_REPORT = "api/Projects/YearGroupReport";
        const string FORMAT_QUERY_ORG_YEAR_MONTH = "orgId={0}&year={1}&month={2}";

        public async Task<IEnumerable<YearGroupRecord>> GetYearGroupReport(int year, int month) => 
            await _apiConnector.HttpGetAsync<IEnumerable<YearGroupRecord>>(
                    ROUTE_YEAR_GROUP_REPORT, string.Format(FORMAT_QUERY_YEAR_MONTH, year, month));

        public async Task<IEnumerable<YearGroupRecord>> GetYearOrgReport(int orgId, int year, int month) =>
            await _apiConnector.HttpGetAsync<IEnumerable<YearGroupRecord>>(
                    ROUTE_YEAR_ORG_REPORT, string.Format(FORMAT_QUERY_ORG_YEAR_MONTH, orgId, year, month));
    }
}
