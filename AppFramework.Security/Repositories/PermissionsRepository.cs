using System.Linq;

namespace AppFramework.Security.Repositories
{
    public class AppPermissionsRepository
    {
        private AppSecurityContext _context;
        public AppPermissionsRepository(AppSecurityContext context)
        {
            _context = context;
        }

        public bool HasPermission(long userId, string actionName, string resourceName) {

            var permissions = (from r in _context.Roles
                       join userRoles in _context.UserRoles on r.Id equals userRoles.RoleId
                       join rolPerm in _context.RolesPermissions on userRoles.RoleId equals rolPerm.RoleId
                       join perms in _context.Permissions on rolPerm.PermissionId equals perms.Id
                       where userRoles.UserId == userId
                       select new { Action = perms.Action.Name, Resource = perms.Resource.Name }).Distinct();

            foreach( var appPerm in permissions)
            {
                if (appPerm.Action.ToLower().CompareTo(actionName.ToLower()) == 0 && appPerm.Resource.ToLower().CompareTo(resourceName.ToLower()) == 0)
                    return true;
            }

            return false;
        }

    }
}
