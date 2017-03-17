using ltracker.Data;
using ltracker.Data.Entities;
using ltracker.Data.Repositories;
using ltracker.Helpers;
using ltracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;

namespace ltracker.Controllers
{
    
     public class CourseController : BaseController
    {
        
        // GET: Course
        public ActionResult Index()
        {
            var repository = new CourseRepository(context);
            var courses = repository.Query(null, "Name");
            var models = MapperHelper.Map<IEnumerable<CourseViewModel>>(courses);
            return View(models);
        }

        // GET: Course/Details/5
        public ActionResult Details(int id)
        {
            var repository = new CourseRepository(context);
            var includes = new Expression<Func<Course, object>>[] { x =>x.Topics};
            var courses = repository.QueryIncluding(x => x.Id == id, includes).SingleOrDefault();
            var model = MapperHelper.Map<DetailsCourseViewModel>(courses);
            return View(model);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            var model = new CourseViewModel();
            var topicRepository = new TopicRepository(context);
            var topics = topicRepository.Query(null, "Name DESC");
            model.AvailableTopics = MapperHelper.Map<ICollection<TopicViewModel>>(topics);
            return View(model);
        }

        // POST: Course/Create
        [HttpPost]
        public ActionResult Create(CourseViewModel model)
        {
            try
            {
                var repository = new CourseRepository(context);
                if (ModelState.IsValid)
                {

                    var course = MapperHelper.Map<Course>(model);
                    //Pasar modelo ya lleno
                    //course.Topics = new List<Topic>();
                    /* var topicRepo = new TopicRepository(context);
                     foreach (var topicId in model.SelectedTopics) {
                         course.Topics.Add(topicRepo.Find(topicId));

                     }
                     repository.Insert(course);
                      */
                    repository.InsertCourseWithTopics(course, model.SelectedTopics);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else {
                    var topicRepository = new TopicRepository(context);
                    var topics = topicRepository.Query(null, "Name DESC");
                    model.AvailableTopics = MapperHelper.Map<ICollection<TopicViewModel>>(topics);
                    return View(model);
                }
                
            }
            catch
            {
                return View();
            }
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int id)
        {
            var repository = new CourseRepository(context);
            var topicRepository = new TopicRepository(context);
            var includes = new Expression<Func<Course, object>>[] { x => x.Topics };
            var course = repository.QueryIncluding(x => x.Id == id, includes).SingleOrDefault();
            var model = MapperHelper.Map<CourseViewModel>(course);
            var topics = topicRepository.Query(null, "Name");
            model.AvailableTopics = MapperHelper.Map<ICollection<TopicViewModel>>(topics);
            model.SelectedTopics = course.Topics.Select(x => x.Id.Value).ToArray();
            return View(model);
        }

        // POST: Course/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, CourseViewModel model)
        {
            var topicRepository = new TopicRepository(context);
            try
            {
                var repository = new CourseRepository(context);
             
                if (ModelState.IsValid) {

                    var course = MapperHelper.Map<Course>(model);
                    repository.UpdateCourseWithTopics(course, model.SelectedTopics);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }

                var topics = topicRepository.Query(null, "Name");
                model.AvailableTopics = MapperHelper.Map<ICollection<TopicViewModel>>(topics);
                return View(model);
            }
            catch(Exception ex)
            {
                var topics = topicRepository.Query(null, "Name");
                model.AvailableTopics = MapperHelper.Map<ICollection<TopicViewModel>>(topics);
                return View(model);

            }
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int id)
        {
            var repository = new CourseRepository(context);
            var course = repository.Find(id);
            var model = MapperHelper.Map<CourseViewModel>(course);
            return View(model);
        }

        // POST: Course/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CourseViewModel model)
        {
            try
            {
                if (id != model.Id) {
                    return new HttpNotFoundResult();
                }
                // TODO: Add delete logic here
                var repository = new CourseRepository(context);
                var course = repository.Find(model.Id);
                repository.Delete(course);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
        }
    }
}
