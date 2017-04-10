using AppFramework.Security.Menus;
using AppFramework.Security.Repositories;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AppFramework.Security.Filters
{
    public static class OwinMenuExtension
    {
        public static ICollection<AppMenuItem> GetUserMenu(this IOwinContext context) {
            var user = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (!user.IsAuthenticated) {
                return new List<AppMenuItem>();
            }

            var userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            AppSecurityContext securityContext = new AppSecurityContext();
            long id = long.Parse(userId);

            var repository = new MenuItemRepository(securityContext);
            return repository.GetMenuByUsuarioId(id);
        }
    }
}
