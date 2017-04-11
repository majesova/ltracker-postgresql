using AppFramework.Security;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ltracker.Models;
using AppFramework.Security.Repositories;
using AppFramework.Security.Menus;

namespace ltracker.Controllers
{
    public class AdminController : Controller
    {
        internal static IMapper mapper;

        static AdminController()
        {
            var config = new MapperConfiguration(x => {
                
                x.CreateMap<AppUserViewModel, AppUser>().ReverseMap();

                x.CreateMap<EditAppUserViewModel, AppUser>().ReverseMap();

                x.CreateMap<AppRoleViewModel, AppRole>().ReverseMap();

                x.CreateMap<EditAppRoleViewModel, AppRole>().ReverseMap();

                x.CreateMap<NewAppRoleViewModel, AppRole>().ReverseMap();

                x.CreateMap<AppPermission, AppPermissionViewModel>()
                .ForMember(dest=>dest.ActionName, opt=>opt.MapFrom(src=>src.Action.Name))
                .ForMember(dest => dest.ResourceName, opt => opt.MapFrom(src => src.Resource.Name));
                
                x.CreateMap<AppPermissionViewModel, AppPermission>();

                x.CreateMap<DetailsAppRoleViewModel, AppRole>().ReverseMap();

                x.CreateMap<MenuViewModel, AppMenu>().ReverseMap();

                x.CreateMap<NewMenuViewModel, AppMenu>().ReverseMap();

                x.CreateMap<EditMenuViewModel, AppMenu>().ReverseMap();

                x.CreateMap<MenuItemViewModel, AppMenuItem>().ReverseMap();

                x.CreateMap<NewMenuItemViewModel, AppMenuItem>()
                .ForMember(dest => dest.AppMenuKey, opt => opt.MapFrom(src => src.MenuKey)).ReverseMap();

                x.CreateMap<EditMenuItemViewModel, AppMenuItem>()
                .ForMember(dest => dest.AppMenuKey, opt => opt.MapFrom(src => src.MenuKey)).ReverseMap();

            });

            mapper = config.CreateMapper();
        }
        private AppUserManager _userManager;
        private AppRoleManager _roleManager;
        public AppUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
    
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        #region Users
        public ActionResult Users() {
            AppSecurityContext context = new AppSecurityContext();
            var users = UserManager.Users;
            var model = mapper.Map<IEnumerable<AppUserViewModel>>(users);
            return View(model);
        }

