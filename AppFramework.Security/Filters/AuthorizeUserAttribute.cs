using AppFramework.Security.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AppFramework.Security.Filters
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        
        public string Action { get; set; }
        public string Resource { get; set; }

        private AuthorizationContext _currentContext;

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            _currentContext = filterContext;
            base.OnAuthorization(filterContext);
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (!user.IsAuthenticated) {
                return false;
            }
            var userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            AppSecurityContext context = new AppSecurityContext();
            long id = long.Parse(userId);
            var repository = new AppPermissionsRepository(context);
            var valid = repository.HasPermission(id, Action, Resource);
            return valid;
        }

    }
}
