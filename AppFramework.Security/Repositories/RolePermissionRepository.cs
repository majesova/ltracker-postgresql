using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AppFramework.Security.Repositories
{
    public class RolePermissionRepository
    {
        private AppSecurityContext _context;

        public RolePermissionRepository(AppSecurityContext context)
        {
            _context = context;
        }

        public void Add(AppRolePermission rolePermission)
        {
            _context.RolesPermissions.Add(rolePermission);
        }

        public IQueryable<AppPermission> GetPermissionByRoleId(long roleId) {

            var permissions = from r in _context.Roles
            join rolPerm in _context.RolesPermissions on r.Id equals rolPerm.RoleId
            join perms in _context.Permissions on rolPerm.PermissionId equals perms.Id
            where r.Id == roleId
            select perms;

            return permissions;
        }


        public IQueryable<AppPermission> GetPermissionsByRoleIncludingActionResource(long roleId) {

            var permissionsResult = from r in _context.Roles
                                    join rolPerm in _context.RolesPermissions on r.Id equals rolPerm.RoleId
                                    join perms in _context.Permissions on rolPerm.PermissionId equals perms.Id
                                    where r.Id == roleId
                                    select perms.Id;

            var ids = permissionsResult.ToArray();

            var permissions = _context.Permissions.Include(x => x.Action).Include(x => x.Resource)
                .Where(x => ids.Contains(x.Id));

            return permissions;
        }

    }
}
