using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class QuizAttempt : System.Web.UI.Page
    {
        private Quiz currentQuiz;
        private User currentUser = null;
        private bool isRegistered = false;
        private List<QuizUserAnswer> userAnswers;

        public bool IsUserRegistered { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] != null)
            {
                currentUser = (User)Session["User"];
                isRegistered = true;
                IsUserRegistered = true;
                if (currentUser.IsAdmin) IsAdmin = true;
            }

            if (Session["ActiveQuizObject"] == null)
            {
                ExitQuiz();
                return;
            }

            currentQuiz = (Quiz)Session["ActiveQuizObject"];

            if (!IsPostBack)
            {
                userAnswers = new List<QuizUserAnswer>();
                Session["UserAnswers"] = userAnswers;
                DisplayQuestion(0);
            }
            else
            {
                userAnswers = (List<QuizUserAnswer>)Session["UserAnswers"];
            }
        }

        protected void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            try
            {
                int index = GetCurrentIndex();
                QuizQuestion q = currentQuiz.Questions[index];
                QuizUserAnswer ans = new QuizUserAnswer();
                ans.QuestionText = q.QuestionText;
                bool isCorrect = false;

                if (q.QuestionTypeName == "MCQ")
                {
                    ans.UserSelection = rblOptions.SelectedItem != null ? rblOptions.SelectedItem.Text : "No answer";
                    var correctOpt = q.Options.FirstOrDefault(opt => opt.IsCorrect);
                    ans.CorrectAnswer = correctOpt != null ? correctOpt.OptionText : "Unknown";
                    isCorrect = rblOptions.SelectedValue == ans.CorrectAnswer;
                }
                else if (q.QuestionTypeName == "TypeInAnswer")
                {
                    ans.UserSelection = txtAnswer.Text.Trim();
                    ans.CorrectAnswer = q.CorrectAnswer;
                    isCorrect = ans.UserSelection.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
                }

                ans.IsCorrect = isCorrect;
                userAnswers.Add(ans);
                Session["UserAnswers"] = userAnswers;

                // Feedback
                pnlFeedback.Visible = true;
                if (isCorrect)
                {
                    pnlFeedback.CssClass = "alert alert-success mt-4";
                    litFeedback.Text = "<strong>Correct!</strong>";
                }
                else
                {
                    pnlFeedback.CssClass = "alert alert-danger mt-4";
                    litFeedback.Text = $"<strong>Incorrect.</strong> The correct answer was: <strong>{ans.CorrectAnswer}</strong>";
                }

                rblOptions.Enabled = false;
                txtAnswer.Enabled = false;
                btnSubmitAnswer.Visible = false;
                btnNextQuestion.Visible = true;

                if (index == currentQuiz.Questions.Count - 1)
                    btnNextQuestion.Text = "View Results";
            }
            catch (Exception ex)
            {
                ShowNotification("error", "Error submitting answer: " + ex.Message);
            }
        }

        protected void btnNextQuestion_Click(object sender, EventArgs e)
        {
            int index = GetCurrentIndex();
            if (index < currentQuiz.Questions.Count - 1) DisplayQuestion(index + 1);
            else ShowReview();
        }

        // btnBackToQuiz now handled client-side, but keeping stub just in case
        protected void btnBackToQuiz_Click(object sender, EventArgs e) { ExitQuiz(); }

        private void ShowReview()
        {
            pnlQuiz.Visible = false;
            pnlReview.Visible = true;

            int correctCount = userAnswers.Count(a => a.IsCorrect);
            int totalQuestions = userAnswers.Count;
            int scorePercent = totalQuestions > 0 ? (int)Math.Round((double)correctCount / totalQuestions * 100) : 0;

            litFinalScore.Text = $"{scorePercent}% ({correctCount} / {totalQuestions})";

            rptReview.DataSource = userAnswers;
            rptReview.DataBind();

            if (isRegistered && currentUser != null && !IsAdmin)
            {
                QuizManager manager = new QuizManager();
                manager.SaveQuizAttempt(currentQuiz.QuizID, currentUser.UserID, scorePercent);
            }

            // We don't exit immediately here; user views results then clicks "Close"
        }

        protected void lnkBack_Click(object sender, EventArgs e) { ExitQuiz(); }

        private void ExitQuiz()
        {
            Session["ActiveQuizObject"] = null;
            Session["UserAnswers"] = null;
            Session["ActiveQuizID"] = null;
            Session["QuizIndex"] = null;

            // Output script to close window instead of redirecting
            ClientScript.RegisterStartupScript(this.GetType(), "closeWindow", "<script>window.close();</script>");
        }

        private void DisplayQuestion(int index)
        {
            ViewState["QuizIndex"] = index;
            QuizQuestion q = currentQuiz.Questions[index];

            litQuizTitle.Text = currentQuiz.Title;
            litCurrentPage.Text = $"{index + 1} / {currentQuiz.Questions.Count}";

            pnlMCQ.Visible = false;
            pnlTypeInAnswer.Visible = false;
            pnlFeedback.Visible = false;

            btnSubmitAnswer.Visible = true;
            btnNextQuestion.Visible = false;
            rblOptions.Enabled = true;
            txtAnswer.Enabled = true;
            txtAnswer.Text = string.Empty;

            litQuestionText.Text = q.QuestionText;
            imgQuestion.Visible = !string.IsNullOrEmpty(q.ImagePath);
            if (imgQuestion.Visible) imgQuestion.ImageUrl = GetImagePath(q.ImagePath);

            if (q.QuestionTypeName == "MCQ")
            {
                pnlMCQ.Visible = true;
                rblOptions.Items.Clear();
                foreach (var opt in q.Options)
                    rblOptions.Items.Add(new ListItem(opt.OptionText, opt.OptionText));
            }
            else if (q.QuestionTypeName == "TypeInAnswer")
            {
                pnlTypeInAnswer.Visible = true;
            }
        }

        private int GetCurrentIndex() { return (int?)ViewState["QuizIndex"] ?? 0; }

        public string GetImagePath(object imagePath) { return (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString())) ? imagePath.ToString() : "/Image/System/placeholder.png"; }

        private void ShowNotification(string type, string message)
        {
            string script = $"showNotification('{type}', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}