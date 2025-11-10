using LexiPath.Data;
using System;
using System.Web.UI;

namespace LexiPath
{
    public partial class CourseDetail : System.Web.UI.Page
    {
        private int currentCourseId = 0;
        private User currentUser = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["CourseID"], out currentCourseId))
            {
                Response.Redirect("Courses.aspx");
            }
            hdnCourseID.Value = currentCourseId.ToString();

            if (Session["User"] != null)
            {
                currentUser = (User)Session["User"];
            }

            if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
            {
                lnkBackToCourses.NavigateUrl = Request.QueryString["ReturnUrl"];
            }
            else
            {
                lnkBackToCourses.NavigateUrl = "~/Courses.aspx";
            }

            if (!IsPostBack)
            {
                BindCourseDetails();
            }

            // *** FIX ***
            // This now runs on EVERY page load (even postbacks/reloads).
            // This stops the re-trigger loop by immediately checking
            // if the course is complete *after* the reload.
            SetPanelVisibility();
        }

        private void BindCourseDetails()
        {
            CourseManager manager = new CourseManager();
            Course course = manager.GetCourseDetails(currentCourseId);

            if (course != null)
            {
                litCourseName.Text = course.CourseName;
                litDescription.Text = course.Description;
                imgCourse.ImageUrl = GetImagePath(course.ImagePath);
                litCourseType.Text = "Type: " + course.CourseType;
            }
        }

        protected void btnStartLesson_Click(object sender, EventArgs e)
        {
            Session["ActiveCourseID"] = currentCourseId;

            if (currentUser != null)
            {
                AccessManager accessManager = new AccessManager();
                accessManager.StartCourse(currentUser.UserID, currentCourseId);
            }

            string url = ResolveUrl("~/Learn.aspx");
            string script = $"window.open('{url}', '_blank');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "openLearnTab", script, true);
        }

        /**
         * NEW: This event fires when the "Start Practice" button is clicked.
         * It loads the full quiz object into the session and opens the quiz tab.
         */
        protected void btnStartPractice_Click(object sender, EventArgs e)
        {
            // 1. Get the Practice Quiz ID from the hidden field
            int practiceQuizId = int.Parse(hdnPracticeQuizID.Value);
            if (practiceQuizId == 0) return; // Safety check

            // 2. Load the full details for this practice (questions, answers, etc.)
            QuizManager manager = new QuizManager();
            Quiz practiceQuiz = manager.GetQuizDetails(practiceQuizId);

            if (practiceQuiz != null)
            {
                // 3. Store the full object in the Session. QuizAttempt.aspx will read this.
                Session["ActiveQuizObject"] = practiceQuiz;
                Session["ActiveQuizID"] = practiceQuizId; // Set this just in case

                // 4. Use JavaScript to open the new tab
                string url = ResolveUrl("~/QuizAttempt.aspx");
                string script = $"window.open('{url}', '_blank');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "openPracticeTab", script, true);
            }
        }

        /**
         * UPDATED: This logic now changes the button text
         * to "Review Lesson" when the course is complete.
         */
        private void SetPanelVisibility()
        {
            if (currentUser == null)
            {
                // --- USER IS A GUEST ---
                pnlLearn.Visible = true;
                litLearnMessage.Text = "As a guest, your progress will not be saved.";
                btnStartLesson.Text = "Start Lesson";
                pnlLoginRequired.Visible = true;
                pnlPractice.Visible = false;
                pnlPracticeLocked.Visible = false;
            }
            else
            {
                // --- USER IS LOGGED IN ---
                pnlLearn.Visible = true;
                pnlLoginRequired.Visible = false;

                AccessManager accessManager = new AccessManager();
                bool isComplete = accessManager.IsCourseComplete(currentUser.UserID, currentCourseId);

                if (isComplete)
                {
                    // Course is complete!
                    btnStartLesson.Text = "Review Lesson"; // Change button text
                    litLearnMessage.Text = "You have completed this course! You can review the lesson, or try the practice module below.";
                    pnlPractice.Visible = true; // Show practice
                    pnlPracticeLocked.Visible = false;

                    QuizManager quizManager = new QuizManager();
                    int practiceQuizId = quizManager.GetPracticeQuizIDByCourseID(currentCourseId);

                    if (practiceQuizId > 0)
                    {
                        // 2. A practice was found! Show the button.
                        hdnPracticeQuizID.Value = practiceQuizId.ToString();
                        btnStartPractice.Visible = true;
                        lblNoPractice.Visible = false;
                    }
                    else
                    {
                        // 3. No practice found. Show the "not available" message.
                        btnStartPractice.Visible = false;
                        lblNoPractice.Visible = true;
                    }
                }
                else
                {
                    // Course is NOT complete
                    btnStartLesson.Text = "Start Lesson"; // Set button text
                    litLearnMessage.Text = "Your progress will be saved upon completion.";
                    pnlPractice.Visible = false;
                    pnlPracticeLocked.Visible = true; // Show "Practice Locked"
                }
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