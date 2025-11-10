using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class Quizzes : System.Web.UI.Page
    {
        // Add this new property at the top of your class
        private int currentLanguageId = 1; // Default
        private HashSet<int> attemptedQuizIds = new HashSet<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- NEW: Read LanguageID from Session ---
            if (Session["LanguageID"] != null)
            {
                currentLanguageId = (int)Session["LanguageID"];
            }

            // --- NEW LOGIC ---
            // Check if user is logged in and get their quiz history
            if (Session["User"] != null)
            {
                User user = (User)Session["User"];
                QuizManager quizManager = new QuizManager();
                attemptedQuizIds = quizManager.GetAttemptedQuizIds(user.UserID);
            }
            // --- END NEW LOGIC ---

            if (!IsPostBack)
            {
                BindQuizzes();
            }
        }

        // Add this NEW event handler for the Repeater
        protected void rptQuizzes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // 1. Get the data for this quiz
                Quiz quiz = (Quiz)e.Item.DataItem;

                // 2. Check if the user has attempted this quiz
                if (attemptedQuizIds.Contains(quiz.QuizID))
                {
                    // 3. Find the "Completed" tag and make it visible
                    Literal litCompletedTag = (Literal)e.Item.FindControl("litCompletedTag");
                    if (litCompletedTag != null)
                    {
                        litCompletedTag.Visible = true;
                    }
                }
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindQuizzes();
        }

        private void BindQuizzes()
        {
            QuizManager quizManager = new QuizManager();
            string searchTerm = txtSearch.Text.Trim();

            // isPractice: null for both quizzes and practice, true for only practice, false for only quizzes
            // Here, we want to show all quizzes (IsPractice = 0)
            List<Quiz> quizzes = quizManager.GetAllQuizzes(searchTerm, false, currentLanguageId);

            if (quizzes.Count > 0)
            {
                rptQuizzes.DataSource = quizzes;
                rptQuizzes.DataBind();
                pnlNoQuizzes.Visible = false;
            }
            else
            {
                rptQuizzes.DataSource = null;
                rptQuizzes.DataBind();
                pnlNoQuizzes.Visible = true;
            }
        }

        public string GetImagePath(object imagePath)
        {
            if (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString()))
            {
                return imagePath.ToString();
            }
            else
            {
                return "/Image/System/placeholder.png"; // Make sure this path exists
            }
        }
    }
}