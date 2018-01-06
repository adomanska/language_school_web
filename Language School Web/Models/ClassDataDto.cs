using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageSchool.Models
{
    public class ClassDataDto
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public string Language { get; set; }
        public string LanguageLevel { get; set; }
        public int StudentsCount { get; set; }
        public int StudentsMax { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Price { get; set; }
    }
}
