using ltracker.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ltracker.Data.Repositories
{

    public class CourseRepository : BaseRepository<Course>
    {
        public CourseRepository(LearningContext context) : base(context)
        {

        }

        public void InsertCourseWithTopics(Course course, int[] selecedTopics) {
            if (selecedTopics != null) {
                var topics = from t in _context.Topics
                             where selecedTopics.Contains((int)t.Id)
                             select t;
                course.Topics = new List<Topic>();
                foreach (var t in topics)
                    course.Topics.Add(t);
            }
            _context.Courses.Add(course);
        }

        public void UpdateCourseWithTopics(Course course, int[] selectedTopics) {
            _context.Courses.Attach(course);
            _context.Entry(course).State = System.Data.Entity.EntityState.Modified;
            _context.Entry(course).Collection(x => x.Topics).Load();
            course.Topics.Clear();
            if(selectedTopics != null) {
                var topics = from t in _context.Topics
                             where selectedTopics.Contains((int)t.Id)
                             select t;
                course.Topics = new List<Topic>();
                foreach (var t in topics)
                    course.Topics.Add(t);
            }

        }



    }


}
