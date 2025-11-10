using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Web.UI;

namespace LexiPath
{
    public partial class QuizDetail : System.Web.UI.Page
    {
        private int currentQuizId = 0;
        private QuizManager manager = new QuizManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get QuizID from URL
            if (!int.TryParse(Request.QueryString["QuizID"], out currentQuizId))
            {
                Response.Redirect("Quizzes.aspx");
            }
            hdnQuizID.Value = currentQuizId.ToString();

            if (!IsPostBack)
            {
                BindQuizDetails();
            }
        }

        private void BindQuizDetails()
        {
            // Get *all* quiz details, including related courses and questions
            Quiz quiz = manager.GetQuizDetails(currentQuizId);

            if (quiz != null)
            {
                litQuizTitle.Text = quiz.Title;
                litDescription.Text = quiz.Description;
                imgQuiz.ImageUrl = GetImagePath(quiz.ImagePath);

                // Check if there are any related courses to show
                if (quiz.RelatedCourses.Count > 0)
                {
                    pnlRelatedCourses.Visible = true;
                    rptRelatedCourses.DataSource = quiz.RelatedCourses;
                    rptRelatedCourses.DataBind();
                }

                // Store the full quiz (with questions) in the Session for the start button
                Session["ActiveQuizObject"] = quiz;
            }
            else
            {
                // Quiz not found, send user back
                Response.Redirect("Quizzes.aspx");
            }
        }

        /**
         * This button click securely starts the quiz
         */
        protected void btnStartQuiz_Click(object sender, EventArgs e)
        {
            // The full quiz object (with questions) is already in the Session
            // from our Page_Load. Now we just set the "Active" flag.
            Session["ActiveQuizID"] = currentQuizId;

            // Use JavaScript to open the new tab
            string url = ResolveUrl("~/QuizAttempt.aspx");
            string script = $"window.open('{url}', '_blank');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "openQuizTab", script, true);
        }

        // Helper function for the "Back" button
        public string GetDetailUrl(object courseId)
        {
            // This just creates a simple link, no complex return URL needed here
            return $"CourseDetail.aspx?CourseID={courseId}";
        }

        // Helper function for images
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