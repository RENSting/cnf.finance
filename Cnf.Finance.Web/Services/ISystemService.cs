using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cnf.Finance.Entity;

namespace Cnf.Finance.Web.Services
{
    public interface ISystemService
    {
        Task<IEnumerable<Organization>> GetOrganizations();
        Task CreateOrganization(Organization organization);
        Task<Organization> FindOrganization(int id);

        Task UpdateOrganization(Organization organization);

        Task DeleteOrganization(int id);

        Task<IEnumerable<Users>> GetUsers(string login = "");

        Task<Users> FindUser(int userId);

        Task SaveUser(Users user);

        Task<Users> Authenticate(string login, string password);
    }
}
