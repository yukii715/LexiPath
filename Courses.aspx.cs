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
        // This variable will hold the current filter type
        private int currentLanguageId = 1; // Default
        private string currentCourseType = "All"; // Default to "All"
        private HashSet<int> completedCourseIds = new HashSet<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            // Read the 'type' from the URL first
            if (!string.IsNullOrEmpty(Request.QueryString["type"]))
            {
                currentCourseType = Request.QueryString["type"];
            }

            // --- NEW: Read LanguageID from Session ---
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
                // Check if a CategoryID was passed in the URL
                if (!string.IsNullOrEmpty(Request.QueryString["CategoryID"]))
                {
                    int categoryId = 0;
                    bool isValid = int.TryParse(Request.QueryString["CategoryID"], out categoryId);

                    if (isValid)
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
                    // No CategoryID, show ALL courses
                    BindAllCourses();
                }

                // After binding, set the active link in the sub-menu
                SetActiveFilterLink();
            }
        }

        // Add this NEW event handler for the Repeater
        protected void rptCourses_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // 1. Get the data for this course
                Course course = (Course)e.Item.DataItem;

                // 2. Check if the user has completed this course
                if (completedCourseIds.Contains(course.CourseID))
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

        /**
         * NEW: This event fires when the search button is clicked
         */
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // We need to re-bind the correct view (Category or All)
            if (!string.IsNullOrEmpty(Request.QueryString["CategoryID"]))
            {
                int categoryId = 0;
                int.TryParse(Request.QueryString["CategoryID"], out categoryId);
                BindPageByCategory(categoryId); // Re-bind with category
            }
            else
            {
                BindAllCourses(); // Re-bind all courses
            }

            // Also need to re-set the active filter link
            SetActiveFilterLink();
        }

        /**
         * UPDATED: This method now reads the search term
         */
        private void BindPageByCategory(int categoryId)
        {
            CourseManager manager = new CourseManager();

            // --- FIX: Read the search term from the textbox ---
            string searchTerm = txtSearch.Text.Trim();

            Category category = manager.GetCategoryDetails(categoryId);
            if (category != null)
            {
                litCategoryName.Text = "Courses in: " + category.CategoryName;
            }

            // --- FIX: Pass ALL parameters to the manager ---
            List<Course> courses = manager.GetCoursesByCategoryID(categoryId, currentCourseType, searchTerm, currentLanguageId);

            if (courses.Count > 0)
            {
                rptCourses.DataSource = courses;
                rptCourses.DataBind();
                pnlNoCourses.Visible = false;
            }
            else
            {
                rptCourses.DataSource = null; // Clear old data
                rptCourses.DataBind();
                pnlNoCourses.Visible = true;
            }

            // Update the sub-menu links (this part was correct)
            lnkAll.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=All";
            lnkMixed.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=Mixed";
            lnkVocab.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=Vocabulary";
            lnkPhrase.NavigateUrl = $"~/Courses.aspx?CategoryID={categoryId}&type=Phrase";
        }

        /**
         * UPDATED: This method now reads the search term
         */
        private void BindAllCourses()
        {
            CourseManager manager = new CourseManager();

            // --- FIX: Read the search term from the textbox ---
            string searchTerm = txtSearch.Text.Trim();

            litCategoryName.Text = "All Courses";
            lnkBack.Visible = false;

            // --- FIX: Pass ALL parameters to the manager ---
            List<Course> courses = manager.GetAllCourses(currentCourseType, searchTerm, currentLanguageId);

            if (courses.Count > 0)
            {
                rptCourses.DataSource = courses;
                rptCourses.DataBind();
                pnlNoCourses.Visible = false;
            }
            else
            {
                rptCourses.DataSource = null; // Clear old data
                rptCourses.DataBind();
                pnlNoCourses.Visible = true;
            }

            // Update the sub-menu links (this part was correct)
            lnkAll.NavigateUrl = "~/Courses.aspx?type=All";
            lnkMixed.NavigateUrl = "~/Courses.aspx?type=Mixed";
            lnkVocab.NavigateUrl = "~/Courses.aspx?type=Vocabulary";
            lnkPhrase.NavigateUrl = "~/Courses.aspx?type=Phrase";
        }

        // NEW METHOD: This adds the "active" class to the correct link
        private void SetActiveFilterLink()
        {
            // Reset all links
            lnkAll.CssClass = "nav-link"; // Add this line
            lnkMixed.CssClass = "nav-link";
            lnkVocab.CssClass = "nav-link";
            lnkPhrase.CssClass = "nav-link";

            // Set the active one
            switch (currentCourseType)
            {
                case "Mixed":
                    lnkMixed.CssClass = "nav-link active";
                    break;
                case "Vocabulary":
                    lnkVocab.CssClass = "nav-link active";
                    break;
                case "Phrase":
                    lnkPhrase.CssClass = "nav-link active";
                    break;
                case "All":
                default:
                    lnkAll.CssClass = "nav-link active"; // Update default
                    break;
            }
        }

        // You can keep your existing GetImagePath helper function
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

        // This is the new helper function
        public string GetDetailUrl(object courseId)
        {
            // 1. Get the current URL (e.g., /Courses.aspx?CategoryID=1&type=Mixed)
            string rawReturnUrl = Request.RawUrl;

            // 2. URL-Encode it so it can be safely passed in another URL
            string encodedReturnUrl = Server.UrlEncode(rawReturnUrl);

            // 3. Build the new link
            return $"CourseDetail.aspx?CourseID={courseId}&ReturnUrl={encodedReturnUrl}";
        }
    }
}