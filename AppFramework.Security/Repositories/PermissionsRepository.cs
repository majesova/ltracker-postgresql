using System.Linq;
using System.Data.Entity;

namespace AppFramework.Security.Repositories
{
    /// <summary>
    /// Operaciones con permisos
    /// </summary>
    public class PermissionRepository
    {
        private AppSecurityContext _context;
        public PermissionRepository(AppSecurityContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Devuelve todos los permisos en la base de datos
        /// </summary>
        /// <returns></returns>
        public IQueryable<AppPermission> GetAll() {
            return _context.Permissions.Include(x => x.Action).Include(x => x.Resource);
        }
        /// <summary>
        /// Agrega un permiso a contexto
        /// </summary>
        /// <param name="permission"></param>
        public void Add(AppPermission permission) {
            _context.Permissions.Add(permission);
        }

        /// <summary>
        /// Valida si entre los permisos contenidos en los roles del usuario está una combinación que satisfaga action - resource
        /// </summary>
        /// <param name="userId">Identificador del usuario</param>
        /// <param name="actionKey">Clave de la acción</param>
        /// <param name="resourceKey">Clave del recurso</param>
        /// <returns>SI/NO</returns>
        public bool HasPermission(long userId, string actionKey, string resourceKey) {

            var permissions = (from r in _context.UserRoles
                           join rolPerm in _context.RolesPermissions on r.RoleId equals rolPerm.RoleId
                           where r.UserId == userId
                           select new { ActionKey = rolPerm.Permission.Action.Key, ResourceKey = rolPerm.Permission.Resource.Key }).Distinct();

            foreach ( var appPerm in permissions)
            {
                if (appPerm.ActionKey.ToLower().CompareTo(actionKey.ToLower()) == 0 && appPerm.ResourceKey.ToLower().CompareTo(resourceKey.ToLower()) == 0)
                    return true;
            }

            return false;
        }

    }
}