        public ActionResult CreateUser() {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateUser(AppUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users", "Admin");
                }
                AddErrors(result);
            }
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        public ActionResult EditUser(long id) {

            var context = new AppSecurityContext();
            var rolRepository = new RoleRepository(context);
            var userRolRepository = new UserRoleRepository(context);

            var user = context.Users.Find(id);
            var model = new EditAppUserViewModel();
            model.Email = user.Email;
            model.Id = user.Id;

            var roles = rolRepository.GetAll();

            var assignedRoles = userRolRepository.GetAssignedUserRoles(id);

            if (assignedRoles.Count() > 0)
            {
                model.SelectedRoles = assignedRoles.Select(x => x.RoleId).ToArray();
            }
            else {
                model.SelectedRoles = new long[0];
            }
            model.AvailableRoles = mapper.Map<ICollection<AppRoleViewModel>>(roles);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(long id, EditAppUserViewModel model) {

            var context = new AppSecurityContext();
            var rolRepository = new RoleRepository(context);
            var userRolRepository = new UserRoleRepository(context);

            if (ModelState.IsValid)
            {
                //Se asignan los roles
                var user = mapper.Map<AppUser>(model);
                var assignedRoles = userRolRepository.GetAssignedUserRoles(id);
                var selectedRoles = new List<AppRole>();
                if (model.SelectedRoles != null)
                { 
                    foreach (var rolId in model.SelectedRoles)
                    {
                        selectedRoles.Add(new AppRole { Id = rolId });
                    }
                }
                userRolRepository.UpdateUserWithRoles(user, selectedRoles);
                try
                {
                    context.SaveChanges();
                }
                catch (Exception ex) {
                    
                }
                return RedirectToAction("Users");
            }

            var roles = rolRepository.GetAll();
            model.AvailableRoles = mapper.Map<ICollection<AppRoleViewModel>>(roles);
            return View(model);
        }



        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public ActionResult DetailsUser(int id)
        {
            var context = new AppSecurityContext();
            var userRepository = new UserRepository(context);
            var roleRepository = new RoleRepository(context);
            var user = userRepository.Find(id);
            var roles = roleRepository.GetRolesByUserId(user.Id);
            var model = new DetailsAppUserViewModel();
            model.Email = user.Email;
            model.Id = user.Id;
            model.AssignedRoles = new List<AppRoleViewModel>();
            foreach (var item in roles) {
                model.AssignedRoles.Add(new AppRoleViewModel { Id = item.Id, Name = item.Name });
            }
            return View(model);
        }

        #endregion

        #region Roles
        public ActionResult Roles() {
            var context = new AppSecurityContext();
            var rolRepository = new RoleRepository(context);
            var roles = rolRepository.GetAll();
            var models = mapper.Map<IEnumerable<AppRoleViewModel>>(roles);
            return View(models);
        }

        public ActionResult CreateRole()
        {
            var context = new AppSecurityContext();
            var model = new NewAppRoleViewModel();
            var permissionRepository = new PermissionRepository(context);
            var permissions = permissionRepository.GetAll();
            model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateRole(NewAppRoleViewModel model) {

            var context = new AppSecurityContext();
            var rolRepository = new RoleRepository(context);
            var permissionRepository = new PermissionRepository(context);
            var rolePermissionRepository = new RolePermissionRepository(context);

            if (ModelState.IsValid) {

                var role = mapper.Map<AppRole>(model);
                rolRepository.Add(role);

                if (model.SelectedPermissions == null)
                    model.SelectedPermissions = new int[0];

                foreach (var permissionId in model.SelectedPermissions) {
                    rolePermissionRepository.Add(new AppRolePermission { PermissionId = permissionId, RoleId = role.Id });   
                }
                context.SaveChanges();
                return RedirectToAction("Roles", "Admin");
            }

            var permissions = permissionRepository.GetAll();
            model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);
            return View(model);
        }

        public ActionResult EditRole(long id) {
            var context = new AppSecurityContext();
            var rolePermissionRepository = new RolePermissionRepository(context);
            var permissionRepository = new PermissionRepository(context);

            var role = context.Roles.Find(id);
            var permissionsResult = rolePermissionRepository.GetPermissionByRoleId(id);
            var permissions = permissionRepository.GetAll();
            var model = mapper.Map<EditAppRoleViewModel>(role);

            if (permissionsResult.Count() >0 )
                model.SelectedPermissions = permissionsResult.Select(x => x.Id).ToArray();
            
            model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditRole(long id, EditAppRoleViewModel model) {
            var context = new AppSecurityContext();
            var rolePermissionRepository = new RolePermissionRepository(context);
            var permissionRepository = new PermissionRepository(context);
            var roleRepostory = new RoleRepository(context);

            if (ModelState.IsValid) {

                var role = mapper.Map<AppRole>(model);
                roleRepostory.UpdateRoleWithPermissions(role, model.SelectedPermissions);
                context.SaveChanges();
                return RedirectToAction("Roles", "Admin");
            }
            var permissions = roleRepostory.GetAll();
            if (permissions.Count() > 0)
                model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);

            return View(model);

        }


        public ActionResult DetailsRole(int id) {

            var context = new AppSecurityContext();
            var roleRepository = new RoleRepository(context);
            var rolePermissionRepository = new RolePermissionRepository(context);

            var role = roleRepository.Find(id);

            var permissionsResult = rolePermissionRepository.GetPermissionsByRoleIncludingActionResource(id);

            var model = mapper.Map<DetailsAppRoleViewModel>(role);
            model.Permissions = new List<AppPermissionViewModel>();

            foreach (var result in permissionsResult) {
                
                model.Permissions.Add(new AppPermissionViewModel { ActionName = result.Action.Name, ResourceName = result.Resource.Name });
            }
         
            return View(model);
        }

        #endregion

        #region Menús

        public ActionResult Menus()
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            var menus = menuRepository.GetAll();
            var model = mapper.Map<ICollection<MenuViewModel>>(menus);
            return View(model);
        }

        public ActionResult CreateMenu()
        {
            var context = new AppSecurityContext();
            return View();
        }

        [HttpPost]
        public ActionResult CreateMenu(NewMenuViewModel model)
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            if (ModelState.IsValid) {

                var menu = mapper.Map<AppMenu>(model);
                menuRepository.Add(menu);
                context.SaveChanges();
                return RedirectToAction("Menus");
            }
            return View();
        }

