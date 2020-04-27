using Cnf.Finance.Entity;
using Cnf.Finance.Web.Models;
using Cnf.Finance.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Cnf.Finance.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISystemService _systemService;

        public HomeController(ILogger<HomeController> logger, ISystemService systemService)
        {
            _logger = logger;
            _systemService = systemService;
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


        public IActionResult Index()
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
