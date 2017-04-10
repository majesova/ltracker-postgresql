using System.Collections.Generic;
using System.Linq;

namespace AppFramework.Security.Repositories
{
    /// <summary>
    /// Operaciones de UserRole con base de datos
    /// </summary>
    public class UserRoleRepository
    {
        private AppSecurityContext _context;
        public UserRoleRepository(AppSecurityContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Actualiz un usuario con sus roles
        /// </summary>
        /// <param name="user">Usuario a actualizar</param>
        /// <param name="roles">Roles asignados</param>
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
        /// <summary>
        /// Devuelve los roles especificados a un usuario
        /// </summary>
        /// <param name="id">Identificador del usuario</param>
        /// <returns>Lista de AppUserRole</returns>
        public IQueryable<AppUserRole> GetAssignedUserRoles(long id)
        {
            var assignedRoles = from r in _context.UserRoles
                                where r.UserId == id
                                select r;
            return assignedRoles;
        }

    }
}
