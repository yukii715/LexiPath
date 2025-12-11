using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class CourseDetail : System.Web.UI.Page
    {
        private int currentCourseId = 0;
        private User currentUser = null;
        private UserManager userManager = new UserManager();
        private ForumManager forumManager = new ForumManager();

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
                BindPractices();
                BindForum();
            }

            SetPanelVisibility();
            CheckForumAccess();
        }

        private void CheckForumAccess()
        {
            if (currentUser == null)
            {
                txtNewPost.Enabled = false;
                btnSubmitPost.Enabled = false;
                pnlLoginToPost.Visible = true;
                txtNewPost.Attributes["placeholder"] = "Please login to join the discussion.";
            }
        }

        private void BindForum()
        {
            var posts = forumManager.GetPostsByCourseID(currentCourseId);
            rptForum.DataSource = posts;
            rptForum.DataBind();

            lblNoPosts.Visible = (posts.Count == 0);
        }

        protected void btnSubmitPost_Click(object sender, EventArgs e)
        {
            if (currentUser != null && !string.IsNullOrWhiteSpace(txtNewPost.Text))
            {
                forumManager.CreatePost(currentCourseId, currentUser.UserID, txtNewPost.Text.Trim());
                txtNewPost.Text = ""; // Clear input
                BindForum(); // Refresh list
            }
        }

        // Runs for every Post in the Repeater to load its comments
        protected void rptForum_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // Get the data for this row
                ForumPostDisplay post = (ForumPostDisplay)e.Item.DataItem;

                // Find the inner repeater
                Repeater rptComments = (Repeater)e.Item.FindControl("rptComments");

                // Load comments
                rptComments.DataSource = forumManager.GetCommentsByPostID(post.PostID);
                rptComments.DataBind();

                // Handle Login State for Comment Box
                TextBox txtComment = (TextBox)e.Item.FindControl("txtNewComment");
                Button btnReply = (Button)e.Item.FindControl("btnPostComment");
                if (currentUser == null)
                {
                    txtComment.Enabled = false;
                    btnReply.Enabled = false;
                    txtComment.Attributes["placeholder"] = "Login to reply";
                }
            }
        }

        // Handles the "Reply" button click
        protected void rptForum_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "AddComment")
            {
                if (currentUser == null) return;

                int postId = Convert.ToInt32(e.CommandArgument);
                TextBox txtComment = (TextBox)e.Item.FindControl("txtNewComment");
                string content = txtComment.Text.Trim();

                if (!string.IsNullOrWhiteSpace(content))
                {
                    forumManager.CreateComment(postId, currentUser.UserID, content);
                    BindForum(); // Refresh everything to show new comment
                }
            }
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

        private void BindPractices()
        {
            QuizManager quizManager = new QuizManager();
            // This gets ONLY active practices
            List<Quiz> practices = quizManager.GetActivePracticesByCourseID(currentCourseId);

            rptPractices.DataSource = practices;
            rptPractices.DataBind();

            // If no practices, hide the repeater area
            bool hasPractices = practices.Count > 0;
            rptPractices.Visible = hasPractices;
            lblNoPractice.Visible = !hasPractices;
        }

        protected void btnStartLesson_Click(object sender, EventArgs e)
        {
            Session["ActiveCourseID"] = currentCourseId;
            Session["ReturnCourseID"] = currentCourseId;

            if (currentUser != null)
            {
                if (!currentUser.IsAdmin)
                {
                    AccessManager accessManager = new AccessManager();
                    accessManager.StartCourse(currentUser.UserID, currentCourseId);
                }
            }

            Response.Redirect("~/Learn.aspx");
        }

        protected void rptPractices_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "StartPractice")
            {
                int quizId = Convert.ToInt32(e.CommandArgument);
                QuizManager manager = new QuizManager();
                Quiz practiceQuiz = manager.GetQuizDetails(quizId);

                if (practiceQuiz != null)
                {
                    Session["ActiveQuizObject"] = practiceQuiz;
                    Session["ActiveQuizID"] = quizId;
                    Session["ReturnCourseID"] = currentCourseId;

                    string url = ResolveUrl("~/QuizAttempt.aspx");

                    string script = $"window.open('{url}', '_blank');";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "openPracticeTab", script, true);
                }
            }
        }

        private void SetPanelVisibility()
        {
            if (currentUser == null)
            {
                pnlLearn.Visible = true;
                litLearnMessage.Text = "As a guest, your progress will not be saved.";
                btnStartLesson.Text = "Start Lesson";
                pnlLoginRequired.Visible = true;
                pnlPractice.Visible = false;
                pnlPracticeLocked.Visible = false;
                pnlInteractions.Visible = false;
            }
            else
            {
                pnlLearn.Visible = true;
                pnlLoginRequired.Visible = false;

                bool isAdmin = currentUser.IsAdmin;
                pnlInteractions.Visible = !isAdmin;

                if (!isAdmin)
                {
                    var status = userManager.GetInteractionStatus(currentUser.UserID, currentCourseId);
                    if (status["IsCollected"]) { litCollectHtml.Text = "<i class=\"bi bi-bookmark-fill\"></i> Collected"; btnToggleCollect.CssClass = "btn btn-info"; }
                    else { litCollectHtml.Text = "<i class=\"bi bi-bookmark-fill\"></i> Collect"; btnToggleCollect.CssClass = "btn btn-outline-info"; }

                    if (status["IsLiked"]) { litLikeHtml.Text = "<i class=\"bi bi-heart-fill\"></i> Liked"; btnToggleLike.CssClass = "btn btn-danger"; }
                    else { litLikeHtml.Text = "<i class=\"bi bi-heart-fill\"></i> Like"; btnToggleLike.CssClass = "btn btn-outline-danger"; }
                }

                bool isComplete = isAdmin ? true : new AccessManager().IsCourseComplete(currentUser.UserID, currentCourseId);

                if (isComplete)
                {
                    btnStartLesson.Text = isAdmin ? "Test Lesson" : "Review Lesson";
                    litLearnMessage.Text = isAdmin
                        ? "Admin Mode: You have full access."
                        : "You have completed this course! You can review the lesson, or try the practice module below.";

                    pnlPractice.Visible = true; 
                    pnlPracticeLocked.Visible = false;

                }
                else
                {
                    btnStartLesson.Text = "Start Lesson";
                    litLearnMessage.Text = "Your progress will be saved upon completion.";
                    pnlPractice.Visible = false;
                    pnlPracticeLocked.Visible = true;
                }
            }
        }

        public string GetImagePath(object imagePath)
        {
            return (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString())) ? imagePath.ToString() : "/Image/System/placeholder.png";
        }

        protected void btnToggleCollect_Click(object sender, EventArgs e)
        {
            if (currentUser != null && !currentUser.IsAdmin)
            {
                userManager.ToggleCollection(currentUser.UserID, currentCourseId);
                SetPanelVisibility();
            }
        }

        protected void btnToggleLike_Click(object sender, EventArgs e)
        {
            if (currentUser != null && !currentUser.IsAdmin)
            {
                userManager.ToggleLike(currentUser.UserID, currentCourseId);
                SetPanelVisibility();
            }
        }
    }
}