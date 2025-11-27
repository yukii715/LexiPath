using LexiPath.Data;
using System;
using System.Data;
using System.IO;
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
                BindLanguageDropdown(ddlNewLanguage);
                BindGrid();
            }

            // Fix for FileUpload inside UpdatePanel
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnAddCategory);
            ScriptManager.GetCurrent(this).RegisterPostBackControl(btnUpdateCategory);
        }

        #region Binding Methods

        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.DataSource = manager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();
        }

        private void BindGrid()
        {
            DataTable data = manager.GetAllCategoriesForAdmin();

            if (ViewState["SortExpression"] != null)
            {
                string sortExpression = ViewState["SortExpression"].ToString();
                string sortDirection = ViewState["SortDirection"].ToString();
                data.DefaultView.Sort = $"{sortExpression} {sortDirection}";
            }
            else
            {
                data.DefaultView.Sort = "CategoryID ASC";
            }

            gvCategories.DataSource = data.DefaultView;
            gvCategories.DataBind();
        }

        #endregion

        #region Add Category

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            string catName = txtNewCategoryName.Text.Trim();
            int langId = Convert.ToInt32(ddlNewLanguage.SelectedValue);
            string dbPath = null;

            if (string.IsNullOrEmpty(catName))
            {
                ShowNotification("error", "Category Name is required.");
                ShowAddModal();
                return;
            }

            if (fileUploadCategory.HasFile)
            {
                // NEW: Image Validation
                if (!IsValidImage(fileUploadCategory.FileName))
                {
                    ShowNotification("error", "Invalid image format. Allowed: JPG, PNG, GIF, WEBP, BMP, SVG.");
                    ShowAddModal();
                    return;
                }

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
                    ShowNotification("error", "File upload failed: " + ex.Message);
                    ShowAddModal();
                    return;
                }
            }

            if (manager.CreateCategory(catName, langId, dbPath))
            {
                BindGrid();
                upGrid.Update(); // Refresh the grid panel

                // Reset fields
                txtNewCategoryName.Text = string.Empty;
                ddlNewLanguage.SelectedIndex = 0;
                imgAddPreview.Attributes["src"] = ""; // Clear preview
                imgAddPreview.Style["display"] = "none";

                ShowNotification("success", "Category added successfully!");
            }
            else
            {
                ShowNotification("error", "Error adding category.");
                ShowAddModal();
            }
        }

        #endregion

        #region Edit / Update / Activate / Deactivate

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ShowEditModal")
            {
                int categoryId = Convert.ToInt32(e.CommandArgument);
                Category cat = manager.GetCategoryDetails(categoryId);
                if (cat == null) return;

                hdnEditCategoryID.Value = cat.CategoryID.ToString();
                txtEditCategoryName.Text = cat.CategoryName;
                imgEditPreview.ImageUrl = GetImagePath(cat.ImagePath);
                imgEditPreview.Style["display"] = "block"; // Force show image

                BindLanguageDropdown(ddlEditLanguage);
                ddlEditLanguage.SelectedValue = cat.LanguageID.ToString();

                // --- TOGGLE BUTTON VISIBILITY ---
                btnActivateCategory.Visible = !cat.isActivated;
                btnDeactivateCategory.Visible = cat.isActivated;
                // --------------------------------

                upEditCategory.Update();

                ShowEditModal();
            }
        }

        protected void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            try
            {
                int categoryId = Convert.ToInt32(hdnEditCategoryID.Value);
                string newName = txtEditCategoryName.Text.Trim();
                int newLangId = Convert.ToInt32(ddlEditLanguage.SelectedValue);
                string dbPath = null;

                if (string.IsNullOrEmpty(newName))
                {
                    ShowNotification("error", "Category Name cannot be empty.");
                    ShowEditModal();
                    return;
                }

                // CHECK FOR CHANGES
                Category currentCat = manager.GetCategoryDetails(categoryId);
                bool isImageChanged = fileUploadEditCategory.HasFile;
                bool isNameChanged = !currentCat.CategoryName.Equals(newName, StringComparison.OrdinalIgnoreCase);
                bool isLangChanged = currentCat.LanguageID != newLangId;

                if (!isNameChanged && !isLangChanged && !isImageChanged)
                {
                    ShowNotification("info", "No changes were made.");
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "hideModal", "hideModal('editCategoryModal');", true);
                    return;
                }

                if (fileUploadEditCategory.HasFile)
                {
                    // NEW: Image Validation
                    if (!IsValidImage(fileUploadEditCategory.FileName))
                    {
                        ShowNotification("error", "Invalid image format. Allowed: JPG, PNG, GIF, WEBP, BMP, SVG.");
                        ShowEditModal();
                        return;
                    }

                    string extension = Path.GetExtension(fileUploadEditCategory.FileName);
                    string fileName = $"{Guid.NewGuid()}{extension}";
                    string savePath = Server.MapPath("~/Image/Category/");
                    string fullPath = Path.Combine(savePath, fileName);

                    Directory.CreateDirectory(savePath);
                    fileUploadEditCategory.SaveAs(fullPath);
                    dbPath = $"/Image/Category/{fileName}";
                }

                if (manager.UpdateCategory(categoryId, newName, newLangId, dbPath))
                {
                    BindGrid();
                    upGrid.Update();
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "hideModal", "hideModal('editCategoryModal');", true);
                    ShowNotification("success", "Category updated successfully!");
                }
                else
                {
                    ShowNotification("error", "Error updating category.");
                    ShowEditModal();
                }
            }
            catch (Exception ex)
            {
                ShowNotification("error", "Error: " + ex.Message);
                ShowEditModal();
            }
        }

        protected void btnActivateCategory_Click(object sender, EventArgs e)
        {
            try
            {
                int categoryId = Convert.ToInt32(hdnEditCategoryID.Value);
                manager.ActivateCategory(categoryId);

                BindGrid();
                upGrid.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "hideModal", "hideModal('editCategoryModal');", true);
                ShowNotification("success", "Category activated successfully!");
            }
            catch (Exception ex)
            {
                ShowNotification("error", "Error activating: " + ex.Message);
                ShowEditModal();
            }
        }

        protected void btnDeactivateCategory_Click(object sender, EventArgs e)
        {
            try
            {
                int categoryId = Convert.ToInt32(hdnEditCategoryID.Value);
                manager.DeactivatedCategory(categoryId);

                BindGrid();
                upGrid.Update();
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "hideModal", "hideModal('editCategoryModal');", true);
                ShowNotification("warning", "Category has been deactivated.");
            }
            catch (Exception ex)
            {
                ShowNotification("error", "Error deactivating: " + ex.Message);
                ShowEditModal();
            }
        }

        #endregion

        #region Helpers

        private void ShowAddModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showAddModal",
                "var myModalEl = document.getElementById('addCategoryModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.show();", true);
        }

        private void ShowEditModal()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "showEditModal",
                "var myModalEl = document.getElementById('editCategoryModal'); var myModal = new bootstrap.Modal(myModalEl); myModal.show();", true);
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

        // NEW: Helper for Image Validation
        private bool IsValidImage(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            return (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp" || ext == ".bmp" || ext == ".svg");
        }

        protected void gvCategories_Sorting(object sender, GridViewSortEventArgs e)
        {
            ViewState["SortExpression"] = e.SortExpression;
            string currentSortDirection = ViewState["SortDirection"] as string;
            ViewState["SortDirection"] = (currentSortDirection == "ASC" ? "DESC" : "ASC");
            BindGrid();
        }

        protected void gvCategories_RowDataBound(object sender, GridViewRowEventArgs e) { }

        #endregion
    }
}