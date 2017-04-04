using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Security.Repositories
{
    public class RoleRepository
    {
        private AppSecurityContext _context;
        public RoleRepository(AppSecurityContext context)
        {
            _context = context;
        }


        public void Add(AppRole role) {
            _context.Roles.Add(role);
        }

        public void UpdateRole(AppRole role) {
            _context.Entry(role).State = System.Data.Entity.EntityState.Modified;
        }




        public void AddRoleWithPermissions(AppRole role, int[] permissionsId) {

            _context.Roles.Add(role);

            if (permissionsId != null)
            {
                var permissions = from r in _context.Permissions
                                  where permissionsId.Contains(r.Id)
                                  select r;
                foreach (var perm in permissions)
                {
                    _context.RolesPermissions.Add(new AppRolePermission { Role = role, PermissionId = perm.Id });
                }
            }

        }

        public void UpdateRoleWithPermissions(AppRole role, int[] permissionsId) {

            _context.Entry(role).State = System.Data.Entity.EntityState.Modified;

            var permissionsResult = from r in _context.Roles
                                    join rolPerm in _context.RolesPermissions on r.Id equals rolPerm.RoleId
                                    join perms in _context.Permissions on rolPerm.PermissionId equals perms.Id
                                    where r.Id == role.Id
                                    select rolPerm;

            if (permissionsResult.Count() > 0)
            {
                foreach (var rolePerm in permissionsResult)
                {
                    _context.RolesPermissions.Remove(rolePerm);
                }
            }

            if (permissionsId != null) {
                foreach (var permissionId in permissionsId)
                {
                    _context.RolesPermissions.Add(new AppRolePermission { PermissionId = permissionId, RoleId = role.Id });
                }
            }


        }

        public IQueryable<AppRole> GetAll() {
            return _context.Roles;
        }


        public AppRole Find(long id) {
            return _context.Roles.Find(id);
        }

        public IQueryable<AppRole> GetRolesByUserId(long userId) {
            var result = from r in _context.Roles
                         join ur in _context.UserRoles on r.Id equals ur.RoleId
                         where ur.UserId == userId
                         select r;

            return result;
        }

    }
}
