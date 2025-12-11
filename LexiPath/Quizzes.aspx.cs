using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class Quizzes : System.Web.UI.Page
    {
        private int currentLanguageId = 1; // Default
        private HashSet<int> attemptedQuizIds = new HashSet<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LanguageID"] != null)
            {
                currentLanguageId = (int)Session["LanguageID"];
            }

            // Check if user is logged in and get their quiz history
            if (Session["User"] != null)
            {
                User user = (User)Session["User"];
                QuizManager quizManager = new QuizManager();
                attemptedQuizIds = quizManager.GetAttemptedQuizIds(user.UserID);
            }

            if (!IsPostBack)
            {
                BindQuizzes();
            }
        }

        protected void rptQuizzes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Quiz quiz = (Quiz)e.Item.DataItem;

                if (attemptedQuizIds.Contains(quiz.QuizID))
                {
                    // Find the "Completed" tag and make it visible
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
                return "/Image/System/placeholder.png"; 
            }
        }
    }
}