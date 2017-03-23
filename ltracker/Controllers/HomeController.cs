using AppFramework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using AppFramework.Security.Filters;

namespace ltracker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //Ejemplo de como obtener el usuario en sesión
            var permission =  Request.GetOwinContext().HasPermission("WRITE", "Assignment");
            if (permission) {
                ViewBag.Resultado = "SI TIENE PERMISO WRITE - Assingment";
            }

            var permission2 = Request.GetOwinContext().HasPermission("ACCESS", "Assignment");
            if (!permission2)
                ViewBag.Resultado2 = "NO TIENE PERMISO ACCESS - Assingment";
            
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