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

        public void AddRole(AppRole role) {
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

        }

        public IQueryable<AppRole> GetAll() {
            return _context.Roles;
        }


    }
}
