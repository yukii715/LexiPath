using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Linq;
using System.Web.UI.HtmlControls;

namespace LexiPath.Admin
{
    public partial class EditCourse : AdminBasePage
    {
        CourseManager manager = new CourseManager();

        private int CourseID
        {
            get { return Convert.ToInt32(Session["Admin_EditCourseID"] ?? 0); }
        }

        protected int ExpandedSequence
        {
            get { return (int)(ViewState["ExpandedSequence"] ?? 0); }
            set { ViewState["ExpandedSequence"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CourseID == 0) { Response.Redirect("ManageCourses.aspx"); return; }

                hdnCourseID.Value = CourseID.ToString();
                BindDetails();
                BindItems();
            }
            else
            {
                RestorePanelVisibility();
            }
        }

        #region Helpers for ASPX

        protected string GetCollapseClass(object sequenceOrder)
        {
            if (sequenceOrder != null && (int)sequenceOrder == ExpandedSequence)
            {
                return "collapse show";
            }
            return "collapse";
        }

        protected string GetIconClass(object sequenceOrder)
        {
            if (sequenceOrder != null && (int)sequenceOrder == ExpandedSequence)
            {
                return "bi bi-chevron-up";
            }
            return "bi bi-chevron-down";
        }

        #endregion

        #region Course Details
        private void BindDetails()
        {
            Course c = manager.GetCourseDetails(CourseID);
            if (c == null) return;
            txtName.Text = c.CourseName;
            txtDesc.Text = c.Description;
            imgPreview.ImageUrl = GetImagePath(c.ImagePath);

            BindLanguageDropdown(ddlLanguage);
            if (ddlLanguage.Items.FindByValue(c.LanguageID.ToString()) != null) ddlLanguage.SelectedValue = c.LanguageID.ToString();

            BindCategory(c.LanguageID);
            if (ddlCategory.Items.FindByValue(c.CategoryID.ToString()) != null) ddlCategory.SelectedValue = c.CategoryID.ToString();

            ddlType.SelectedValue = c.CourseType;

            btnAddVocab.Visible = (c.CourseType == "Mixed" || c.CourseType == "Vocabulary");
            btnAddPhrase.Visible = (c.CourseType == "Mixed" || c.CourseType == "Phrase");

            btnActivate.Visible = !c.isActivated;
            btnDeactivate.Visible = c.isActivated;
        }

        private void RestorePanelVisibility()
        {
            foreach (RepeaterItem item in rptItems.Items)
            {
                HiddenField hdnType = (HiddenField)item.FindControl("hdnItemType");
                if (hdnType != null)
                {
                    Panel pVocab = (Panel)item.FindControl("pnlVocabFields");
                    Panel pPhrase = (Panel)item.FindControl("pnlPhraseFields");

                    if (hdnType.Value == "Vocabulary" && pVocab != null) pVocab.Visible = true;
                    else if (hdnType.Value == "Phrase" && pPhrase != null) pPhrase.Visible = true;
                }
            }
        }

        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.DataSource = manager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();
            ListItem placeholder = new ListItem("-- Select Language --", "0");
            placeholder.Attributes["disabled"] = "disabled";
            ddl.Items.Insert(0, placeholder);
        }

        private void BindCategory(int langId)
        {
            ddlCategory.Items.Clear();
            if (langId > 0)
            {
                var categories = manager.GetCategoriesByLanguage(langId);
                ddlCategory.DataSource = categories;
                ddlCategory.DataTextField = "CategoryName";
                ddlCategory.DataValueField = "CategoryID";
                ddlCategory.DataBind();
            }
            string text = (langId == 0) ? "-- Select Language First --" : "-- Select Category --";
            if (langId > 0 && ddlCategory.Items.Count == 0) text = "-- No Categories Found --";
            ListItem placeholder = new ListItem(text, "0");
            placeholder.Attributes["disabled"] = "disabled";
            placeholder.Selected = true;
            ddlCategory.Items.Insert(0, placeholder);
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCategory(Convert.ToInt32(ddlLanguage.SelectedValue));
        }

