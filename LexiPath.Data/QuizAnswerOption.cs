using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class QuizAnswerOption
    {
        public int OptionID { get; set; }
        public int QuestionID { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; } // For MCQ (determines if this option is THE correct one)
        public string OptionValue { get; set; } // For MatchOptions (e.g., the matching term/definition)
    }
}
