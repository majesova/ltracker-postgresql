using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using AppFramework.Security.Repositories;
using System.Web;

namespace AppFramework.Security.Filters
{
    public static class SecurityExtension
    {
        public static bool HasPermission(this IOwinContext context, string actionKey, string resourceKey)
        {
            var user = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (!user.IsAuthenticated)
            {
                return false;
            }
            var userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            AppSecurityContext contextSecurity = new AppSecurityContext();
            long id = long.Parse(userId);
            var repository = new PermissionRepository(contextSecurity);
            var valid = repository.HasPermission(id, actionKey, resourceKey);
            return valid;
        }
    }
}
