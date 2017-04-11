using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ltracker.Models
{
    public class TopicViewModel
    {
        public int? Id { get; set; }
        [DisplayName("Nombre")]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }

    public class EditTopicViewModel
    {
        public int? Id { get; set; }

        [DisplayName("Nombre")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public string NombreAnterior { get; set; }
    }
}