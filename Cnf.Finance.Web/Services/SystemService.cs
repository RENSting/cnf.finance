using Cnf.Finance.Entity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Services
{
    public class SystemService : ISystemService
    {
        const string ROUTE_GET_ALL_ORGs = "api/Organizations";
        const string ROUTE_GET_ORG = "api/Organizations";       //  need add /5
        const string ROUTE_POST_ORG = "api/Organizations";
        const string ROUTE_PUT_ORG = "api/Organizations";       // need add  /5
        const string ROUTE_DELETE_ORG = "api/Organizations";    // need add     /5

        const string ROUTE_USERS = "api/Users";
        const string FORMAT_ROUTE_AUTH = "api/Users/Authenticate?login={0}&pwd={1}";

        private readonly IApiConnector _apiConnector;

        public SystemService(IApiConnector apiConnector) =>
            _apiConnector = apiConnector;

        public async Task<Users> Authenticate(string login, string password)=>
            await _apiConnector.HttpGetAsync<Users>(
                string.Format(FORMAT_ROUTE_AUTH, login, HttpUtility.UrlEncode(password)));

        public async Task CreateOrganization(Organization organization)
        {
            await _apiConnector.HttpPostAsync<Organization, Organization>(ROUTE_POST_ORG, organization);
        }

        public async Task DeleteOrganization(int id)
        {
            await _apiConnector.HttpDeleteAsync<Organization>(ROUTE_DELETE_ORG + $"/{id}");
        }

        public async Task<Organization> FindOrganization(int id)
        {
            string route = ROUTE_GET_ORG + $"/{id}";
            return await _apiConnector.HttpGetAsync<Organization>(route);
        }

        public async Task<Users> FindUser(int userId) =>
            await _apiConnector.HttpGetAsync<Users>(ROUTE_USERS + $"/{userId}");

        public async Task<IEnumerable<Organization>> GetOrganizations()
        {
            return await _apiConnector.HttpGetAsync<IEnumerable<Organization>>(ROUTE_GET_ALL_ORGs);
        }

        public async Task<IEnumerable<Users>> GetUsers(string login = "") =>
            await _apiConnector.HttpGetAsync<IEnumerable<Users>>(ROUTE_USERS, $"login={login}");

        public async Task SaveUser(Users user)
        {
            if (user.UserId > 0)
                await _apiConnector.HttpPutAsync<Users, StatusCodeResult>(ROUTE_USERS + $"/{user.UserId}", user);
            else
                await _apiConnector.HttpPostAsync<Users, Users>(ROUTE_USERS, user);
        }

        public async Task UpdateOrganization(Organization organization)
        {
            if (organization.OrganizationId <= 0)
                throw new Exception("不能提交保存尚不存在的单位");

            await _apiConnector.HttpPutAsync<Organization, StatusCodeResult>(ROUTE_PUT_ORG + $"/{organization.OrganizationId}", organization);
        }
    }
}
