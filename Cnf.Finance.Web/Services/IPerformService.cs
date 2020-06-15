
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;

namespace Cnf.Finance.Web.Services
{
    public interface IPerformService
    {
        Task<IEnumerable<Perform>> GetYearPerformsOfProjects(int year, IEnumerable<int> projectIds);

        Task<IEnumerable<Perform>> GetYearPerformsOfProject(int year, int projectId);

        Task SavePerform(Perform perform);
        Task<IEnumerable<PerformTerms>> GetPerformTerms(int performId);

        Task SavePerformTerms(PerformTerms item);

        Task DeletePerformTerms(int itemId);
    }
}
