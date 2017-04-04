using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AppFramework.Security.Repositories
{
    public class UserRepository
    {
        AppSecurityContext _context;
        public UserRepository(AppSecurityContext context)
        {
            _context = context;
        }
        
        public AppUser Find(long id) {
            return _context.Users.Find(id);
        }

        
    }
}
