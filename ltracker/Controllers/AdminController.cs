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

namespace ltracker.Controllers
{
    public class AdminController : Controller
    {
        internal static IMapper mapper;

        static AdminController()
        {
            var config = new MapperConfiguration(x => {
                
                x.CreateMap<AppUserViewModel, AppUser>().ReverseMap();

                x.CreateMap<AppRoleViewModel, AppRole>().ReverseMap();

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

        public ActionResult DetailsRole(int id) {
            var context = new AppSecurityContext();
            var role = context.Roles.Find(id);
            var permissionsResult =  from r in context.Roles
                               join rolPerm in context.RolesPermissions on r.Id equals rolPerm.RoleId
                               join perms in context.Permissions on rolPerm.PermissionId equals perms.Id
                               where r.Id == role.Id
                               select new {ActionName= perms.Action.Name, ResourceName = perms.Resource.Name };

            var model = mapper.Map<DetailsAppRoleViewModel>(role);
            model.Permissions = new List<AppPermissionViewModel>();
            foreach (var result in permissionsResult) {
                model.Permissions.Add(new AppPermissionViewModel { ActionName = result.ActionName, ResourceName = result.ResourceName });
            }
         
            return View(model);
        }
        
        #endregion

    }

    #region ViewModels
    public class AppUserViewModel {
        public long Id { get; set; }
        public string Email { get; set; }
        
        public string Password { get; set; }
    }

    public class EditAppUserViewModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public ICollection<AppRoleViewModel> AvailableRoles { get; set; }

    }

    public class AppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class NewAppRoleViewModel {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppPermissionViewModel> AvailablePermissions { get; set; }
        public int[] SelectedPermissions { get; set; }
    }

    public class EditAppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppPermissionViewModel> AvailablePermissions { get; set; }
        public int[] SelectedPermissins { get; set; }
    }

    public class DetailsAppRoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<AppPermissionViewModel> Permissions { get; set; }
    }

    public class AppPermissionViewModel {
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string ResourceName { get; set; }
    }
    #endregion

}