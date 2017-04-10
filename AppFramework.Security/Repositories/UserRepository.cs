using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AppFramework.Security.Repositories
{
    /// <summary>
    /// Operaciones de user con base de datos
    /// </summary>
    public class UserRepository
    {
        AppSecurityContext _context;
        public UserRepository(AppSecurityContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Encuentra un usuario
        /// </summary>
        /// <param name="id">Identificador del usuario</param>
        /// <returns></returns>
        public AppUser Find(long id) {
            return _context.Users.Find(id);
        }

        
    }
}
