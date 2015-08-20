using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SampleApp.Models
{
    public class ApplicationUser : IUser<string>
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store) : base(store)
        {
        }
    }

    public class ApplicationRole : IRole<string>
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store)
        {
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(
            UserManager<ApplicationUser, string> userManager,
            IAuthenticationManager authenticationManager) : base(userManager, authenticationManager)
        {
        }
    }

    public class UserStore :
        IUserStore<ApplicationUser>,
        IUserStore<ApplicationUser, string>,
        IUserPasswordStore<ApplicationUser, string>,
        IUserRoleStore<ApplicationUser, string>,
        IRoleStore<ApplicationRole, string>
    {
        /// <summary>
        /// ユーザー保存先
        /// </summary>
        private static List<ApplicationUser> Users { get; } = new List<ApplicationUser>();
        /// <summary>
        /// ロールの保存先
        /// </summary>
        private static List<ApplicationRole> Roles { get; } = new List<ApplicationRole>();
        /// <summary>
        /// ユーザーとロールのリレーション
        /// </summary>
        private static List<Tuple<string, string>> UserRoleMap { get; } = new List<Tuple<string, string>>();

        /// <summary>
        /// ユーザーをロールに追加する
        /// </summary>
        public Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            Debug.WriteLine(nameof(AddToRoleAsync));
            var role = Roles.FirstOrDefault(x => x.Name == roleName);
            if (role == null) { throw new InvalidOperationException(); }

            var userRoleMap = UserRoleMap.FirstOrDefault(x => x.Item1 == user.Id && x.Item2 == role.Id);
            if (userRoleMap == null)
            {
                UserRoleMap.Add(Tuple.Create(user.Id, role.Id));
            }

            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーを作成する
        /// </summary>
        public Task CreateAsync(ApplicationUser user)
        {
            Debug.WriteLine(nameof(CreateAsync));
            Users.Add(user);
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーを削除する
        /// </summary>
        public Task DeleteAsync(ApplicationUser user)
        {
            Debug.WriteLine(nameof(DeleteAsync));
            Users.Remove(Users.First(x => x.Id == user.Id));
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// 何か後始末（DbContextとかDBのコネクションとか作ってたら後始末をする）
        /// </summary>
        public void Dispose()
        {
            Debug.WriteLine(nameof(Dispose));
        }

        /// <summary>
        /// ユーザーをId指定で取得する
        /// </summary>
        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            Debug.WriteLine(nameof(FindByIdAsync));
            var result = Users.FirstOrDefault(x => x.Id == userId);
            return Task.FromResult(result);
        }

        /// <summary>
        /// ユーザーをユーザー名で取得する
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Task<ApplicationUser> FindByNameAsync(string userName)
        {
            Debug.WriteLine(nameof(FindByNameAsync));
            var result = Users.FirstOrDefault(x => x.UserName == userName);
            return Task.FromResult(result);
        }

        /// <summary>
        /// ユーザーからパスワードのハッシュを取得する
        /// </summary>
        public Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            Debug.WriteLine(nameof(GetPasswordHashAsync));
            return Task.FromResult(user.Password);
        }

        /// <summary>
        /// ユーザーのロールを取得する
        /// </summary>
        public Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            Debug.WriteLine(nameof(GetRolesAsync));
            IList<string> roleNames = UserRoleMap.Where(x => x.Item1 == user.Id).Select(x => x.Item2)
                .Select(x => Roles.First(y => y.Id == x))
                .Select(x => x.Name)
                .ToList();
            return Task.FromResult(roleNames);
        }


        /// <summary>
        /// パスワードを持ってるか
        /// </summary>
        public Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            Debug.WriteLine(nameof(HasPasswordAsync));
            return Task.FromResult(user.Password != null);
        }

        /// <summary>
        /// ユーザーがロールに所属するか
        /// </summary>
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            Debug.WriteLine(nameof(IsInRoleAsync));
            var roles = await this.GetRolesAsync(user);
            return roles.FirstOrDefault(x => x.ToUpper() == roleName.ToUpper()) != null;
        }

        /// <summary>
        /// ユーザーをロールから削除する
        /// </summary>
        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            Debug.WriteLine(nameof(RemoveFromRoleAsync));
            var role = Roles.FirstOrDefault(x => x.Name == roleName);
            if (role == null) { return Task.FromResult(default(object)); }
            var userRoleMap = UserRoleMap.FirstOrDefault(x => x.Item1 == user.Id && x.Item2 == role.Id);
            if (userRoleMap != null)
            {
                UserRoleMap.Remove(userRoleMap);
            }
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザーにハッシュ化されたパスワードを設定する
        /// </summary>
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            Debug.WriteLine(nameof(SetPasswordHashAsync));
            user.Password = passwordHash;
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ユーザー情報を更新する
        /// </summary>
        public Task UpdateAsync(ApplicationUser user)
        {
            Debug.WriteLine(nameof(UpdateAsync));
            var r = Users.FirstOrDefault(x => x.Id == user.Id);
            if (r == null) { return Task.FromResult(default(object)); }
            r.UserName = user.UserName;
            r.Password = user.Password;
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ロールを作成します。
        /// </summary>
        public Task CreateAsync(ApplicationRole role)
        {
            Debug.WriteLine(nameof(CreateAsync) + " role");
            Roles.Add(role);
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ロールを更新します
        /// </summary>
        public Task UpdateAsync(ApplicationRole role)
        {
            Debug.WriteLine(nameof(UpdateAsync) + " role");
            var r = Roles.FirstOrDefault(x => x.Id == role.Id);
            if (r == null) { return Task.FromResult(default(object)); }
            r.Name = role.Name;
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ロールを削除します
        /// </summary>
        public Task DeleteAsync(ApplicationRole role)
        {
            Debug.WriteLine(nameof(DeleteAsync) + " role");
            var r = Roles.FirstOrDefault(x => x.Id == role.Id);
            if (r == null) { return Task.FromResult(default(object)); }
            Roles.Remove(r);
            return Task.FromResult(default(object));
        }

        /// <summary>
        /// ロールをIDで取得します。
        /// </summary>
        Task<ApplicationRole> IRoleStore<ApplicationRole, string>.FindByIdAsync(string roleId)
        {
            Debug.WriteLine(nameof(FindByIdAsync) + " role");
            var r = Roles.FirstOrDefault(x => x.Id == roleId);
            return Task.FromResult(r);
        }

        /// <summary>
        /// ロールを名前で取得します。
        /// </summary>
        Task<ApplicationRole> IRoleStore<ApplicationRole, string>.FindByNameAsync(string roleName)
        {
            Debug.WriteLine(nameof(FindByNameAsync) + " role");
            var r = Roles.FirstOrDefault(x => x.Name == roleName);
            return Task.FromResult(r);
        }
    }
}
