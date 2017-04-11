using Microsoft.Owin;
using System.Linq;
using System.Security.Claims;
using AppFramework.Security.Repositories;
using System.Web;

namespace AppFramework.Security.Extensions
{
    /// <summary>
    /// Extensiones de seguridad
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Verifica si un usuario tiene o no permiso para realizar una acción
        /// </summary>
        /// <param name="context">Contexto de seguridad</param>
        /// <param name="actionKey">Clave de la acción que se desea realizar</param>
        /// <param name="resourceKey">Clave del recurso que se desea acceder</param>
        /// <returns>true/false</returns>
        public static bool HasPermission(this IOwinContext context, string actionKey, string resourceKey)
        {
            var user = (ClaimsIdentity)HttpContext.Current.User.Identity;
            if (!user.IsAuthenticated)
            {
                return false;
            }
            var userId = user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            AppSecurityContext contextSecurity = new AppSecurityContext();
            long id = long.Parse(userId);
            var repository = new PermissionRepository(contextSecurity);
            var valid = repository.HasPermission(id, actionKey, resourceKey);
            return valid;
        }
    }
}
