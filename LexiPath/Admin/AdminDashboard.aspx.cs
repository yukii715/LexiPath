using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath.Admin
{
    public partial class AdminDashboard : LexiPath.Admin.AdminBasePage
    {
        private UserManager userManager = new UserManager();
        private CourseManager courseManager = new CourseManager();
        private QuizManager quizManager = new QuizManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindKPIs();
            }
        }

        private void BindKPIs()
        {
            // Fetch and display data from the new manager methods
            litTotalUsers.Text = userManager.GetTotalActiveUserCount().ToString();
            litTotalCourses.Text = courseManager.GetTotalActiveCourseCount().ToString();
            litTotalQuizzes.Text = quizManager.GetTotalQuizCount().ToString();
            litTotalCategories.Text = courseManager.GetTotalActiveCategoryCount().ToString();

            // You can add logic here to bind the "Recently Registered Users" if you implement that manager method later.
        }
    }
}