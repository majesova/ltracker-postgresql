using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ltracker.Data;
using ltracker.Data.Repositories;
using ltracker.Data.Entities;
using ltracker.Models;
using ltracker.Helpers;
using AppFramework.Security.Filters;

namespace ltracker.Controllers
{
    [AuthorizeUser(ActionKey = "R", ResourceKey = "INDIV")]
    public class IndividualController : BaseController
    {
        

        // GET: Individual
        /// <summary>
        /// Lista de individuals y se la va a pasar a la vista Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Individual List";
            var repository = new IndividualRepository(context);
            var individuals = repository.GetAll();
            var models = MapperHelper.Map <IEnumerable<IndividualViewModel>>(individuals);
            return View(models);
        }

        
        // GET: Individual/Details/5
        public ActionResult Details(int id)
        {
            var repository = new IndividualRepository(context);
            var individual = repository.Find(id);
            var model = MapperHelper.Map<IndividualViewModel>(individual);
            return View(model);
        }

        // GET: Individual/Create
        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Individual/Create
        [HttpPost]
        public ActionResult Create(NewIndividualViewModel model)
        {
            try
            {
                if (ModelState.IsValid) {

                    var repository = new IndividualRepository(context);
                    var individualQry = new Individual { Email = model.Email };
                    var emailExiste = repository.QueryByExample(individualQry).Count > 0;
                    if (!emailExiste)
                    {
                        var individual = MapperHelper.Map<Individual>(model);
                        repository.Insert(individual);
                        context.SaveChanges();
                    }
                    else {
                        ModelState.AddModelError("Email", "El email está ocupado");
                        return View(model);
                    }
                }

                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: Individual/Edit/5
        public ActionResult Edit(int id)
        {
            var repositorio = new IndividualRepository(context);
            var individual = repositorio.Find(id);
            var model = MapperHelper.Map<EditIndividualViewModel>(individual);
            model.EmailAnterior = model.Email;
            return View(model);
        }

        // POST: Individual/Edit/5
        [HttpPost]
       
        public ActionResult Edit(int id, EditIndividualViewModel model)
        {
            try
            {
                var repository = new IndividualRepository(context);
                if (model.Email != model.EmailAnterior)
                {
                    var existeEmail = repository.Query(x => x.Email == model.Email && x.Id != model.Id).Count > 0;
                    if (existeEmail)
                    {
                        ModelState.AddModelError("Email", "Email ocupado");
                        return View(model);
                    }
                }
                else {
                    ModelState.Remove("Email");
                }

                if (ModelState.IsValid) {
                    var individual = MapperHelper.Map<Individual>(model);
                    repository.Update(individual);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Individual/Delete/5
        public ActionResult Delete(int id)
        {
            var repositorio = new IndividualRepository(context);
            var individual = repositorio.Find(id);
            var model = MapperHelper.Map<IndividualViewModel>(individual);
            return View(model);
        }

        // POST: Individual/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, IndividualViewModel model)
        {
            try
            {

                var repository = new IndividualRepository(context);
             
                    var individual = MapperHelper.Map<Individual>(model);
                    repository.Delete(individual);
                    context.SaveChanges();
            
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AllowAnonymous]
        public JsonResult CheckEmail(string email) {
            var repository = new IndividualRepository(context);
            var emailExiste = repository.Query(x => x.Email == email).Count == 0;
            return Json(emailExiste, JsonRequestBehavior.AllowGet);
        }


    }
}
