using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ltracker.Models
{
    public class CourseViewModel
    {
        public int? Id { get; set; }

        [Required]
        [MaxLength(200)]
        [DisplayName("Nombre")]
        public string Name { get; set; }
        
        [DisplayName("Horas promedio")]
        public decimal? DurationAVG { get; set; }

        [MaxLength(500)]
        [DisplayName("Descripción")]
        public string Description { get; set; }   

        /// <summary>
        /// Sirve para desplegar los topics disponibles
        /// </summary>
        public ICollection<TopicViewModel> AvailableTopics { get; set; }
        /// <summary>
        /// Sirve para guardar la selección del usuario
        /// </summary>
        public int[] SelectedTopics { get; set; }
        
    }

    public class DetailsCourseViewModel
    {
        public int? Id { get; set; }

      
        public string Name { get; set; }

        public decimal? DurationAVG { get; set; }

     
        public string Description { get; set; }

        public ICollection<TopicViewModel> Topics { get; set; }
      
    }

   
}