        protected void btnSaveDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtName.Text.Trim())) { ShowNotification("error", "Course Name is required."); return; }
                if (ddlLanguage.SelectedValue == "0") { ShowNotification("warning", "Please select a Language."); return; }
                if (ddlCategory.SelectedValue == "0") { ShowNotification("warning", "Please select a Category."); return; }

                Course currentCourse = manager.GetCourseDetails(CourseID);
                string newName = txtName.Text.Trim();
                string newDesc = txtDesc.Text.Trim();
                int newLang = int.Parse(ddlLanguage.SelectedValue);
                int newCat = int.Parse(ddlCategory.SelectedValue);
                bool imageChanged = fileUploadImage.HasFile;

                bool isChanged = !currentCourse.CourseName.Equals(newName) ||
                                 !currentCourse.Description.Equals(newDesc) ||
                                 currentCourse.LanguageID != newLang ||
                                 currentCourse.CategoryID != newCat ||
                                 imageChanged;

                if (!isChanged) { ShowNotification("info", "No changes were made."); return; }

                string img = null;
                if (fileUploadImage.HasFile)
                {
                    if (!IsValidImage(fileUploadImage.FileName)) { ShowNotification("error", "Invalid image format."); return; }
                    try { img = HandleFileUpload(fileUploadImage); }
                    catch (Exception ex) { ShowNotification("error", "Image Upload Error: " + ex.Message); return; }
                }

                manager.UpdateCourse(CourseID, newName, newDesc, newLang, newCat, ddlType.SelectedValue, img);

                ExpandedSequence = 0;

                BindDetails();
                BindItems();
                upCourseDetails.Update();
                upItems.Update();
                ShowNotification("success", "Course details saved successfully!");
            }
            catch (Exception ex) { ShowNotification("error", "Error: " + ex.Message); }
        }

        protected void btnActivate_Click(object sender, EventArgs e)
        {
            try
            {
                var items = manager.GetCourseContent(CourseID);
                if (items.Count < 5) { ShowNotification("error", "Course cannot be activated. You need at least 5 learning items."); return; }
                manager.ActivateCourse(CourseID);
                BindDetails();
                BindItems();
                ShowNotification("success", "Course activated successfully!");
            }
            catch (Exception ex) { ShowNotification("error", "Error activating: " + ex.Message); }
        }

        protected void btnDeactivate_Click(object sender, EventArgs e)
        {
            try
            {
                manager.DeactivatedCourse(CourseID);
                BindDetails();
                ShowNotification("warning", "Course has been deactivated.");
            }
            catch (Exception ex) { ShowNotification("error", "Error deactivating: " + ex.Message); }
        }

        #endregion

        #region Item Management

        private void BindItems()
        {
            List<LearningItem> items = manager.GetCourseContent(CourseID);
            rptItems.DataSource = items;
            rptItems.DataBind();
        }

        protected void rptItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                LearningItem item = (LearningItem)e.Item.DataItem;
                Panel pVocab = (Panel)e.Item.FindControl("pnlVocabFields");
                Panel pPhrase = (Panel)e.Item.FindControl("pnlPhraseFields");
                Button btnSave = (Button)e.Item.FindControl("btnSave");

                if (item.ItemType == "Vocabulary")
                {
                    pVocab.Visible = true;
                }
                else
                {
                    pPhrase.Visible = true;
                    Repeater rptDetails = (Repeater)e.Item.FindControl("rptPhraseDetails");
                    rptDetails.DataSource = manager.GetPhraseDetails(item.ItemID);
                    rptDetails.DataBind();
                }
                ScriptManager.GetCurrent(this).RegisterPostBackControl(btnSave);
            }
        }

        protected void rptItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);
            string type = ((HiddenField)e.Item.FindControl("hdnItemType")).Value;
            int currentSeq = Convert.ToInt32(((HiddenField)e.Item.FindControl("hdnSequence")).Value);

            if (e.CommandName == "DeleteItem")
            {
                manager.DeleteLearningItem(id, type, CourseID);
                ExpandedSequence = 0;
                BindItems();
                ShowNotification("warning", "Item deleted.");
            }
            else if (e.CommandName == "SaveItem")
            {
                ExpandedSequence = currentSeq;
                SaveSingleItem(e.Item, id, type);
            }
            else if (e.CommandName == "AddDetail")
            {
                // FIX: 1. Save the MAIN Phrase Text & Meaning first!
                string phraseText = ((TextBox)e.Item.FindControl("txtPhrase")).Text.Trim();
                string phraseMeaning = ((TextBox)e.Item.FindControl("txtPhraseMeaning")).Text.Trim();
                manager.UpdatePhrase(id, phraseText, phraseMeaning);

                // 2. Save existing details (the rows below)
                SavePhraseState(e.Item, id);

                // 3. Now safe to add new row
                manager.AddPhraseDetail(id, "", "");

                ExpandedSequence = currentSeq;
                BindItems();
            }
            else if (e.CommandName == "MoveUp")
            {
                manager.ReorderLearningItem(CourseID, id, type, "UP");
                ExpandedSequence = 0;
                BindItems();
            }
            else if (e.CommandName == "MoveDown")
            {
                manager.ReorderLearningItem(CourseID, id, type, "DOWN");
                ExpandedSequence = 0;
                BindItems();
            }
        }

        private void SaveSingleItem(RepeaterItem item, int id, string type)
        {
            var currentItems = manager.GetCourseContent(CourseID);
            var currentItem = currentItems.FirstOrDefault(i => i.ItemID == id && i.ItemType == type);
            if (currentItem == null) return;

            bool anyChange = false;

            if (type == "Vocabulary")
            {
                string newText = ((TextBox)item.FindControl("txtVocab")).Text.Trim();
                string newMeaning = ((TextBox)item.FindControl("txtVocabMeaning")).Text.Trim();
                FileUpload fu = (FileUpload)item.FindControl("fileVocabImg");
                bool hasNewFile = fu.HasFile;

                if (hasNewFile && !IsValidImage(fu.FileName))
                {
                    ShowNotification("error", "Invalid image format."); return;
                }

                if (!currentItem.VocabText.Equals(newText) || !currentItem.VocabMeaning.Equals(newMeaning) || hasNewFile)
                {
                    string img = hasNewFile ? HandleFileUpload(fu) : null;
                    manager.UpdateVocabulary(id, newText, newMeaning, img);
                    anyChange = true;
                }
            }
            else // Phrase
            {
                string newText = ((TextBox)item.FindControl("txtPhrase")).Text.Trim();
                string newMeaning = ((TextBox)item.FindControl("txtPhraseMeaning")).Text.Trim();

                if (!currentItem.PhraseText.Equals(newText) || !currentItem.PhraseMeaning.Equals(newMeaning))
                {
                    manager.UpdatePhrase(id, newText, newMeaning);
                    anyChange = true;
                }
                if (SavePhraseState(item, id)) anyChange = true;
            }

            if (anyChange) { BindItems(); ShowNotification("success", "Saved successfully."); }
            else { ShowNotification("info", "No changes were made."); }
        }

        private bool SavePhraseState(RepeaterItem parentItem, int phraseId)
        {
            bool changeDetected = false;
            Repeater rptDetails = (Repeater)parentItem.FindControl("rptPhraseDetails");
            var dbDetails = manager.GetPhraseDetails(phraseId);

            foreach (RepeaterItem detailItem in rptDetails.Items)
            {
                int detailId = Convert.ToInt32(((HiddenField)detailItem.FindControl("hdnDetailID")).Value);
                string newDType = ((TextBox)detailItem.FindControl("txtDetailType")).Text.Trim();
                string newDContent = ((TextBox)detailItem.FindControl("txtDetailContent")).Text.Trim();

                var original = dbDetails.FirstOrDefault(d => d.PhraseDetailID == detailId);
                if (original != null)
                {
                    if (!original.DetailType.Equals(newDType) || !original.Content.Equals(newDContent))
                    {
                        manager.UpdatePhraseDetail(detailId, newDType, newDContent);
                        changeDetected = true;
                    }
                }
            }
            return changeDetected;
        }

        protected void rptPhraseDetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteDetail")
            {
                Repeater rptDetails = (Repeater)source;
                RepeaterItem phraseItem = (RepeaterItem)rptDetails.NamingContainer;

                HiddenField hdnItemId = (HiddenField)phraseItem.FindControl("hdnItemID");
                HiddenField hdnSeq = (HiddenField)phraseItem.FindControl("hdnSequence");

                int phraseId = Convert.ToInt32(hdnItemId.Value);
                int seq = Convert.ToInt32(hdnSeq.Value);

                // FIX: 1. Save the MAIN Phrase Text & Meaning first!
                string phraseText = ((TextBox)phraseItem.FindControl("txtPhrase")).Text.Trim();
                string phraseMeaning = ((TextBox)phraseItem.FindControl("txtPhraseMeaning")).Text.Trim();
                manager.UpdatePhrase(phraseId, phraseText, phraseMeaning);

                // 2. Save state of OTHER rows before deleting this one
                SavePhraseState(phraseItem, phraseId);

                // 3. Delete
                int detailId = Convert.ToInt32(e.CommandArgument);
                manager.DeletePhraseDetail(detailId);

                ExpandedSequence = seq;
                BindItems();
            }
        }

        protected void btnAddVocab_Click(object sender, EventArgs e)
        {
            int nextSeq = rptItems.Items.Count + 1;
            manager.CreateLearningItem(CourseID, "Vocabulary", nextSeq);

            ExpandedSequence = nextSeq;
            BindItems();
            ShowNotification("success", "New Vocabulary item added.");
        }

        protected void btnAddPhrase_Click(object sender, EventArgs e)
        {
            int nextSeq = rptItems.Items.Count + 1;
            manager.CreateLearningItem(CourseID, "Phrase", nextSeq);

            ExpandedSequence = nextSeq;
            BindItems();
            ShowNotification("success", "New Phrase item added.");
        }

        #endregion

        #region Helpers

        private string HandleFileUpload(FileUpload uploader)
        {
            if (uploader.HasFile)
            {
                string ext = Path.GetExtension(uploader.FileName);
                string fname = $"{Guid.NewGuid()}{ext}";
                string path = Server.MapPath("~/Image/Course/");
                Directory.CreateDirectory(path);
                uploader.SaveAs(Path.Combine(path, fname));
                return $"/Image/Course/{fname}";
            }
            return null;
        }

        public string GetImagePath(object img)
        {
            return (img != null && !string.IsNullOrEmpty(img.ToString())) ? img.ToString() : "/Image/System/placeholder.png";
        }

        private void ShowNotification(string type, string message)
        {
            string safeMessage = message.Replace("'", "\\'").Replace("\r", "").Replace("\n", "");
            string script = $"showNotification('{type}', '{safeMessage}');";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "SweetAlert_" + Guid.NewGuid(), script, true);
        }

        private bool IsValidImage(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            return (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".webp" || ext == ".bmp" || ext == ".svg");
        }

        #endregion
    }
}