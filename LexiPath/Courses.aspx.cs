using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class Courses : System.Web.UI.Page
    {
        private int currentLanguageId = 1;
        private string currentCourseType = "All";
        private HashSet<int> completedCourseIds = new HashSet<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["type"]))
            {
                currentCourseType = Request.QueryString["type"];
            }

            if (Session["LanguageID"] != null)
            {
                currentLanguageId = (int)Session["LanguageID"];
            }

            if (Session["User"] != null)
            {
                User user = (User)Session["User"];
                AccessManager accessManager = new AccessManager();
                completedCourseIds = accessManager.GetCompletedCourseIds(user.UserID);
            }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["CategoryID"]))
                {
                    int categoryId = 0;
                    if (int.TryParse(Request.QueryString["CategoryID"], out categoryId))
                    {
                        BindPageByCategory(categoryId);
                    }
                    else
                    {
                        Response.Redirect("Courses.aspx");
                    }
                }
                else
                {
                    BindAllCourses();
                }

                SetActiveFilterLink();
            }
        }

        protected void rptCourses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Course course = (Course)e.Item.DataItem;
                if (completedCourseIds.Contains(course.CourseID))
                {
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
            if (!string.IsNullOrEmpty(Request.QueryString["CategoryID"]))
            {
                int categoryId = 0;
                int.TryParse(Request.QueryString["CategoryID"], out categoryId);
                BindPageByCategory(categoryId);
            }
            else
            {
                BindAllCourses();
            }
            SetActiveFilterLink();
        }

        private void BindPageByCategory(int categoryId)
        {
            CourseManager manager = new CourseManager();
            string searchTerm = txtSearch.Text.Trim();

            Category category = manager.GetCategoryDetails(categoryId);
            if (category != null)
            {
                litCategoryName.Text = "Courses in: " + category.CategoryName;
            }

            List<Course> courses = manager.GetCoursesByCategoryID(categoryId, currentCourseType, searchTerm, currentLanguageId);

            if (courses.Count > 0)
            {
                rptCourses.DataSource = courses;
                rptCourses.DataBind();
                pnlNoCourses.Visible = false;
            }
            else
            {
                rptCourses.DataSource = null;
                rptCourses.DataBind();
                pnlNoCourses.Visible = true;
            }

            lnkAll.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=All";
            lnkMixed.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=Mixed";
            lnkVocab.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=Vocabulary";
            lnkPhrase.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=Phrase";
        }

        private void BindAllCourses()
        {
            CourseManager manager = new CourseManager();
            string searchTerm = txtSearch.Text.Trim();

            litCategoryName.Text = "All Courses";
            lnkBack.Visible = false;

            List<Course> courses = manager.GetAllCourses(currentCourseType, searchTerm, currentLanguageId);

            if (courses.Count > 0)
            {
                rptCourses.DataSource = courses;
                rptCourses.DataBind();
                pnlNoCourses.Visible = false;
            }
            else
            {
                rptCourses.DataSource = null;
                rptCourses.DataBind();
                pnlNoCourses.Visible = true;
            }

            lnkAll.NavigateUrl = "~/Courses.aspx?type=All";
            lnkMixed.NavigateUrl = "~/Courses.aspx?type=Mixed";
            lnkVocab.NavigateUrl = "~/Courses.aspx?type=Vocabulary";
            lnkPhrase.NavigateUrl = "~/Courses.aspx?type=Phrase";
        }

        // --- UPDATED: Logic to swap button styles ---
        private void SetActiveFilterLink()
        {
            // Base styles
            string inactiveStyle = "btn btn-outline-primary rounded-pill px-4 border-2 fw-medium";
            string activeStyle = "btn btn-primary rounded-pill px-4 shadow fw-bold";

            // Reset all to inactive first
            lnkAll.CssClass = inactiveStyle;
            lnkMixed.CssClass = inactiveStyle;
            lnkVocab.CssClass = inactiveStyle;
            lnkPhrase.CssClass = inactiveStyle;

            // Set the active one
            switch (currentCourseType)
            {
                case "Mixed":
                    lnkMixed.CssClass = activeStyle;
                    break;
                case "Vocabulary":
                    lnkVocab.CssClass = activeStyle;
                    break;
                case "Phrase":
                    lnkPhrase.CssClass = activeStyle;
                    break;
                case "All":
                default:
                    lnkAll.CssClass = activeStyle;
                    break;
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

        public string GetDetailUrl(object courseId)
        {
            string rawReturnUrl = Request.RawUrl;
            string encodedReturnUrl = Server.UrlEncode(rawReturnUrl);
            return $"CourseDetail.aspx?CourseID={courseId}&ReturnUrl={encodedReturnUrl}";
        }
    }
}