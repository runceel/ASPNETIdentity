using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using SampleApp.Models;

[assembly: OwinStartup(typeof(SampleApp.Startup))]

namespace SampleApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Home/Login")
            });

            app.CreatePerOwinContext<UserStore>(() => new UserStore());
            app.CreatePerOwinContext<ApplicationUserManager>((options, context) => new ApplicationUserManager(context.Get<UserStore>()));
            app.CreatePerOwinContext<ApplicationRoleManager>((options, context) => new ApplicationRoleManager(context.Get<UserStore>()));
            app.CreatePerOwinContext<ApplicationSignInManager>((options, context) => 
                new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication));
        }
    }
}
