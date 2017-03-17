using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ltracker.Data.Entities
{
    /// <summary>
    /// Course disponible en la plataforma
    /// </summary>
    public class Course
    {
        
        public int? Id { get; set; }
        public string Name { get; set; }
        public decimal? DurationAVG { get; set; }
        public string Description { get; set; }
        public ICollection<Topic> Topics { get; set; }
    }
}