        public ActionResult EditMenu(string id)
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            var menu = menuRepository.Find(id);
            var model = mapper.Map<EditMenuViewModel>(menu);
            return View(model);
        }

        [HttpPost]
        public ActionResult EditMenu(string id, EditMenuViewModel model)
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            try
            {
                if (ModelState.IsValid)
                {
                    var menu = mapper.Map<AppMenu>(model);
                    menuRepository.Update(menu);
                    context.SaveChanges();
                    return RedirectToAction("Menus");
                }
            }
            catch (Exception ex) {
                ViewBag.Error = ex.Message;
            }

            return View(model);
        }

        public ActionResult MenuItems(string id)
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            var menuItemRepository = new MenuItemRepository(context);
            var menu = menuRepository.Find(id);
            var items = menuItemRepository.GetItemsByMenuKey(id);
            var model = new MenuItemListViewModel();
            model.MenuItems = mapper.Map<ICollection<MenuItemViewModel>>(items);
            model.MenuKey = menu.Key;
            model.MenuName = menu.Name;
            return View(model);
        }

        public ActionResult CreateMenuItem(string id)
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            var menu = menuRepository.Find(id);
            var model = new  NewMenuItemViewModel();
            model.MenuName = menu.Name;
            model.MenuKey = menu.Key;
            model.AvailablePermissions = PopulatePermissions(model.PermissionId);
            model.AvailableMenuItems = PopulateMenuItems(model.ParentId);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateMenuItem(string id, NewMenuItemViewModel model) {

            var context = new AppSecurityContext();
            var menuItemRepository = new MenuItemRepository(context);
            try
            {
                var menuItem = mapper.Map<AppMenuItem>(model);
                menuItemRepository.Add(menuItem);
                context.SaveChanges();
                return RedirectToAction("menuItems", new { id = model.MenuKey });
            }
            catch (Exception ex) {
                ViewBag.Error = ex.Message;
            }

            model.AvailablePermissions = PopulatePermissions(model.PermissionId);
            model.AvailableMenuItems = PopulateMenuItems(model.ParentId);
            return View(model);
        }


        [HttpGet]
        public ActionResult EditMenuItem(int id) //Id del item
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            var menuItemRepository = new MenuItemRepository(context);

            var menuItem = menuItemRepository.Find(id);
            var model = mapper.Map<EditMenuItemViewModel>(menuItem);
            model.MenuKey = menuItem.AppMenuKey;
            model.MenuName = menuItem.AppMenu.Name;
            model.AvailablePermissions = PopulatePermissions(model.PermissionId);
            model.AvailableMenuItems = PopulateMenuItems(model.ParentId);

            model.AvailablePermissions = PopulatePermissions(model.PermissionId);
            model.AvailableMenuItems = PopulateMenuItems(model.ParentId);

            var itemSelf = model.AvailableMenuItems.Where(x => x.Value == id.ToString());
            if (itemSelf != null && itemSelf.Count() > 0)
            {
                var itemForRemove = itemSelf.SingleOrDefault();
                model.AvailablePermissions.ToList().Remove(itemForRemove);
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult EditMenuItem(int id, EditMenuItemViewModel model)
        {
            var context = new AppSecurityContext();
            var menuRepository = new MenuRepository(context);
            var menuItemRepository = new MenuItemRepository(context);
            try {
                if (ModelState.IsValid)
                {
                    var menuItem = mapper.Map<AppMenuItem>(model);
                    menuItemRepository.Update(menuItem);
                    context.SaveChanges();
                    return RedirectToAction("menuItems", new { id = model.MenuKey });
                }
            } catch (Exception ex) {
                ViewBag.Error = ex.Message;
            }
           
           

            model.AvailablePermissions = PopulatePermissions(model.PermissionId);
            model.AvailableMenuItems = PopulateMenuItems(model.ParentId);

            var itemSelf = model.AvailableMenuItems.Where(x => x.Value == id.ToString());
            if (itemSelf != null && itemSelf.Count()>0) {
                var itemForRemove = itemSelf.SingleOrDefault();
                model.AvailablePermissions.ToList().Remove(itemForRemove);
            }
            return View(model);
        }


        
        public SelectList PopulatePermissions(object selectedItem = null)
        {
            var context = new AppSecurityContext();
            var repository = new PermissionRepository(context);
            var permissions = repository.GetAll().Include(x=>x.Resource)
                .Include(x=>x.Action)
                .OrderBy(x => x.ResourceKey);
            var permissionList = new List<SelectListItem>();
            permissionList.Add(new SelectListItem { Value=null, Text="Sin permiso"});
            foreach (var perm in permissions) {
                var permDesc = $"{perm.Resource.Name} - {perm.Action.Name}";
                permissionList.Add(new SelectListItem { Value = perm.Id.ToString(), Text = permDesc });
            }
            return new SelectList(permissionList, "Value", "Text", selectedItem);
        }

        public SelectList PopulateMenuItems(object selectedItem = null)
        {
            var context = new AppSecurityContext();
            var repository = new MenuItemRepository(context);
            var permissions = repository.GetAll().OrderBy(x => x.Name).ToList();
            permissions.Insert(0, new AppMenuItem { Id = null, Name = "Sin padre" });
            return new SelectList(permissions, "Id", "Name", selectedItem);
        }

        #endregion
    }



}