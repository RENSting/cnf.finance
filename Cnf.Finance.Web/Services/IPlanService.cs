using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Services
{
    public interface IPlanService
    {
        Task<IEnumerable<Plan>> GetYearPlansOfProjects(int year, IEnumerable<int> projectIds);

        Task<IEnumerable<Plan>> GetYearPlansOfProject(int year, int projectId);

        Task SavePlan(Plan plan);
    }
}
