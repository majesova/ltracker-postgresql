using AppFramework.Security.Filters;
using ltracker.Data.Entities;
using ltracker.Data.Repositories;
using ltracker.Helpers;
using ltracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ltracker.Controllers
{
    [AuthorizeUser(ActionKey = "R", ResourceKey = "TOPIC")]
    public class TopicController : BaseController
    {
        // GET: Topic
        public ActionResult Index()
        {
            var repository = new TopicRepository(context);
            var topic = repository.Query(null, "Name");
            var models = MapperHelper.Map<IEnumerable<TopicViewModel>>(topic);
            return View(models);
        }

        // GET: Topic/Details/5
        public ActionResult Details(int id)
        {
            var repository = new TopicRepository(context);
            var topic = repository.Find(id);
            var model = MapperHelper.Map<TopicViewModel>(topic);
            return View(model);
        }

        // GET: Topic/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Topic/Create
        [HttpPost]
        public ActionResult Create(TopicViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var repository = new TopicRepository(context);
                    var topicQry = new Topic { Name = model.Name };
                    var nombreExiste = repository.QueryByExample(topicQry).Count > 0;
                    if (!nombreExiste)
                    {
                        var topic = MapperHelper.Map<Topic>(model);
                        repository.Insert(topic);
                        context.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("Name", "El nombre ya está ocupado por otro tópico.");
                        return View(model);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: Topic/Edit/5
        public ActionResult Edit(int id)
        {
            var repository = new TopicRepository(context);
            var topic = repository.Find(id);
            var model = MapperHelper.Map<EditTopicViewModel>(topic);
            model.NombreAnterior = model.Name;
            return View(model);
        }

        // POST: Topic/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, EditTopicViewModel model)
        {
            try
            {
                var repository = new TopicRepository(context);
                if (model.Name != model.NombreAnterior)
                {
                    var existeNombre = repository.Query(x => x.Name == model.Name && x.Id != model.Id).Count > 0;
                    if (existeNombre)
                    {
                        ModelState.AddModelError("Name", "El nombre ya está ocupado por otro tópico.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.Remove("Nombre");
                }

                if (ModelState.IsValid)
                {
                    var topic = MapperHelper.Map<Topic>(model);
                    repository.Update(topic);
                    context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Topic/Delete/5
        public ActionResult Delete(int id)
        {
            var repository = new TopicRepository(context);
            var topic = repository.Find(id);
            var model = MapperHelper.Map<TopicViewModel>(topic);
            return View(model);
        }

        // POST: Topic/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, TopicViewModel model)
        {
            try
            {
                var repository = new TopicRepository(context);

                var topic = MapperHelper.Map<Topic>(model);
                repository.Delete(topic);
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
