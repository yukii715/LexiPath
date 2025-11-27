using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath.Admin
{
    public partial class ManageQuizzes : AdminBasePage
    {
        private QuizManager manager = new QuizManager();
        private CourseManager courseManager = new CourseManager();

        // ViewState to store selected courses for the NEW quiz (Bubble System)
        protected List<int> NewQuizSelectedCourseIDs
        {
            get { return (List<int>)(ViewState["NewQuizSelectedCourseIDs"] ?? new List<int>()); }
            set { ViewState["NewQuizSelectedCourseIDs"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();

                // Initialize Add Modal Dropdowns with Placeholders
                BindLanguageDropdown();
                BindDropdownWithPlaceholder(ddlNewCategory, null, "-- Select Language First --");
                BindDropdownWithPlaceholder(ddlNewPracticeCategory, null, "-- Select Language First --");
                BindDropdownWithPlaceholder(ddlNewPracticeCourse, null, "-- Select Category First --");
            }
        }

        #region Data Binding & Helpers

        private void BindLanguageDropdown()
        {
            BindDropdownWithPlaceholder(ddlNewLanguage, courseManager.GetAllLanguages(), "-- Select Language --", "LanguageName", "LanguageID");
        }

        // Smart Dropdown Helper (Handles Empty vs Placeholder)
        private void BindDropdownWithPlaceholder(DropDownList ddl, object dataSource, string defaultText, string textField = "", string valueField = "")
        {
            ddl.Items.Clear();
            ddl.ClearSelection();

            if (dataSource != null)
            {
                ddl.DataSource = dataSource;
                if (!string.IsNullOrEmpty(textField))
                {
                    ddl.DataTextField = textField;
                    ddl.DataValueField = valueField;
                }
                ddl.DataBind();
            }

            // If data source was empty but not null (e.g. Language has no Categories)
            if (ddl.Items.Count == 0 && dataSource != null)
            {
                ListItem noItem = new ListItem(defaultText.Replace("Select", "No").Replace("First", "Found"), "0");
                noItem.Attributes.Add("disabled", "disabled");
                noItem.Selected = true;
                ddl.Items.Add(noItem);
            }
            else
            {
                // Standard Placeholder
                ListItem placeholder = new ListItem(defaultText, "0");
                placeholder.Attributes.Add("disabled", "disabled");
                placeholder.Selected = true;
                ddl.Items.Insert(0, placeholder);
            }
        }

        private void BindGrid()
        {
            DataTable data = manager.GetAllQuizzesForAdmin();
            if (ViewState["SortExpression"] != null)
            {
                data.DefaultView.Sort = $"{ViewState["SortExpression"]} {ViewState["SortDirection"]}";
            }
            gvQuizzes.DataSource = data.DefaultView;
            gvQuizzes.DataBind();
        }

        private string HandleFileUpload(FileUpload uploader, string subfolder)
        {
            string dbPath = null;
            if (uploader.HasFile)
            {
                string extension = Path.GetExtension(uploader.FileName);
                string fileName = $"{Guid.NewGuid()}{extension}";
                string savePath = Server.MapPath($"~/Image/{subfolder}/");
                string fullPath = Path.Combine(savePath, fileName);
                Directory.CreateDirectory(savePath);
                uploader.SaveAs(fullPath);
                dbPath = $"/Image/{subfolder}/{fileName}";
            }
            return dbPath;
        }

        public string GetImagePath(object imagePath)
        {
            return (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString())) ? imagePath.ToString() : "/Image/System/placeholder.png";
        }

        private void ShowNotification(string type, string message)
        {
            string safeMessage = message.Replace("'", "\\'").Replace("\r", "").Replace("\n", "");
            string script = $"showNotification('{type}', '{safeMessage}');";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "SweetAlert_" + Guid.NewGuid(), script, true);
        }

        private void ShowAddModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showAddModal", "showAddModal();", true);
        }

        private void HideAddModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "hideAddModal", "hideAddModal();", true);
        }

        private bool IsValidImage(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            return (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp" || ext == ".bmp" || ext == ".svg");
        }

        #endregion

        #region Add Modal Logic (Cascading & Bubbles)

        protected void ddlNewLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);

            // Cascade to Categories (Both Quiz and Practice)
            var cats = (langId > 0) ? courseManager.GetCategoriesByLanguage(langId) : null;

            BindDropdownWithPlaceholder(ddlNewCategory, cats, "-- Filter by Category --", "CategoryName", "CategoryID");
            BindDropdownWithPlaceholder(ddlNewPracticeCategory, cats, "-- Filter by Category --", "CategoryName", "CategoryID");

            // Reset sub-lists
            BindDropdownWithPlaceholder(ddlNewPracticeCourse, null, "-- Select Category First --");
            RefreshAvailableCoursesUI();

            // Reset Selected Bubbles on Language Change
            NewQuizSelectedCourseIDs = new List<int>();
            RefreshSelectedCoursesUI();

            ShowAddModal();
        }

        // --- QUIZ MODE: AVAILABLE COURSES GRID ---
        protected void ddlNewCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshAvailableCoursesUI();
            ShowAddModal();
        }

        private void RefreshAvailableCoursesUI()
        {
            int catId = Convert.ToInt32(ddlNewCategory.SelectedValue);
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);

            gvNewAvailableCourses.DataSource = null;
            if (langId > 0)
            {
                List<Course> courses;
                // Logic: If Category is 0, show ALL courses for Language. If > 0, filter by Category.
                if (catId > 0)
                    courses = courseManager.GetCoursesByCategoryID(catId, "All", null, langId);
                else
                    courses = courseManager.GetAllCourses("All", null, langId);

                // Filter out already selected bubbles
                List<int> selected = NewQuizSelectedCourseIDs;
                var filtered = courses.Where(c => !selected.Contains(c.CourseID)).ToList();
                gvNewAvailableCourses.DataSource = filtered;
            }
            gvNewAvailableCourses.DataBind();
        }

        protected void gvNewAvailableCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AddCourse")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                List<int> list = NewQuizSelectedCourseIDs;
                if (!list.Contains(id)) { list.Add(id); NewQuizSelectedCourseIDs = list; }

                RefreshSelectedCoursesUI();
                RefreshAvailableCoursesUI();
                ShowAddModal();
            }
        }

        // --- QUIZ MODE: SELECTED BUBBLES ---
        private void RefreshSelectedCoursesUI()
        {
            List<int> ids = NewQuizSelectedCourseIDs;
            if (ids.Count > 0)
            {
                int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
                if (langId == 0) langId = 1; // Fallback to prevent crash if lang reset
                var allCourses = courseManager.GetAllCourses("All", null, langId);

                var displayList = allCourses
                    .Where(c => ids.Contains(c.CourseID))
                    .Select(c => new { c.CourseID, c.CourseName })
                    .ToList();

                rptNewSelectedCourses.DataSource = displayList;
                rptNewSelectedCourses.DataBind();
                pnlNewNoSelection.Visible = false;
            }
            else
            {
                rptNewSelectedCourses.DataSource = null;
                rptNewSelectedCourses.DataBind();
                pnlNewNoSelection.Visible = true;
            }
        }

        protected void rptNewSelectedCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveCourse")
            {
                int id = Convert.ToInt32(e.CommandArgument);
                List<int> list = NewQuizSelectedCourseIDs;
                list.Remove(id);
                NewQuizSelectedCourseIDs = list;

                RefreshSelectedCoursesUI();
                RefreshAvailableCoursesUI();
                ShowAddModal();
            }
        }

        // --- PRACTICE MODE: CATEGORY FILTER ---
        protected void ddlNewPracticeCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int catId = Convert.ToInt32(ddlNewPracticeCategory.SelectedValue);
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);

            if (catId > 0 && langId > 0)
            {
                var courses = courseManager.GetCoursesByCategoryID(catId, "All", null, langId);
                BindDropdownWithPlaceholder(ddlNewPracticeCourse, courses, "-- Select Course --", "CourseName", "CourseID");
            }
            else
            {
                BindDropdownWithPlaceholder(ddlNewPracticeCourse, null, "-- Select Category First --");
            }
            ShowAddModal();
        }

        // --- ADD BUTTON CLICK ---
        protected void btnAddQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtNewTitle.Text.Trim();
                string desc = txtNewDescription.Text.Trim();
                int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
                bool isPractice = chkNewIsPractice.Checked;
                int[] courseIds;

                // 1. Validation
                if (string.IsNullOrEmpty(title)) { ShowNotification("error", "Title is required."); ShowAddModal(); return; }
                if (langId == 0) { ShowNotification("warning", "Please select a Language."); ShowAddModal(); return; }

                // 2. Logic Specific Validation
                if (isPractice)
                {
                    if (ddlNewPracticeCourse.SelectedValue == "0")
                    {
                        ShowNotification("warning", "Select a dedicated course for Practice."); ShowAddModal(); return;
                    }
                    courseIds = new int[] { Convert.ToInt32(ddlNewPracticeCourse.SelectedValue) };
                }
                else
                {
                    // Use ViewState List for Quiz
                    courseIds = NewQuizSelectedCourseIDs.ToArray();
                    if (courseIds.Length < 2)
                    {
                        ShowNotification("warning", "Select at least 2 courses for a Quiz."); ShowAddModal(); return;
                    }
                }

                // 3. File Upload
                string dbPath = null;
                if (fileUploadNewImage.HasFile)
                {
                    if (!IsValidImage(fileUploadNewImage.FileName)) { ShowNotification("error", "Invalid image."); ShowAddModal(); return; }
                    dbPath = HandleFileUpload(fileUploadNewImage, "Quiz");
                }

                // 4. Create (Passing NULL for tags)
                if (manager.CreateQuiz(title, desc, langId, isPractice, dbPath, courseIds, null))
                {
                    BindGrid();

                    // Reset Form
                    txtNewTitle.Text = "";
                    txtNewDescription.Text = "";
                    ddlNewLanguage.SelectedIndex = 0;
                    chkNewIsPractice.Checked = false;
                    NewQuizSelectedCourseIDs = new List<int>(); // Clear bubbles
                    RefreshSelectedCoursesUI();

                    // Reset Placeholders
                    BindDropdownWithPlaceholder(ddlNewCategory, null, "-- Select Language First --");
                    BindDropdownWithPlaceholder(ddlNewPracticeCategory, null, "-- Select Language First --");
                    BindDropdownWithPlaceholder(ddlNewPracticeCourse, null, "-- Select Category First --");

                    HideAddModal();
                    ShowNotification("success", "Created successfully!");
                }
                else
                {
                    ShowNotification("error", "Failed to add.");
                    ShowAddModal();
                }
            }
            catch (Exception ex) { ShowNotification("error", "Error: " + ex.Message); ShowAddModal(); }
        }

        #endregion

        #region GridView Events

        protected void gvQuizzes_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["SortExpression"] = e.SortExpression;
            string currentSortDirection = ViewState["SortDirection"] as string;
            ViewState["SortDirection"] = (currentSortDirection == "ASC" ? "DESC" : "ASC");
            BindGrid();
        }

        protected void gvQuizzes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteQuiz")
            {
                try
                {
                    manager.DeactivatedQuiz(Convert.ToInt32(e.CommandArgument));
                    BindGrid();
                    ShowNotification("warning", "Deactivated.");
                }
                catch (Exception ex) { ShowNotification("error", "Error: " + ex.Message); }
            }
            else if (e.CommandName == "EditQuiz")
            {
                Session["Admin_EditQuizID"] = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("EditQuiz.aspx");
            }
        }

        #endregion
    }
}