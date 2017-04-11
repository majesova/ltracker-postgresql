using AppFramework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using AppFramework.Security.Extensions;

namespace ltracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Ejemplo de como obtener el usuario en sesión
            var permission =  Request.GetOwinContext().HasPermission("W", "ROLES");
            if (permission) {
                ViewBag.Resultado = "SI TIENE PERMISO WRITE - ROLES";
            }

            var permission2 = Request.GetOwinContext().HasPermission("R", "ROLES");
            if (!permission2)
                ViewBag.Resultado2 = "NO TIENE PERMISO Read - ROLES";
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}