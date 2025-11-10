using LexiPath.Data;
using System;
using System.Data;
using System.IO;    // Required for FileUpload
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath.Admin
{
    public partial class ManageCategories : AdminBasePage
    {
        private CourseManager manager = new CourseManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate the "Add New" dropdown
                BindLanguageDropdown(ddlNewLanguage);
                // Bind the main grid
                BindGrid();
            }
        }

        /**
         * Fills a language dropdown
         */
        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.DataSource = manager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();
        }

        /**
         * Fills the main GridView with all categories
         */
        private void BindGrid()
        {
            DataTable data = manager.GetAllCategoriesForAdmin();

            // --- UPDATED: Sorting Logic ---
            // Check if a sort expression is stored in ViewState
            if (ViewState["SortExpression"] != null)
            {
                // User has clicked a column, apply their chosen sort
                string sortExpression = ViewState["SortExpression"].ToString();
                string sortDirection = ViewState["SortDirection"].ToString();

                data.DefaultView.Sort = $"{sortExpression} {sortDirection}";
            }
            else
            {
                // --- NEW: DEFAULT SORT ---
                // This is the first page load (or after an edit).
                // Set the default sort to "CategoryID ASC" (ascending).
                data.DefaultView.Sort = "CategoryID ASC";
            }
            // --- END OF UPDATE ---

            gvCategories.DataSource = data.DefaultView;
            gvCategories.DataBind();
        }

        // --- Helper function to re-open Add modal on error
        private void ShowAddModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showAddModal",
                "var myModalEl = document.getElementById('addCategoryModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.show();", true);
        }

        // --- Helper function to re-open Edit modal on error
        private void ShowEditModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showEditModal",
                "var myModalEl = document.getElementById('editCategoryModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.show();", true);
        }

        // --- 1. ADD NEW CATEGORY ---

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            string catName = txtNewCategoryName.Text.Trim();
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
            string dbPath = null;

            if (string.IsNullOrEmpty(catName))
            {
                lblAddMessage.Text = "Category Name is required.";
                lblAddMessage.ForeColor = System.Drawing.Color.Red;
                ShowAddModal(); // Re-open modal
                return;
            }

            if (fileUploadCategory.HasFile)
            {
                try
                {
                    string extension = Path.GetExtension(fileUploadCategory.FileName);
                    string fileName = $"{Guid.NewGuid()}{extension}";
                    string savePath = Server.MapPath("~/Image/Category/");
                    string fullPath = Path.Combine(savePath, fileName);
                    Directory.CreateDirectory(savePath);
                    fileUploadCategory.SaveAs(fullPath);
                    dbPath = $"/Image/Category/{fileName}";
                }
                catch (Exception ex)
                {
                    lblAddMessage.Text = "File upload failed: " + ex.Message;
                    lblAddMessage.ForeColor = System.Drawing.Color.Red;
                    ShowAddModal(); // Re-open modal
                    return;
                }
            }

            if (manager.CreateCategory(catName, langId, dbPath))
            {
                BindGrid();
                txtNewCategoryName.Text = string.Empty;
                ddlNewLanguage.SelectedIndex = 0;
                // On success, page reloads and modal stays closed.
            }
            else
            {
                lblAddMessage.Text = "Error adding category. It might already exist.";
                lblAddMessage.ForeColor = System.Drawing.Color.Red;
                ShowAddModal(); // Re-open modal
            }
        }

        // --- 2. SORTING (Unchanged) ---

        protected void gvCategories_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["SortExpression"] = e.SortExpression;
            string currentSortDirection = ViewState["SortDirection"] as string;
            ViewState["SortDirection"] = (currentSortDirection == "ASC" ? "DESC" : "ASC");
            BindGrid();
        }

        // --- 3. EDIT / UPDATE / DELETE ---

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCategory")
            {
                int categoryId = Convert.ToInt32(e.CommandArgument);
                manager.DeleteCategory(categoryId);
                BindGrid();
            }
            else if (e.CommandName == "ShowEditModal")
            {
                int categoryId = Convert.ToInt32(e.CommandArgument);
                Category cat = manager.GetCategoryDetails(categoryId);
                if (cat == null) return;

                // Populate the Edit Modal
                hdnEditCategoryID.Value = cat.CategoryID.ToString();
                txtEditCategoryName.Text = cat.CategoryName;
                imgEditPreview.ImageUrl = GetImagePath(cat.ImagePath); // Set preview image

                // Populate the dropdown and select the correct language
                BindLanguageDropdown(ddlEditLanguage);
                ddlEditLanguage.SelectedValue = cat.LanguageID.ToString();

                // Show the modal
                ShowEditModal();
            }
        }

        /**
         * UPDATED: This now handles the Edit FileUpload control
         */
        protected void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            int categoryId = Convert.ToInt32(hdnEditCategoryID.Value);
            string newName = txtEditCategoryName.Text.Trim();
            int newLangId = Convert.ToInt32(ddlEditLanguage.SelectedValue);
            string dbPath = null; // Default to NULL (don't change the image)

            // --- New Image Upload Logic ---
            if (fileUploadEditCategory.HasFile)
            {
                try
                {
                    string extension = Path.GetExtension(fileUploadEditCategory.FileName);
                    string fileName = $"{Guid.NewGuid()}{extension}";
                    string savePath = Server.MapPath("~/Image/Category/");
                    string fullPath = Path.Combine(savePath, fileName);

                    Directory.CreateDirectory(savePath);
                    fileUploadEditCategory.SaveAs(fullPath);
                    dbPath = $"/Image/Category/{fileName}";
                }
                catch (Exception ex)
                {
                    lblEditMessage.Text = "File upload failed: " + ex.Message;
                    lblEditMessage.ForeColor = System.Drawing.Color.Red;
                    ShowEditModal(); // Re-open modal to show error
                    return;
                }
            }

            // Call the manager to update the DB
            // dbPath will be NULL if no new file was uploaded, and our
            // manager logic knows to ignore it.
            if (manager.UpdateCategory(categoryId, newName, newLangId, dbPath))
            {
                BindGrid(); // Refresh the grid
                // On success, page reloads and modal stays closed.
            }
            else
            {
                lblEditMessage.Text = "Error updating category.";
                lblEditMessage.ForeColor = System.Drawing.Color.Red;
                ShowEditModal(); // Re-open modal to show error
            }
        }

        /**
         * This event fires for EVERY row when the grid is bound.
         * We use it to populate the dropdown ONLY when a row is in Edit mode.
         */
        protected void gvCategories_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // This event is no longer needed because we moved editing to the modal
        }

        // --- HELPER FUNCTION ---

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