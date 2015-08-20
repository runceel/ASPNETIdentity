using Microsoft.AspNet.Identity.Owin;
using SampleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SampleApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(ApplicationUser parameter, string returnUrl)
        {
            var userManager = this.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = await userManager.FindAsync(parameter.UserName, parameter.Password);
            if (user == null)
            {
                return View(parameter);
            }

            var signInManager = this.HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            await signInManager.SignInAsync(user, false, false);

            return Redirect(returnUrl);
        }
    }
}