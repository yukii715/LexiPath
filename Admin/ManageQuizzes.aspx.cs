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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate the "Add New" modal's lists
                BindLanguageDropdown(ddlNewLanguage);
                BindCourseList(lstNewCourses);
                BindTagList(lstNewTags);

                // Bind the main grid
                BindGrid();
            }
        }

        #region Utilities and Helpers

        // Fills a language dropdown
        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.DataSource = courseManager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();
        }

        // Fills a course listbox
        private void BindCourseList(ListBox lst)
        {
            lst.DataSource = manager.GetAllCourses();
            lst.DataTextField = "CourseName";
            lst.DataValueField = "CourseID";
            lst.DataBind();
        }

        // Fills a tag listbox
        private void BindTagList(ListBox lst)
        {
            lst.DataSource = manager.GetAllTags();
            lst.DataTextField = "TagName";
            lst.DataValueField = "TagID";
            lst.DataBind();
        }

        // Fills the main GridView
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

        // Handles file uploads
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

        // Helper to show modal on error
        private void ShowAddModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showAddModal",
                "var myModalEl = document.getElementById('addQuizModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.Show();", true);
        }

        // ShowEditModal() is no longer needed

        // Helper to get selected int[] from a ListBox
        private int[] GetSelectedIds(ListBox lst)
        {
            return lst.GetSelectedIndices().Select(i => Convert.ToInt32(lst.Items[i].Value)).ToArray();
        }

        #endregion

        #region Add New Quiz (Modal)

        protected void btnAddQuiz_Click(object sender, EventArgs e)
        {
            try
            {
                string title = txtNewTitle.Text.Trim();
                string desc = txtNewDescription.Text.Trim();
                int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
                bool isPractice = chkNewIsPractice.Checked;
                string dbPath = HandleFileUpload(fileUploadNewImage, "Quiz");

                int[] courseIds = GetSelectedIds(lstNewCourses);
                int[] tagIds = GetSelectedIds(lstNewTags);

                if (manager.CreateQuiz(title, desc, langId, isPractice, dbPath, courseIds, tagIds))
                {
                    BindGrid();
                }
                else
                {
                    lblAddMessage.Text = "Error adding quiz.";
                    lblAddMessage.ForeColor = System.Drawing.Color.Red;
                    ShowAddModal();
                }
            }
            catch (Exception ex)
            {
                lblAddMessage.Text = "Error: " + ex.Message;
                lblAddMessage.ForeColor = System.Drawing.Color.Red;
                ShowAddModal();
            }
        }

        #endregion

        #region GridView Events (Sort, Delete)

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
                int quizId = Convert.ToInt32(e.CommandArgument);
                manager.DeleteQuiz(quizId);
                BindGrid();
            }
            // ADD THIS NEW "else if" BLOCK
            else if (e.CommandName == "EditQuiz")
            {
                // 1. Get the QuizID from the button
                int quizId = Convert.ToInt32(e.CommandArgument);

                // 2. Store it securely in the Session
                Session["Admin_EditQuizID"] = quizId;

                // 3. Redirect to the edit page (no QueryString!)
                Response.Redirect("EditQuiz.aspx");
            }
        }

        // btnUpdateQuiz_Click() and related methods have been removed

        #endregion

        // Helper for displaying images (still used by Add modal, so keep)
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