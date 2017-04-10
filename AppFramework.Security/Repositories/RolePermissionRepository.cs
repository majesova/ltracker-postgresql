using System.Linq;
using System.Data.Entity;

namespace AppFramework.Security.Repositories
{
    /// <summary>
    /// Operaciones RolePermission con la base de datos
    /// </summary>
    public class RolePermissionRepository
    {
        private AppSecurityContext _context;

        public RolePermissionRepository(AppSecurityContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Agrega un RolPermission al contexto
        /// </summary>
        /// <param name="rolePermission"></param>
        public void Add(AppRolePermission rolePermission)
        {
            _context.RolesPermissions.Add(rolePermission);
        }
        /// <summary>
        /// Obtiene los permisos de un rol específico
        /// </summary>
        /// <param name="roleId">Identificador del rol</param>
        /// <returns>Permisos del rol</returns>
        public IQueryable<AppPermission> GetPermissionByRoleId(long roleId) {

            var permissions = from role in _context.Roles
            join rolPermission in _context.RolesPermissions on role.Id equals rolPermission.RoleId
            join permission in _context.Permissions on rolPermission.PermissionId equals permission.Id
            where role.Id == roleId
            select permission;

            return permissions;
        }

        /// <summary>
        /// Obtiene los permisos de un rol específico con las instancias de Action y Resource
        /// </summary>
        /// <param name="roleId">Identificador del rol</param>
        /// <returns>Permisos del rol</returns>
        public IQueryable<AppPermission> GetPermissionsByRoleIncludingActionResource(long roleId) {

            var permissionsArray = (from r in _context.Roles
                                    join rolPermission in _context.RolesPermissions on r.Id equals rolPermission.RoleId
                                    join permission in _context.Permissions on rolPermission.PermissionId equals permission.Id
                                    where r.Id == roleId
                                    select permission.Id).ToArray();
            
            var permissions = _context.Permissions
                .Include(x => x.Action)
                .Include(x => x.Resource)
                .Where(x => permissionsArray.Contains(x.Id));

            return permissions;
        }

    }
}
