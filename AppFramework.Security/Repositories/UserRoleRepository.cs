using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Security.Repositories
{
    public class UserRoleRepository
    {
        private AppSecurityContext _context;
        public UserRoleRepository(AppSecurityContext context)
        {
            _context = context;
        }

        public void UpdateUserWithRoles(AppUser user, List<AppRole> roles) {

            var assignedRoles = GetAssignedUserRoles(user.Id);
            
            foreach (var a in assignedRoles)
            {
                _context.UserRoles.Remove(a);
            }

            if (roles != null) {
                foreach (var rol in roles) {
                    AppUserRole appUserRole = new AppUserRole { RoleId = rol.Id, UserId = user.Id };
                    _context.UserRoles.Add(appUserRole);
                }
            }

        }

        public IQueryable<AppUserRole> GetAssignedUserRoles(long id)
        {
            var assignedRoles = from r in _context.UserRoles
                                where r.UserId == id
                                select r;
            return assignedRoles;
        }

    }
}
