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
    public partial class EditQuiz : AdminBasePage
    {
        private QuizManager manager = new QuizManager();
        private CourseManager courseManager = new CourseManager();

        private int QuizID
        {
            get { return Convert.ToInt32(Session["Admin_EditQuizID"] ?? 0); }
        }

        protected List<int> SelectedCourseIDs
        {
            get { return (List<int>)(ViewState["SelectedCourseIDs"] ?? new List<int>()); }
            set { ViewState["SelectedCourseIDs"] = value; }
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
                if (this.QuizID == 0) { Response.Redirect("ManageQuizzes.aspx"); return; }
                hdnQuizID.Value = this.QuizID.ToString();

                BindLanguageDropdown();
                BindDropdownWithPlaceholder(ddlEditCategory, null, "-- Select Language First --");
                BindDropdownWithPlaceholder(ddlPracticeCategory, null, "-- Select Language First --");
                BindDropdownWithPlaceholder(ddlPracticeCourse, null, "-- Select Category First --");

                BindQuizDetails();
                BindAllQuestions();
            }
        }

        #region Data Binding & Quiz Details
        private void BindLanguageDropdown() { BindDropdownWithPlaceholder(ddlEditLanguage, courseManager.GetAllLanguages(), "-- Select Language --", "LanguageName", "LanguageID"); }
        private void BindDropdownWithPlaceholder(DropDownList ddl, object dataSource, string defaultText, string textField = "", string valueField = "") { ddl.Items.Clear(); ddl.ClearSelection(); if (dataSource != null) { ddl.DataSource = dataSource; if (!string.IsNullOrEmpty(textField)) { ddl.DataTextField = textField; ddl.DataValueField = valueField; } ddl.DataBind(); } if (ddl.Items.Count == 0 && dataSource != null) { ddl.Items.Add(new ListItem(defaultText.Replace("Select", "No").Replace("First", "Found"), "0") { Selected = true, Enabled = false }); } else { ddl.Items.Insert(0, new ListItem(defaultText, "0") { Selected = true, Enabled = false }); } }
        private void RefreshAvailableCoursesUI() { int catId = Convert.ToInt32(ddlEditCategory.SelectedValue); int langId = Convert.ToInt32(ddlEditLanguage.SelectedValue); gvAvailableCourses.DataSource = null; if (langId > 0) { List<Course> courses = (catId > 0) ? courseManager.GetCoursesByCategoryID(catId, "All", null, langId) : courseManager.GetAllCourses("All", null, langId); gvAvailableCourses.DataSource = courses.Where(c => !SelectedCourseIDs.Contains(c.CourseID)).ToList(); } gvAvailableCourses.DataBind(); }
        private void BindQuizDetails() { Quiz quiz = manager.GetQuizDetails(QuizID); if (quiz == null) return; txtEditTitle.Text = quiz.Title; txtEditDescription.Text = quiz.Description; imgEditPreview.ImageUrl = GetImagePath(quiz.ImagePath); chkEditIsPractice.Checked = quiz.IsPractice; if (ddlEditLanguage.Items.FindByValue(quiz.LanguageID.ToString()) != null) { ddlEditLanguage.SelectedValue = quiz.LanguageID.ToString(); var cats = courseManager.GetCategoriesByLanguage(quiz.LanguageID); BindDropdownWithPlaceholder(ddlEditCategory, cats, "-- Filter by Category --", "CategoryName", "CategoryID"); BindDropdownWithPlaceholder(ddlPracticeCategory, cats, "-- Select Category --", "CategoryName", "CategoryID"); } var ids = quiz.RelatedCourseIDs?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() ?? new List<int>(); if (quiz.IsPractice && ids.Count > 0) { int pId = ids[0]; Course c = courseManager.GetCourseDetails(pId); if (c != null && ddlPracticeCategory.Items.FindByValue(c.CategoryID.ToString()) != null) { ddlPracticeCategory.SelectedValue = c.CategoryID.ToString(); var courses = courseManager.GetCoursesByCategoryID(c.CategoryID, "All", null, quiz.LanguageID); BindDropdownWithPlaceholder(ddlPracticeCourse, courses, "-- Select Course --", "CourseName", "CourseID"); if (ddlPracticeCourse.Items.FindByValue(pId.ToString()) != null) ddlPracticeCourse.SelectedValue = pId.ToString(); } } else { BindDropdownWithPlaceholder(ddlPracticeCourse, null, "-- Select Category First --"); } SelectedCourseIDs = ids; RefreshSelectedCoursesUI(); RefreshAvailableCoursesUI(); TogglePanels(); btnActivateQuiz.Visible = !quiz.isActivated; btnDeactivateQuiz.Visible = quiz.isActivated; }
        protected void ddlEditLanguage_SelectedIndexChanged(object s, EventArgs e) { int lid = int.Parse(ddlEditLanguage.SelectedValue); var cats = (lid > 0) ? courseManager.GetCategoriesByLanguage(lid) : null; BindDropdownWithPlaceholder(ddlEditCategory, cats, "-- Filter --", "CategoryName", "CategoryID"); BindDropdownWithPlaceholder(ddlPracticeCategory, cats, "-- Select --", "CategoryName", "CategoryID"); RefreshAvailableCoursesUI(); BindDropdownWithPlaceholder(ddlPracticeCourse, null, "-- Select Category First --"); }
        protected void ddlEditCategory_SelectedIndexChanged(object s, EventArgs e) { RefreshAvailableCoursesUI(); }
        protected void ddlPracticeCategory_SelectedIndexChanged(object s, EventArgs e) { int cid = int.Parse(ddlPracticeCategory.SelectedValue); int lid = int.Parse(ddlEditLanguage.SelectedValue); if (cid > 0 && lid > 0) BindDropdownWithPlaceholder(ddlPracticeCourse, courseManager.GetCoursesByCategoryID(cid, "All", null, lid), "-- Select --", "CourseName", "CourseID"); else BindDropdownWithPlaceholder(ddlPracticeCourse, null, "-- Select Category First --"); }
        protected void chkEditIsPractice_CheckedChanged(object s, EventArgs e) { TogglePanels(); }
        private void TogglePanels() { bool isP = chkEditIsPractice.Checked; pnlPracticeCourse.Visible = isP; pnlRelatedCourses.Visible = !isP; }
        private void RefreshSelectedCoursesUI() { int lid = Convert.ToInt32(ddlEditLanguage.SelectedValue); if (lid == 0) lid = 1; var all = courseManager.GetAllCourses("All", null, lid); rptSelectedCourses.DataSource = all.Where(c => SelectedCourseIDs.Contains(c.CourseID)).Select(c => new { c.CourseID, c.CourseName }).ToList(); rptSelectedCourses.DataBind(); pnlNoSelection.Visible = (SelectedCourseIDs.Count == 0); }
        protected void rptSelectedCourses_ItemCommand(object s, RepeaterCommandEventArgs e) { if (e.CommandName == "RemoveCourse") { SelectedCourseIDs.Remove(int.Parse(e.CommandArgument.ToString())); RefreshSelectedCoursesUI(); RefreshAvailableCoursesUI(); } }
        protected void gvAvailableCourses_RowCommand(object s, GridViewCommandEventArgs e) { if (e.CommandName == "AddCourse") { int id = int.Parse(e.CommandArgument.ToString()); if (!SelectedCourseIDs.Contains(id)) SelectedCourseIDs.Add(id); RefreshSelectedCoursesUI(); RefreshAvailableCoursesUI(); } }
        protected void cvRelatedCourses_ServerValidate(object s, ServerValidateEventArgs a) { if (!chkEditIsPractice.Checked) a.IsValid = (SelectedCourseIDs.Count >= 2); else a.IsValid = true; }
        protected void btnUpdateQuizDetails_Click(object sender, EventArgs e) { Page.Validate("QuizDetails"); if (!Page.IsValid) { ShowNotification("error", "Check fields."); return; } try { Quiz dbQ = manager.GetQuizDetails(QuizID); string t = txtEditTitle.Text.Trim(), d = txtEditDescription.Text.Trim(); int l = int.Parse(ddlEditLanguage.SelectedValue); bool iP = chkEditIsPractice.Checked, iC = fileUploadEditImage.HasFile; List<int> nC = iP ? new List<int> { int.Parse(ddlPracticeCourse.SelectedValue) } : SelectedCourseIDs; var dbC = dbQ.RelatedCourseIDs?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList() ?? new List<int>(); if (t == dbQ.Title && d == dbQ.Description && l == dbQ.LanguageID && iP == dbQ.IsPractice && !iC && new HashSet<int>(nC).SetEquals(dbC)) { ShowNotification("info", "No changes made."); return; } string p = iC ? HandleFileUpload(fileUploadEditImage, "Quiz") : null; if (manager.UpdateQuiz(QuizID, t, d, l, iP, p, nC.ToArray(), null)) { ShowNotification("success", "Saved."); BindQuizDetails(); } else ShowNotification("error", "Failed."); } catch (Exception ex) { ShowNotification("error", ex.Message); } }
        protected void btnActivateQuiz_Click(object s, EventArgs e) { if (manager.GetQuizQuestions(QuizID).Count < 5) { ShowNotification("warning", "Need 5 questions."); return; } manager.ActivateQuiz(QuizID); BindQuizDetails(); ShowNotification("success", "Activated!"); }
        protected void btnDeactivateQuiz_Click(object s, EventArgs e) { manager.DeactivatedQuiz(QuizID); BindQuizDetails(); ShowNotification("warning", "Deactivated."); }
        #endregion

        #region Question Management

        private void BindAllQuestions()
        {
            List<QuizQuestion> questions = manager.GetQuizQuestions(QuizID);
            rptQuestions.DataSource = questions.OrderBy(q => q.SequenceOrder);
            rptQuestions.DataBind();
            if (lblQuestionCountWarning != null) lblQuestionCountWarning.Visible = (questions.Count < 5);
        }

        protected void rptQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                QuizQuestion q = (QuizQuestion)e.Item.DataItem;
                DropDownList ddl = (DropDownList)e.Item.FindControl("ddlQuestionType");
                ddl.DataSource = manager.GetQuestionTypes();
                ddl.DataTextField = "TypeName";
                ddl.DataValueField = "QuestionTypeID";
                ddl.DataBind();
                ddl.SelectedValue = q.QuestionTypeID.ToString();

                ToggleQuestionEditPanels(e.Item, q.QuestionTypeID, q);

                Button btnSave = (Button)e.Item.FindControl("btnSaveQuestion");
                ScriptManager.GetCurrent(this).RegisterPostBackControl(btnSave);

                Button btnAddOpt = (Button)e.Item.FindControl("btnAddOption");
                if (btnAddOpt != null) ScriptManager.GetCurrent(this).RegisterPostBackControl(btnAddOpt);
            }
        }

        protected void ddlQuestionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            int questionId = Convert.ToInt32(((HiddenField)item.FindControl("hdnQuestionID")).Value);
            int seq = Convert.ToInt32(((HiddenField)item.FindControl("hdnSequence")).Value);
            int newTypeId = Convert.ToInt32(ddl.SelectedValue);
            string newTypeName = ddl.SelectedItem.Text;

            SaveQuestion(item, false);
            manager.UpdateQuestion(questionId, ((TextBox)item.FindControl("txtQuestionText")).Text.Trim(), newTypeId, null, null, null);

            if (newTypeName == "MCQ")
            {
                var q = manager.GetQuizQuestions(QuizID).FirstOrDefault(x => x.QuestionID == questionId);
                if (q != null && (q.Options == null || q.Options.Count == 0))
                {
                    manager.AddBlankOption(questionId);
                    manager.AddBlankOption(questionId);
                }
            }

            ExpandedSequence = seq;
            BindAllQuestions();
            KeepScrollPosition(questionId);
        }

        private void ToggleQuestionEditPanels(RepeaterItem item, int typeId, QuizQuestion data)
        {
            string type = manager.GetQuestionTypeNameById(typeId);
            item.FindControl("pnlMcqOptions").Visible = (type == "MCQ");
            item.FindControl("pnlTextInputAnswer").Visible = (type == "TypeInAnswer" || type == "FillInTheBlank");

            if (type == "MCQ" && data != null)
            {
                Repeater r = (Repeater)item.FindControl("rptOptions");
                r.DataSource = data.Options.OrderBy(o => o.OptionID);
                r.DataBind();
            }
        }

        protected void btnRemoveOption_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int optionIdToRemove = int.Parse(btn.CommandArgument);

            RepeaterItem optionItem = (RepeaterItem)btn.NamingContainer;
            Repeater rptOptions = (Repeater)optionItem.NamingContainer;
            RepeaterItem questionItem = (RepeaterItem)rptOptions.NamingContainer;

            int qId = int.Parse(((HiddenField)questionItem.FindControl("hdnQuestionID")).Value);
            int seq = int.Parse(((HiddenField)questionItem.FindControl("hdnSequence")).Value);

            // 1. Collect data to reconstruct the question state
            string txt = ((TextBox)questionItem.FindControl("txtQuestionText")).Text.Trim();
            int type = int.Parse(((DropDownList)questionItem.FindControl("ddlQuestionType")).SelectedValue);
            string img = HandleFileUpload((FileUpload)questionItem.FindControl("fileQuestionImage"), "Question");

            List<QuizAnswerOption> opts = new List<QuizAnswerOption>();
            foreach (RepeaterItem ri in rptOptions.Items)
            {
                int currentOptId = int.Parse(((HiddenField)ri.FindControl("hdnOptionID")).Value);
                // Exclude the one we clicked to remove
                if (currentOptId != optionIdToRemove)
                {
                    opts.Add(new QuizAnswerOption
                    {
                        QuestionID = qId,
                        OptionText = ((TextBox)ri.FindControl("txtOptionText")).Text.Trim(),
                        IsCorrect = ((RadioButton)ri.FindControl("rbCorrect")).Checked
                    });
                }
            }

            // 2. Update via manager (This nukes old options and inserts our filtered list)
            manager.UpdateQuestion(qId, txt, type, img, null, opts);

            ExpandedSequence = seq;
            BindAllQuestions();
            KeepScrollPosition(qId);
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int qId = Convert.ToInt32(e.CommandArgument);
            int seq = Convert.ToInt32(((HiddenField)e.Item.FindControl("hdnSequence")).Value);
            string cmd = e.CommandName;

            if (cmd == "MoveUp") { manager.ReorderQuestion(QuizID, qId, "UP"); ExpandedSequence = 0; BindAllQuestions(); KeepScrollPosition(qId); }
            else if (cmd == "MoveDown") { manager.ReorderQuestion(QuizID, qId, "DOWN"); ExpandedSequence = 0; BindAllQuestions(); KeepScrollPosition(qId); }
            else if (cmd == "DeleteQuestion") { manager.DeleteQuestion(qId); ExpandedSequence = 0; BindAllQuestions(); }
            else if (cmd == "SaveQuestion")
            {
                ExpandedSequence = seq;
                SaveQuestion(e.Item, true); // Explicit save (Validation & Change Check ON)
                // Do NOT rebind here inside the if, SaveQuestion handles logic
            }
            else if (cmd == "AddOption")
            {
                SaveQuestion(e.Item, false); // Silent save
                manager.AddBlankOption(qId);
                ExpandedSequence = seq;
                BindAllQuestions();
                KeepScrollPosition(qId);
            }
        }

        private void SaveQuestion(RepeaterItem item, bool showNotification)
        {
            try
            {
                int qId = int.Parse(((HiddenField)item.FindControl("hdnQuestionID")).Value);
                string txt = ((TextBox)item.FindControl("txtQuestionText")).Text.Trim();
                int type = int.Parse(((DropDownList)item.FindControl("ddlQuestionType")).SelectedValue);
                string img = HandleFileUpload((FileUpload)item.FindControl("fileQuestionImage"), "Question");
                string ans = null;
                List<QuizAnswerOption> opts = new List<QuizAnswerOption>();
                string typeName = manager.GetQuestionTypeNameById(type);

                if (typeName == "MCQ")
                {
                    Repeater r = (Repeater)item.FindControl("rptOptions");
                    foreach (RepeaterItem ri in r.Items)
                    {
                        opts.Add(new QuizAnswerOption
                        {
                            QuestionID = qId,
                            OptionText = ((TextBox)ri.FindControl("txtOptionText")).Text.Trim(),
                            IsCorrect = ((RadioButton)ri.FindControl("rbCorrect")).Checked
                        });
                    }
                }
                else
                {
                    ans = ((TextBox)item.FindControl("txtCorrectAnswer")).Text.Trim();
                }

                // --- VALIDATION LOGIC (Only for explicit save) ---
                if (showNotification)
                {
                    if (string.IsNullOrEmpty(txt)) { ShowNotification("warning", "Question text required."); return; }

                    if (typeName == "MCQ")
                    {
                        if (opts.Count < 2) { ShowNotification("warning", "MCQ needs at least 2 options."); return; }

                        // FIX: Ensure one option is correct
                        if (!opts.Any(o => o.IsCorrect)) { ShowNotification("warning", "Please select a correct answer."); return; }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ans)) { ShowNotification("warning", "Correct answer required."); return; }
                    }

                    // --- NO CHANGE DETECTION ---
                    var allQs = manager.GetQuizQuestions(QuizID);
                    var dbQ = allQs.FirstOrDefault(q => q.QuestionID == qId);
                    if (dbQ != null)
                    {
                        bool hasChanged = false;
                        if (dbQ.QuestionText != txt) hasChanged = true;
                        if (dbQ.QuestionTypeID != type) hasChanged = true;
                        if (!string.IsNullOrEmpty(img)) hasChanged = true; // new image uploaded
                        if ((dbQ.CorrectAnswer ?? "") != (ans ?? "")) hasChanged = true;

                        if (typeName == "MCQ")
                        {
                            if (dbQ.Options.Count != opts.Count) hasChanged = true;
                            else
                            {
                                for (int i = 0; i < opts.Count; i++)
                                {
                                    // Check if text or correct status differs
                                    if (dbQ.Options[i].OptionText != opts[i].OptionText ||
                                        dbQ.Options[i].IsCorrect != opts[i].IsCorrect)
                                    {
                                        hasChanged = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!hasChanged)
                        {
                            ShowNotification("info", "No changes were made.");
                            return; // Stop saving
                        }
                    }
                }

                // Save to Database
                manager.UpdateQuestion(qId, txt, type, img, ans, opts);

                if (showNotification)
                {
                    ShowNotification("success", "Question saved.");
                    BindAllQuestions(); // Rebind to refresh UI
                }
            }
            catch (Exception ex) { if (showNotification) ShowNotification("error", ex.Message); }
        }

        protected void btnAddNewQuestionSimple_Click(object s, EventArgs e)
        {
            manager.CreateQuestion(QuizID, "New Question", 1, null);
            var qs = manager.GetQuizQuestions(QuizID);
            ExpandedSequence = qs.Max(q => q.SequenceOrder);
            BindAllQuestions();
            ShowNotification("success", "Question added.");
            var lastQ = qs.OrderByDescending(q => q.SequenceOrder).FirstOrDefault();
            if (lastQ != null) KeepScrollPosition(lastQ.QuestionID);
        }

        // ... Helpers ...
        private void KeepScrollPosition(int questionId)
        {
            string script = $"setScrollTarget({questionId});";
            ScriptManager.RegisterStartupScript(this, GetType(), "ScrollFix_" + questionId, script, true);
        }
        protected string GetCollapseClass(object s) => (s != null && (int)s == ExpandedSequence) ? "collapse show" : "collapse";
        protected string GetIconClass(object s) => (s != null && (int)s == ExpandedSequence) ? "bi bi-chevron-up" : "bi bi-chevron-down";
        private string HandleFileUpload(FileUpload uploader, string subfolder) { if (uploader.HasFile) { string ext = Path.GetExtension(uploader.FileName); string fname = $"{Guid.NewGuid()}{ext}"; string path = Server.MapPath($"~/Image/{subfolder}/"); Directory.CreateDirectory(path); uploader.SaveAs(Path.Combine(path, fname)); return $"/Image/{subfolder}/{fname}"; } return null; }
        public string GetImagePath(object imagePath) { return (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString())) ? imagePath.ToString() : "/Image/System/placeholder.png"; }
        private void ShowNotification(string type, string message) { string s = message.Replace("'", "\\'").Replace("\r", "").Replace("\n", ""); ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"showNotification('{type}', '{s}');", true); }
        private void previewImage(object sender, EventArgs e) { }

        #endregion
    }
}
