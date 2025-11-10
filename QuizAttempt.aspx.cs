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
        // Class-level variables to hold quiz state
        private Quiz currentQuiz;
        private User currentUser = null;
        private bool isRegistered = false;
        private List<QuizUserAnswer> userAnswers;
        public bool IsUserRegistered { get; set; } = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Load User and Quiz from Session
            if (Session["User"] != null)
            {
                currentUser = (User)Session["User"];
                isRegistered = true;
                IsUserRegistered = true;
            }

            if (Session["ActiveQuizObject"] == null)
            {
                // No quiz started, close this tab.
                ClientScript.RegisterStartupScript(this.GetType(), "close", "window.close();", true);
                return;
            }

            currentQuiz = (Quiz)Session["ActiveQuizObject"];

            // 2. Manage the list of answers
            if (!IsPostBack)
            {
                // First time page load: Create a new answer list
                userAnswers = new List<QuizUserAnswer>();
                Session["UserAnswers"] = userAnswers;
                DisplayQuestion(0);
            }
            else
            {
                // On postback: Get the answer list from Session
                userAnswers = (List<QuizUserAnswer>)Session["UserAnswers"];
            }
        }

        /**
         * This button checks the user's answer and provides immediate feedback.
         */
        protected void btnSubmitAnswer_Click(object sender, EventArgs e)
        {
            int index = GetCurrentIndex();
            QuizQuestion q = currentQuiz.Questions[index];

            // Create a new answer object to store results
            QuizUserAnswer ans = new QuizUserAnswer();
            ans.QuestionText = q.QuestionText;
            bool isCorrect = false;

            // --- 1. Grade the Answer ---
            if (q.QuestionTypeName == "MCQ")
            {
                ans.UserSelection = rblOptions.SelectedItem != null ? rblOptions.SelectedItem.Text : "No answer";
                ans.CorrectAnswer = q.Options.First(opt => opt.IsCorrect).OptionText;
                isCorrect = rblOptions.SelectedValue == ans.CorrectAnswer;
            }
            else if (q.QuestionTypeName == "TypeInAnswer")
            {
                ans.UserSelection = txtAnswer.Text.Trim();
                ans.CorrectAnswer = q.CorrectAnswer;
                isCorrect = ans.UserSelection.Equals(q.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
            }
            ans.IsCorrect = isCorrect;

            // --- 2. Store the Answer for Review ---
            userAnswers.Add(ans);
            Session["UserAnswers"] = userAnswers;

            // --- 3. Show Immediate Feedback ---
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

            // --- 4. Update UI ---
            // Disable inputs
            rblOptions.Enabled = false;
            txtAnswer.Enabled = false;

            // Swap buttons
            btnSubmitAnswer.Visible = false;
            btnNextQuestion.Visible = true;

            // Check if it's the last question
            if (index == currentQuiz.Questions.Count - 1)
            {
                btnNextQuestion.Text = "View Results";
            }
        }

        /**
         * This button moves to the next question OR shows the review panel.
         */
        protected void btnNextQuestion_Click(object sender, EventArgs e)
        {
            int index = GetCurrentIndex();
            if (index < currentQuiz.Questions.Count - 1)
            {
                // Go to next question
                DisplayQuestion(index + 1);
            }
            else
            {
                // Last question: Show the review
                ShowReview();
            }
        }

        /**
         * This button closes the tab and refreshes the opener.
         */
        protected void btnBackToQuiz_Click(object sender, EventArgs e)
        {
            // We've already set the JS flag, now just call the helper
            CloseAndRefreshOpener();
        }


        // --- Helper Methods ---

        /**
         * Populates the page with the question at the given index.
         */
        private void DisplayQuestion(int index)
        {
            // Set index
            ViewState["QuizIndex"] = index;
            QuizQuestion q = currentQuiz.Questions[index];

            // Reset UI
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

            // Populate Question
            litQuestionText.Text = q.QuestionText;
            imgQuestion.Visible = !string.IsNullOrEmpty(q.ImagePath);
            if (imgQuestion.Visible)
            {
                imgQuestion.ImageUrl = GetImagePath(q.ImagePath);
            }

            // Show the correct answer panel
            if (q.QuestionTypeName == "MCQ")
            {
                pnlMCQ.Visible = true;
                rblOptions.Items.Clear();
                foreach (var opt in q.Options)
                {
                    // Use Value for checking, Text for display
                    rblOptions.Items.Add(new ListItem(opt.OptionText, opt.OptionText));
                }
            }
            else if (q.QuestionTypeName == "TypeInAnswer")
            {
                pnlTypeInAnswer.Visible = true;
            }
        }

        /**
         * Hides the quiz panel and shows the final review.
         */
        private void ShowReview()
        {
            pnlQuiz.Visible = false;
            pnlReview.Visible = true;

            // 1. Calculate Score
            int correctCount = userAnswers.Count(a => a.IsCorrect);
            int totalQuestions = userAnswers.Count;
            int scorePercent = (int)Math.Round((double)correctCount / totalQuestions * 100);
            litFinalScore.Text = $"{scorePercent}% ({correctCount} / {totalQuestions})";

            // 2. Bind the review repeater
            rptReview.DataSource = userAnswers;
            rptReview.DataBind();

            // 3. Save to DB if registered
            if (isRegistered && currentUser != null)
            {
                QuizManager manager = new QuizManager();
                manager.SaveQuizAttempt(currentQuiz.QuizID, currentUser.UserID, scorePercent);
            }

            // 4. Clean up session
            Session["ActiveQuizObject"] = null;
            Session["ActiveQuizID"] = null;
            // We keep UserAnswers in session so the repeater can re-bind on postback
        }

        private int GetCurrentIndex()
        {
            return (int?)ViewState["QuizIndex"] ?? 0;
        }

        public string GetImagePath(object imagePath)
        {
            if (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString()))
            {
                return imagePath.ToString();
            }
            else
            {
                return "/Image/System/placeholder.png";
            }
        }

        /**
 * NEW: Click event for the 'X' button (lnkBack)
 * This button does NOT save progress.
 */
        protected void lnkBack_Click(object sender, EventArgs e)
        {
            // This runs *after* the JavaScript confirm()
            // It does NOT save progress, just closes the tab and refreshes.
            CloseAndRefreshOpener();
        }

        /**
         * NEW: Helper function to close tab and refresh opener
         */
        private void CloseAndRefreshOpener()
        {
            // Clean up session
            Session["ActiveQuizObject"] = null;
            Session["UserAnswers"] = null;
            Session["ActiveQuizID"] = null;

            // This script tells the original tab to refresh, then closes this tab
            string script = @"
                if (window.opener && !window.opener.closed) {
                    window.opener.location.href = window.opener.location.href;
                }
                var isQuizComplete = true; // Stop the onbeforeunload prompt
                window.onbeforeunload = null; 
                window.close();
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAndClose", script, true);
        }
    }
}