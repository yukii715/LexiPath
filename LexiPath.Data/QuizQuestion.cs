using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class QuizQuestion
    {
        public int QuestionID { get; set; }
        public int QuizID { get; set; }
        public int QuestionTypeID { get; set; }
        public string QuestionTypeName { get; set; } // e.g., "MCQ", "TypeInAnswer"
        public string QuestionText { get; set; }
        public string CorrectAnswer { get; set; } // For TypeInAnswer, FillInTheBlank
        public string ImagePath { get; set; }
        public int SequenceOrder { get; set; }
        public List<QuizAnswerOption> Options { get; set; } = new List<QuizAnswerOption>(); // For MCQ, MatchOptions
    }
}
