using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cnf.Finance.Web.Services;
using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class SystemController : Controller
    {
        private readonly ISystemService _systemService;

        public SystemController(ISystemService systemService)
        {
            _systemService = systemService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Organizations()
        {
            var orgs = await _systemService.GetOrganizations();
            return View(orgs);
        }

        public IActionResult CreateOrganization()
        {
            //var org = new Organization();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrganization(Organization organization)
        {
            if (ModelState.IsValid)
            {
                await _systemService.CreateOrganization(organization);
                return RedirectToAction(nameof(Organizations));
            }
            return View(organization);
        }

        [HttpGet]
        public async Task<IActionResult> EditOrganization(int id)
        {
            var organization = await _systemService.FindOrganization(id);
            return View(organization);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrganization(int id, Organization organization)
        {
            if (ModelState.IsValid)
            {
                if (id != organization.OrganizationId)
                {
                    ModelState.AddModelError("", "提交的单位模型不一致");
                }
                else
                {
                    await _systemService.UpdateOrganization(organization);
                    return RedirectToAction(nameof(Organizations));
                }
            }
            return View(organization);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrganization(int id, Organization organization)
        {
            try
            {
                await _systemService.DeleteOrganization(id);
                return RedirectToAction(nameof(Organizations));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(nameof(EditOrganization), organization);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _systemService.GetUsers();

            ViewBag.OrgList = new SelectList(await _systemService.GetOrganizations(),
                    nameof(Organization.OrganizationId), nameof(Organization.Name));

            return View(users.Select(u => (UserViewModel)u));
        }

        public async Task<IActionResult> EditUser(int? id)
        {
            var user = (id == null || id.Value <= 0) ? new UserViewModel
            {
                ActiveStatus = true,
                Role = UserRole.None,
            } : await _systemService.FindUser(id.Value);

            ViewBag.OrgList = new SelectList(await _systemService.GetOrganizations(),
                    nameof(Organization.OrganizationId), nameof(Organization.Name));

            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(UserViewModel model)
        {
            ViewBag.OrgList = new SelectList(await _systemService.GetOrganizations(),
                    nameof(Organization.OrganizationId), nameof(Organization.Name));

            if (ModelState.IsValid)
            {
                var checkUsers = await _systemService.GetUsers(model.Login);
                bool isDuplicated = false;
                if (checkUsers.Count() > 0)
                {
                    if (model.UserId <= 0)
                        isDuplicated = true;
                    else
                    {
                        if (model.UserId != checkUsers.FirstOrDefault().UserId)
                            isDuplicated = true;
                    }
                }
                if (isDuplicated)
                {
                    ModelState.AddModelError("", "登录账户重复");
                    return View(model);
                }
                model.Role = (model.IsSystemAdmin ? UserRole.SystemAdmin : UserRole.None)
                    | (model.IsPlanner ? UserRole.Planner : UserRole.None)
                    | (model.IsReporter ? UserRole.Reporter : UserRole.None)
                    | (model.IsSupervisor ? UserRole.Supervisor : UserRole.None);

                var user = model.UserId > 0 ? await _systemService.FindUser(model.UserId)
                    : new Users
                    {
                        Password = model.Password,
                    };
                user.Login = model.Login;
                user.OrganizationId = model.OrganizationId;
                user.Role = (int)model.Role;
                user.UserName = model.UserName;
                user.ActiveStatus = model.ActiveStatus;

                await _systemService.SaveUser(user);
                return RedirectToAction(nameof(Users));
            }
            return View(model);
        }
    }
}