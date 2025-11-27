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
                BindLanguageDropdown(ddlNewLanguage);
                BindCategoryDropdown(ddlNewCategory, 0);
                BindGrid();
            }

            // Fix for FileUpload inside UpdatePanel
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnAddCourse);
        }

        #region Utilities and Helpers

        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.DataSource = manager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();

            ListItem placeholder = new ListItem("-- Select Language --", "0");
            placeholder.Attributes["disabled"] = "disabled";
            placeholder.Selected = true;
            ddl.Items.Insert(0, placeholder);
        }

        private void BindCategoryDropdown(DropDownList ddl, int languageId)
        {
            ddl.Items.Clear();

            if (languageId > 0)
            {
                ddl.DataSource = manager.GetCategoriesByLanguage(languageId);
                ddl.DataTextField = "CategoryName";
                ddl.DataValueField = "CategoryID";
                ddl.DataBind();
            }

            string text = (languageId == 0) ? "-- Select Language First --" : "-- Select Category --";
            if (languageId > 0 && ddl.Items.Count == 0) text = "-- No Categories Found --";

            ListItem placeholder = new ListItem(text, "0");
            placeholder.Attributes["disabled"] = "disabled";
            placeholder.Selected = true;
            ddl.Items.Insert(0, placeholder);
        }

        private void BindGrid()
        {
            DataTable data = manager.GetAllCoursesForAdmin();
            if (ViewState["SortExpression"] != null)
            {
                string sortExpression = ViewState["SortExpression"].ToString();
                string sortDirection = ViewState["SortDirection"].ToString();
                data.DefaultView.Sort = $"{sortExpression} {sortDirection}";
            }
            else
            {
                data.DefaultView.Sort = "CourseID ASC";
            }

            gvCourses.DataSource = data.DefaultView;
            gvCourses.DataBind();
        }

        private string HandleFileUpload(FileUpload uploader)
        {
            if (uploader.HasFile)
            {
                string extension = Path.GetExtension(uploader.FileName);
                string fileName = $"{Guid.NewGuid()}{extension}";
                string savePath = Server.MapPath("~/Image/Course/");
                string fullPath = Path.Combine(savePath, fileName);

                Directory.CreateDirectory(savePath);
                uploader.SaveAs(fullPath);

                return $"/Image/Course/{fileName}";
            }
            return null;
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

        // NEW: Helper for Image Validation
        private bool IsValidImage(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            return (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp" || ext == ".bmp" || ext == ".svg");
        }

        #endregion

        #region Add New Course

        protected void ddlNewLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
            BindCategoryDropdown(ddlNewCategory, langId);
            ShowAddModal();
        }

        protected void btnAddCourse_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtNewCourseName.Text.Trim();
                string desc = txtNewDescription.Text.Trim();
                int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
                int catId = Convert.ToInt32(ddlNewCategory.SelectedValue);
                string courseType = ddlNewCourseType.SelectedValue;

                // --- Validation ---
                if (string.IsNullOrEmpty(name))
                {
                    ShowNotification("error", "Course Name is required.");
                    ShowAddModal();
                    return;
                }
                if (langId == 0)
                {
                    ShowNotification("warning", "Please select a Language.");
                    ShowAddModal();
                    return;
                }
                if (catId == 0)
                {
                    ShowNotification("warning", "Please select a Category.");
                    ShowAddModal();
                    return;
                }

                // --- File Upload ---
                string dbPath = null;
                try
                {
                    if (fileUploadCourse.HasFile)
                    {
                        // NEW: Validate Image Type
                        if (!IsValidImage(fileUploadCourse.FileName))
                        {
                            ShowNotification("error", "Invalid image format. Allowed: JPG, PNG, GIF, WEBP, BMP, SVG.");
                            ShowAddModal();
                            return;
                        }

                        dbPath = HandleFileUpload(fileUploadCourse);
                    }
                }
                catch (Exception ex)
                {
                    ShowNotification("error", "Image Upload Error: " + ex.Message);
                    ShowAddModal();
                    return;
                }

                // --- Save ---
                if (manager.CreateCourse(name, desc, langId, catId, courseType, dbPath))
                {
                    BindGrid();

                    // Clear Fields
                    txtNewCourseName.Text = "";
                    txtNewDescription.Text = "";
                    ddlNewLanguage.SelectedIndex = 0;
                    BindCategoryDropdown(ddlNewCategory, 0);
                    imgAddPreview.Attributes["src"] = "";
                    imgAddPreview.Style["display"] = "none";

                    HideAddModal();
                    ShowNotification("success", "Course added successfully!");
                }
                else
                {
                    ShowNotification("error", "Failed to add course.");
                    ShowAddModal();
                }
            }
            catch (Exception ex)
            {
                ShowNotification("error", "Error: " + ex.Message);
                ShowAddModal();
            }
        }

        #endregion

        #region Grid Events

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
                try
                {
                    int courseId = Convert.ToInt32(e.CommandArgument);
                    manager.DeactivatedCourse(courseId);
                    BindGrid();
                    ShowNotification("warning", "Course deactivated.");
                }
                catch (Exception ex)
                {
                    ShowNotification("error", "Error deactivating: " + ex.Message);
                }
            }
            else if (e.CommandName == "EditCourse")
            {
                int courseId = Convert.ToInt32(e.CommandArgument);
                Session["Admin_EditCourseID"] = courseId;
                Response.Redirect("EditCourse.aspx");
            }
        }

        protected void gvCourses_RowDataBound(object sender, GridViewRowEventArgs e) { }

        #endregion
    }
}