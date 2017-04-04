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
            var user = context.Users.Find(id);
            var model = new EditAppUserViewModel();
            model.Email = user.Email;
            model.Id = user.Id;
            var roles = context.Roles;
            var assignedRoles = from r in context.UserRoles
                                where r.UserId == id
                                select r;
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

            if (ModelState.IsValid) {

                //Se asignan los roles
                var user = mapper.Map<AppUser>(model);

                var assignedRoles = from r in context.UserRoles
                                    where r.UserId == user.Id
                                    select r;

                foreach (var a in assignedRoles) {
                    context.UserRoles.Remove(a);
                }

                foreach (var rolId in model.SelectedRoles) {
                    AppUserRole appUserRole = new AppUserRole { RoleId = rolId, UserId = user.Id };
                    context.UserRoles.Add(appUserRole);
                }

                context.SaveChanges();
                return RedirectToAction("Users");
            }

            var roles = context.Roles;
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
        #endregion

        #region Roles
        public ActionResult Roles() {
            var context = new AppSecurityContext();
            var roles = context.Roles;
            var models = mapper.Map<IEnumerable<AppRoleViewModel>>(roles);

            return View(models);
        }

        public ActionResult CreateRole()
        {
            var context = new AppSecurityContext();
            var model = new NewAppRoleViewModel();
            var permissions = context.Permissions.Include(x => x.Action).Include(x => x.Resource).ToList();
            model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateRole(NewAppRoleViewModel model) {

            var context = new AppSecurityContext();
            if (ModelState.IsValid) {
                var role = mapper.Map<AppRole>(model);
                context.Roles.Add(role);
                if (model.SelectedPermissions == null) model.SelectedPermissions = new int[0];
                foreach (var permissionId in model.SelectedPermissions) {
                    context.RolesPermissions.Add(new AppRolePermission { PermissionId = permissionId, RoleId = role.Id });
                }
                context.SaveChanges();
                return RedirectToAction("Roles", "Admin");
            }
            var permissions = context.Permissions.Include(x => x.Action).Include(x => x.Resource).ToList();
            model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);
            return View(model);
        }

        public ActionResult EditRole(long id) {
            var context = new AppSecurityContext();
            var role = context.Roles.Find(id);
            var permissionsResult = from r in context.Roles
                                    join rolPerm in context.RolesPermissions on r.Id equals rolPerm.RoleId
                                    join perms in context.Permissions on rolPerm.PermissionId equals perms.Id
                                    where r.Id == role.Id
                                    select perms;
            var permissions = context.Permissions.Include(x => x.Action).Include(x => x.Resource).ToList();
            var model = mapper.Map<EditAppRoleViewModel>(role);

            if (permissionsResult.Count() >0 )
                model.SelectedPermissions = permissionsResult.Select(x => x.Id).ToArray();
            
            model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);

            return View(model);
        }

        [HttpPost]
        public ActionResult EditRole(long id, EditAppRoleViewModel model) {
            var context = new AppSecurityContext();
            if (ModelState.IsValid) {

                var role = mapper.Map<AppRole>(model);

                var permissionsResult = from r in context.Roles
                                        join rolPerm in context.RolesPermissions on r.Id equals rolPerm.RoleId
                                        join perms in context.Permissions on rolPerm.PermissionId equals perms.Id
                                        where r.Id == id
                                        select rolPerm;

                if (permissionsResult.Count() > 0)
                {
                    foreach (var rolePerm in permissionsResult) {
                        context.RolesPermissions.Remove(rolePerm);
                    }
                }

                if (model.SelectedPermissions == null) model.SelectedPermissions = new int[0];
                foreach (var permissionId in model.SelectedPermissions)
                {
                    context.RolesPermissions.Add(new AppRolePermission { PermissionId = permissionId, RoleId = role.Id });
                }
                context.SaveChanges();
                return RedirectToAction("Roles", "Admin");
            }
            var permissions = context.Permissions.Include(x => x.Action).Include(x => x.Resource).ToList();
            if (permissions.Count() > 0)
                model.AvailablePermissions = mapper.Map<ICollection<AppPermissionViewModel>>(permissions);

            return View(model);

        }


        public ActionResult DetailsRole(int id) {
            var context = new AppSecurityContext();
            var role = context.Roles.Find(id);
            var permissionsResult =  from r in context.Roles
                               join rolPerm in context.RolesPermissions on r.Id equals rolPerm.RoleId
                               join perms in context.Permissions on rolPerm.PermissionId equals perms.Id
                               where r.Id == role.Id
                               select new { ActionName= perms.Action.Name, ResourceName = perms.Resource.Name };

            var model = mapper.Map<DetailsAppRoleViewModel>(role);
            model.Permissions = new List<AppPermissionViewModel>();
            foreach (var result in permissionsResult) {
                model.Permissions.Add(new AppPermissionViewModel { ActionName = result.ActionName, ResourceName = result.ResourceName });
            }
         
            return View(model);
        }
        
        #endregion

    }

   

}