using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    // This simple class holds the user's answer for one question,
    // so we can show it in the final review.
    public class QuizUserAnswer
    {
        public string QuestionText { get; set; }
        public string UserSelection { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}