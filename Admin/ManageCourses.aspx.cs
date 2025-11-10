using LexiPath.Data;
using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath.Admin
{
    public partial class ManageCourses : AdminBasePage
    {
        private CourseManager manager = new CourseManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate the "Add New" dropdowns
                BindLanguageDropdown(ddlNewLanguage);
                // Trigger the category dropdown fill
                BindCategoryDropdown(ddlNewCategory, Convert.ToInt32(ddlNewLanguage.SelectedValue));

                // Bind the main grid
                BindGrid();
            }
        }

        #region Utilities and Helpers

        // Fills a language dropdown
        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.DataSource = manager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();
        }

        // Fills a category dropdown based on the selected language
        private void BindCategoryDropdown(DropDownList ddl, int languageId)
        {
            ddl.DataSource = manager.GetCategoriesByLanguage(languageId);
            ddl.DataTextField = "CategoryName";
            ddl.DataValueField = "CategoryID";
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-- Select Category --", "0"));
        }

        // Fills the main GridView
        private void BindGrid()
        {
            DataTable data = manager.GetAllCoursesForAdmin();
            if (ViewState["SortExpression"] != null)
            {
                data.DefaultView.Sort = $"{ViewState["SortExpression"]} {ViewState["SortDirection"]}";
            }
            gvCourses.DataSource = data.DefaultView;
            gvCourses.DataBind();
        }

        // Handles file uploads
        private string HandleFileUpload(FileUpload uploader)
        {
            string dbPath = null;
            if (uploader.HasFile)
            {
                string extension = Path.GetExtension(uploader.FileName);
                string fileName = $"{Guid.NewGuid()}{extension}";
                string savePath = Server.MapPath("~/Image/Course/"); // New Folder
                string fullPath = Path.Combine(savePath, fileName);
                Directory.CreateDirectory(savePath);
                uploader.SaveAs(fullPath);
                dbPath = $"/Image/Course/{fileName}";
            }
            return dbPath;
        }

        // Helper to show modal on error
        private void ShowAddModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showAddModal",
                "var myModalEl = document.getElementById('addCourseModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.show();", true);
        }
        private void ShowEditModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showEditModal",
                "var myModalEl = document.getElementById('editCourseModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.show();", true);
        }

        #endregion

        #region Add New Course (Modal)

        // This fires when the "Language" dropdown in the ADD modal is changed
        protected void ddlNewLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-populate the category dropdown based on the new language
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
            BindCategoryDropdown(ddlNewCategory, langId);

            // Re-open the modal (since a postback closed it)
            ShowAddModal();
        }

        protected void btnAddCourse_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Get all values
                string name = txtNewCourseName.Text.Trim();
                string desc = txtNewDescription.Text.Trim();
                int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
                int catId = Convert.ToInt32(ddlNewCategory.SelectedValue);
                string courseType = ddlNewCourseType.SelectedValue;

                // 2. Handle file upload
                string dbPath = HandleFileUpload(fileUploadCourse);

                // 3. Create course
                manager.CreateCourse(name, desc, langId, catId, courseType, dbPath);
                BindGrid(); // Refresh grid
            }
            catch (Exception ex)
            {
                lblAddMessage.Text = "Error adding course: " + ex.Message;
                lblAddMessage.ForeColor = System.Drawing.Color.Red;
                ShowAddModal(); // Re-open modal to show error
            }
        }

        #endregion

        #region GridView Events (Sort, Edit, Update, Delete)

        protected void gvCourses_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["SortExpression"] = e.SortExpression;
            string currentSortDirection = ViewState["SortDirection"] as string;
            ViewState["SortDirection"] = (currentSortDirection == "ASC" ? "DESC" : "ASC");
            BindGrid();
        }

        protected void gvCourses_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCourse")
            {
                int courseId = Convert.ToInt32(e.CommandArgument);
                manager.DeleteCourse(courseId);
                BindGrid();
            }
            else if (e.CommandName == "ShowEditModal")
            {
                int courseId = Convert.ToInt32(e.CommandArgument);
                // Get *full* course details (we need all IDs)
                Course course = manager.GetCourseDetails(courseId);
                if (course == null) return;

                // Populate the Edit Modal
                hdnEditCourseID.Value = course.CourseID.ToString();
                txtEditCourseName.Text = course.CourseName;
                txtEditDescription.Text = course.Description;
                imgEditPreview.ImageUrl = GetImagePath(course.ImagePath);
                ddlEditCourseType.SelectedValue = course.CourseType;

                // Populate Language dropdown
                BindLanguageDropdown(ddlEditLanguage);
                ddlEditLanguage.SelectedValue = course.LanguageID.ToString();

                // Populate Category dropdown *based on* the language
                BindCategoryDropdown(ddlEditCategory, course.LanguageID);
                ddlEditCategory.SelectedValue = course.CategoryID.ToString();

                // Show the modal
                ShowEditModal();
            }
        }

        // This fires when the "Language" dropdown in the EDIT modal is changed
        protected void ddlEditLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Re-populate the category dropdown
            int langId = Convert.ToInt32(ddlEditLanguage.SelectedValue);
            BindCategoryDropdown(ddlEditCategory, langId);

            // Re-open the modal
            ShowEditModal();
        }

        protected void btnUpdateCourse_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Get all values
                int courseId = Convert.ToInt32(hdnEditCourseID.Value);
                string name = txtEditCourseName.Text.Trim();
                string desc = txtEditDescription.Text.Trim();
                int langId = Convert.ToInt32(ddlEditLanguage.SelectedValue);
                int catId = Convert.ToInt32(ddlEditCategory.SelectedValue);
                string courseType = ddlEditCourseType.SelectedValue;

                // 2. Handle file upload (dbPath will be null if no file is new)
                string dbPath = HandleFileUpload(fileUploadEditCourse);

                // 3. Update course
                manager.UpdateCourse(courseId, name, desc, langId, catId, courseType, dbPath);
                BindGrid(); // Refresh grid
            }
            catch (Exception ex)
            {
                lblEditMessage.Text = "Error updating course: " + ex.Message;
                lblEditMessage.ForeColor = System.Drawing.Color.Red;
                ShowEditModal(); // Re-open modal to show error
            }
        }

        #endregion

        // Helper for displaying images
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

        // This event is no longer needed
        protected void gvCourses_RowDataBound(object sender, GridViewRowEventArgs e) { }
    }
}