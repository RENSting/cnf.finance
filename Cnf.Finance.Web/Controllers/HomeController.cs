using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;
using Cnf.Finance.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISystemService _systemService;
        private readonly IPlanService _planService;
        private readonly IPerformService _performService;

        public HomeController(ILogger<HomeController> logger, ISystemService systemService,
                        IPlanService planService, IPerformService performService)
        {
            _logger = logger;
            _systemService = systemService;
            _planService = planService;
            _performService = performService;
        }

        [AllowAnonymous]

        public IActionResult Auth()
        {
            Helper.Signout(HttpContext);

            var model = new AuthViewModel
            {
                HasChecked = false
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Auth(AuthViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Users user = await _systemService.Authenticate(model.UserName, model.Password);
            if (user == null || user.UserId <= 0 || user.ActiveStatus == false)
            {
                model.HasChecked = true;
                return View(model);
            }
            else
            {
                //_logger.LogInformation("before sign in");

                Helper.Signin(user, HttpContext);

                //_logger.LogInformation("after sign in");

                return RedirectToAction(nameof(Index));
            }
        }

        [AllowAnonymous]
        public ActionResult Denied()
        {
            return Content("当前用户不具有需要的权限访问指定的资源");
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Helper.Signout(HttpContext);
            return RedirectToAction(nameof(Auth));
        }

        public async Task<IActionResult> ChangePassword()
        {
            int currentUserId = Helper.GetUserID(HttpContext);
            ChangePasswordViewModel model = await _systemService.FindUser(currentUserId);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _systemService.Authenticate(model.Login, model.OldPassword);
                if (user == null || user.UserId <= 0)
                {
                    ModelState.AddModelError("", "修改口令遇到错误");
                }
                else
                {
                    user.Password = model.NewPassword;
                    await _systemService.SaveUser(user);
                    return RedirectToAction(nameof(Logout));
                }
            }
            return View(model);
        }


        public async Task<IActionResult> Index(TaskIndexViewModel model)
        {
            if(model == null)
            {
                model = new TaskIndexViewModel();
            }

            if (model.Year <= 0)
                model.Year = DateTime.Today.Year;
            if (model.Month <= 0)
                model.Month = DateTime.Today.Month;

            var allowAllOrgs = Helper.AllowAllOrgs(HttpContext, out int? allowedOrgId);
            var orgnizations = allowAllOrgs ? await _systemService.GetOrganizations() :
                                new Organization[] { await _systemService.FindOrganization(allowedOrgId.Value) };

            if (!allowAllOrgs)
                model.OrganizationId = allowedOrgId.Value;
            else
            {
                if (model.OrganizationId <= 0)
                    model.OrganizationId = orgnizations.FirstOrDefault().OrganizationId;
            }

            ViewBag.OrgList = new SelectList(orgnizations,
                nameof(Organization.OrganizationId), nameof(Organization.Name));

            var planTerms = await _planService.GetMonthlyTasksOfOrg(model.OrganizationId, model.Year, model.Month);

            model.BindPlanTasks(planTerms);

            var performTerms = await _performService.GetMonthlyTasksOfOrg(model.OrganizationId, model.Year, model.Month);

            model.BindPerformTasks(performTerms);

            return View(model);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
