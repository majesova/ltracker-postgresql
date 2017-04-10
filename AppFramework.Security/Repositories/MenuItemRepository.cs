using AppFramework.Security.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace AppFramework.Security.Repositories
{
    public class MenuItemRepository
    {
        private AppSecurityContext _context;
        public MenuItemRepository(AppSecurityContext context)
        {
            _context = context;
        }

        public IQueryable<AppMenuItem> GetAll()
        {
            return _context.MenuItems;
        }

        public void Add(AppMenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
        }

        public void Update(AppMenuItem menuItem)
        {
            if (!_context.Exists(menuItem))
            {
                _context.MenuItems.Attach(menuItem);
            }
            _context.Entry(menuItem).State = EntityState.Modified;
        }

        public void Delete(AppMenuItem menuItem)
        {
            _context.Entry(menuItem).State = System.Data.Entity.EntityState.Deleted;
        }

        public AppMenuItem Find(int id) {
            var result = _context.MenuItems
                .Include(x=>x.AppMenu)
                .Include(x => x.Parent).Where(x=>x.Id == id).SingleOrDefault();
            return result;
        }


        public ICollection<AppMenuItem> GetMenuByUsuarioId(long id) {

            var menuItems = (from r in _context.UserRoles
                             join rolPerm in _context.RolesPermissions on r.RoleId equals rolPerm.RoleId
                             join menuItem in _context.MenuItems on rolPerm.PermissionId equals menuItem.PermissionId
                             where r.UserId == id
                             orderby menuItem.Order
                             select menuItem.Id).Distinct();

            var items = _context.MenuItems.Include(x => x.Children).Where(x =>menuItems.Contains(x.Id.Value) && x.ParentId == null).ToList();

            return items;
        }

    }
}
