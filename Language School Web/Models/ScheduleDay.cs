using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanguageSchool.Models;

namespace LanguageSchool.Models
{
    public class ScheduleDay
    {
        public string Day { get; set; }
        public List<ClassDataDto> Items { get; set; }
    }
}