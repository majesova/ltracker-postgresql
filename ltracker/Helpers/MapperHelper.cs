using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ltracker.Models;
using ltracker.Data.Entities;
using AutoMapper;

namespace ltracker.Helpers
{
    public class MapperHelper
    {
        internal static IMapper mapper;

        static MapperHelper()
        {
            var config = new MapperConfiguration(x=> {
                x.CreateMap<Individual, IndividualViewModel>().ReverseMap();
                x.CreateMap<Individual, NewIndividualViewModel>().ReverseMap();
                x.CreateMap<Individual, EditIndividualViewModel>().ReverseMap();
                x.CreateMap<Course, CourseViewModel>().ReverseMap();
                x.CreateMap<Topic, TopicViewModel>().ReverseMap();
                x.CreateMap<Course, DetailsCourseViewModel>();
                x.CreateMap<NewAssignmentViewModel, AssignedCourse>();
                x.CreateMap<AssignedCourse, AssignmentViewModel>();
                x.CreateMap<EditAssignmentViewModel, AssignedCourse>().ReverseMap();
            });
            mapper = config.CreateMapper();
        }

        public static T Map<T>(object source) {
            return mapper.Map<T>(source);
        }
    }
}