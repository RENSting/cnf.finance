using Cnf.Finance.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Services
{
    public interface IAnalysisService
    {
        Task<IEnumerable<YearGroupRecord>> GetYearGroupReport(int year, int month);

        Task<IEnumerable<YearGroupRecord>> GetYearOrgReport(int orgId, int year, int month);
    }
}
