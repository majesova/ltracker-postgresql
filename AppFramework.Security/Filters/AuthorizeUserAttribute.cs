using AppFramework.Security.Repositories;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AppFramework.Security.Filters
{
    /// <summary>
    /// Filter para establecer la autorización en los controladores de vista
    /// </summary>
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Clave de la acción que se quiere realizar
        /// </summary>
        public string ActionKey { get; set; }
        /// <summary>
        /// Clave del recurso al que se quiere acceder
        /// </summary>
        public string ResourceKey { get; set; }
        /// <summary>
        /// Contexto asociado a la seguridad
        /// </summary>
        private AuthorizationContext _currentContext;
        /// <summary>
        /// Se ejecuta cuando se atiende una autorización
        /// </summary>
        /// <param name="filterContext">Contexto de autorización</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            _currentContext = filterContext;
            base.OnAuthorization(filterContext);
        }
        /// <summary>
        /// Se ejecuta cuando se va a validar si se tiene o no acceso a lo solicitado
        /// </summary>
        /// <param name="httpContext">Contexo Http</param>
        /// <returns>true/false</returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (!user.IsAuthenticated) {
                return false;
            }
            var userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            AppSecurityContext context = new AppSecurityContext();
            long id = long.Parse(userId);
            var repository = new PermissionRepository(context);
            var valid = repository.HasPermission(id, ActionKey, ResourceKey);
            return valid;
        }

    }
}
