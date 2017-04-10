using AppFramework.Security.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppFramework.Security.Repositories
{
    public class MenuRepository
    {
        private AppSecurityContext _context;

        public MenuRepository(AppSecurityContext context)
        {
            _context = context;
        }

        public IQueryable<AppMenu> GetAll() {
            return _context.Menus;
        }

        public void Add(AppMenu menu) {
            _context.Menus.Add(menu);
        }

        public void Update(AppMenu menu) {
            if (!_context.Exists(menu)) {
                _context.Menus.Attach(menu);
            }
            _context.Entry(menu).State = System.Data.Entity.EntityState.Modified;
        }

        public AppMenu Find(string key) {
            return _context.Menus.Find(key);
        }
        
    }

    
}
