using ltracker.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Expressions;
using ltracker.Data.Entities;
using ltracker.Models;
using ltracker.Helpers;
using AppFramework.Security.Filters;

namespace ltracker.Controllers
{
    [AuthorizeUser(Action="write", Resource ="Assignment")]
    public class AssignmentController : BaseController
    {
        // GET: Assignment
        public ActionResult Index() //Busqueda
        {
            var repository = new AssignedCourseRepository(context);
            var includes = new Expression<Func<AssignedCourse, object>>[] { x=>x.Course, x=>x.Invidivual };
            var courses = repository.QueryIncluding(null, includes, "AssignmentDate");
            var models = MapperHelper.Map <ICollection<AssignmentViewModel>>(courses);
            return View(models);
        }

        //Assignment/Create
        public ActionResult Create()
        {
            var model = new NewAssignmentViewModel();
            model.CoursesList = PopulateCourses(model.CourseId);
            model.IndividualList = PopulateInviduals(model.IndividualId);
            model.AssignmentDate = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(NewAssignmentViewModel model) {

            var repository = new AssignedCourseRepository(context);
            try
            {

           
            if (ModelState.IsValid) {
                 var assignedCourse = MapperHelper.Map<AssignedCourse>(model);
                assignedCourse.isCompleted = false;
                repository.Insert(assignedCourse);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            model.CoursesList = PopulateCourses(model.CourseId);
            model.IndividualList = PopulateInviduals(model.IndividualId);
            return View(model);
            }
            catch (Exception ex)
            {
                model.CoursesList = PopulateCourses(model.CourseId);
                model.IndividualList = PopulateInviduals(model.IndividualId);
                return View(model);
            }
        }

        public ActionResult Edit(int id)
        {
            var repository = new AssignedCourseRepository(context);
            //Expression<>[]
            //Expression<Func<Type,object>>[]{ x=>x.Propiedad }
            var includes = new Expression<Func<AssignedCourse, object>>[] { x => x.Course, x => x.Invidivual };
            var criteria = new AssignedCourse { Id = id };
            var courses = repository.QueryByExampleIncludig(criteria, includes).SingleOrDefault();
            var model = MapperHelper.Map<EditAssignmentViewModel>(courses);
            return View(model);
        
        }

        [HttpPost]
        public ActionResult Edit(int id, EditAssignmentViewModel model)
        {
            var repository = new AssignedCourseRepository(context);
            try
            {
                if (ModelState.IsValid)
                {
                    var assignmentUpd = MapperHelper.Map<AssignedCourse>(model);
                    repository.Update(assignmentUpd);
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else {
                    return View(model);
                }

            }
            catch (Exception ex) {
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }

            return View();
        }

        public SelectList PopulateInviduals(object selectedItem = null) {
            var repository = new IndividualRepository(context);
            var individuals = repository.Query(null, "Name").ToList();
            individuals.Insert(0, new Individual { Id = null, Name = "Seleccione" });
            return new SelectList(individuals, "Id", "Name", selectedItem);
        }

        public SelectList PopulateCourses(object selectedItem = null) {
            var repository = new CourseRepository(context);
            var courses = repository.Query(null, "Name").ToList();
            courses.Insert(0, new Course { Id = null, Name = "Seleccione" });
            return new SelectList(courses, "Id", "Name", selectedItem);
        }

    }
}