using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string CourseType { get; set; }
        public int CategoryID { get; set; }
        public int LanguageID { get; set; }
        public bool isActivated { get; set; }
    }
}