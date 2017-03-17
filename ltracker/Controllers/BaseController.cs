using ltracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ltracker.Controllers
{
    public class BaseController : Controller
    {

        protected LearningContext context = new LearningContext();

        protected override void Dispose(bool disposing)
        {
            context.Dispose();

            base.Dispose(disposing);
        }
    }
}