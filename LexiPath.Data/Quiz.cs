using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class Quiz
    {
        public int QuizID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public bool IsPractice { get; set; }
        public int LanguageID { get; set; }
        public string LanguageName { get; set; } // For display
        public bool isActivated { get; set; }      // For activation status
        public List<Course> RelatedCourses { get; set; } = new List<Course>();
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public string RelatedCourseIDs { get; set; } // e.g., "1,5,10"
        public string RelatedTagIDs { get; set; }    // e.g., "2,3"
    }
}
