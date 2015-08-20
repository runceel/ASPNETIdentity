using SampleApp.Models;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;

namespace SampleApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // ユーザーとロールの初期化
            // ロールの作成
            var roleManager = new ApplicationRoleManager(new UserStore());
            await roleManager.CreateAsync(new ApplicationRole { Name = "admin" });
            await roleManager.CreateAsync(new ApplicationRole { Name = "users" });

            var userManager = new ApplicationUserManager(new UserStore());
            // 一般ユーザーの作成
            await userManager.CreateAsync(new ApplicationUser { UserName = "tanaka" }, "p@ssw0rd");
            await userManager.AddToRoleAsync(
                (await userManager.FindByNameAsync("tanaka")).Id,
                "users");
            // 管理者の作成
            await userManager.CreateAsync(new ApplicationUser { UserName = "super_tanaka" }, "p@ssw0rd");
            await userManager.AddToRoleAsync(
                (await userManager.FindByNameAsync("super_tanaka")).Id,
                "users");
            await userManager.AddToRoleAsync(
                (await userManager.FindByNameAsync("super_tanaka")).Id,
                "admin");

            Debug.WriteLine("-----------");
        }
    }
}
