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
            get
            {
                // Try to get value from Session
                object val = Session["Admin_EditQuizID"];
                if (val == null)
                {
                    // If session is lost or empty, return 0 or redirect
                    return 0;
                }
                return Convert.ToInt32(val);
            }
            set
            {
                Session["Admin_EditQuizID"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (this.QuizID == 0)
                {
                    // No QuizID in session, send them back to the list
                    Response.Redirect("ManageQuizzes.aspx");
                    return;
                }

                hdnQuizID.Value = this.QuizID.ToString();
                BindQuizDetails();
                BindAllQuestions();

                // Bind the "Add New" dropdown
                BindQuestionTypeDropdown(ddlNewQuestionType);
            }
        }

        #region Quiz Details Management

        private void BindQuizDetails()
        {
            Quiz quiz = manager.GetQuizDetails(QuizID);
            if (quiz == null)
            {
                Response.Redirect("ManageQuizzes.aspx");
                return;
            }

            // Populate Text Fields
            txtEditTitle.Text = quiz.Title;
            txtEditDescription.Text = quiz.Description;
            imgEditPreview.ImageUrl = GetImagePath(quiz.ImagePath);
            chkEditIsPractice.Checked = quiz.IsPractice;

            // Populate Language
            BindLanguageDropdown(ddlEditLanguage);
            ddlEditLanguage.SelectedValue = quiz.LanguageID.ToString();

            // Populate Courses CheckBoxList
            BindCourseList(cblEditCourses);
            var courseIds = quiz.RelatedCourseIDs?.Split(',') ?? new string[0];
            foreach (ListItem item in cblEditCourses.Items)
            {
                item.Selected = courseIds.Contains(item.Value);
            }

            // Populate Tags CheckBoxList
            BindTagList(cblEditTags);
            var tagIds = quiz.RelatedTagIDs?.Split(',') ?? new string[0];
            foreach (ListItem item in cblEditTags.Items)
            {
                item.Selected = tagIds.Contains(item.Value);
            }

            // Trigger the panel visibility check
            ToggleCoursePanel();
        }

        protected void chkEditIsPractice_CheckedChanged(object sender, EventArgs e)
        {
            ToggleCoursePanel();
            upQuizDetails.Update();
        }

        private void ToggleCoursePanel()
        {
            bool isPractice = chkEditIsPractice.Checked;

            // Show panel for Practice (DropDownList)
            pnlPracticeCourse.Visible = isPractice;
            rfvPracticeCourse.Enabled = isPractice;

            // Show panel for Quiz (CheckBoxList)
            pnlRelatedCourses.Visible = !isPractice;
            cvRelatedCourses.Enabled = !isPractice;
        }

        protected void cvRelatedCourses_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // This validation only applies if it is a Quiz
            if (!chkEditIsPractice.Checked)
            {
                int selectedCount = cblEditCourses.Items.Cast<ListItem>().Count(li => li.Selected);
                args.IsValid = (selectedCount >= 2);
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void btnUpdateQuizDetails_Click(object sender, EventArgs e)
        {
            Page.Validate("QuizDetails");
            if (!Page.IsValid)
            {
                lblDetailMessage.Text = "Please correct the errors in the details section.";
                lblDetailMessage.ForeColor = System.Drawing.Color.Red;
                upQuizDetails.Update();
                return;
            }

            try
            {
                string title = txtEditTitle.Text.Trim();
                string desc = txtEditDescription.Text.Trim();
                int langId = Convert.ToInt32(ddlEditLanguage.SelectedValue);
                bool isPractice = chkEditIsPractice.Checked;

                string dbPath = HandleFileUpload(fileUploadEditImage, "Quiz");

                int[] courseIds = GetSelectedCheckboxIds(cblEditCourses);
                int[] tagIds = GetSelectedCheckboxIds(cblEditTags);

                if (manager.UpdateQuiz(QuizID, title, desc, langId, isPractice, dbPath, courseIds, tagIds))
                {
                    lblDetailMessage.Text = "Quiz details updated successfully.";
                    lblDetailMessage.ForeColor = System.Drawing.Color.Green;
                    BindQuizDetails(); // Rebind to reflect changes
                }
                else
                {
                    lblDetailMessage.Text = "Error updating quiz.";
                    lblDetailMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblDetailMessage.Text = "Error: " + ex.Message;
                lblDetailMessage.ForeColor = System.Drawing.Color.Red;
            }
            upQuizDetails.Update();
        }

        #endregion

        #region Question Management

        private void BindAllQuestions()
        {
            // *** NEW QuizManager Method Needed ***
            List<QuizQuestion> questions = manager.GetQuizQuestions(QuizID);

            rptQuestions.DataSource = questions.OrderBy(q => q.SequenceOrder);
            rptQuestions.DataBind();

            // REQUIREMENT: Check for 5 questions
            lblQuestionCountWarning.Visible = (questions.Count < 5 && !chkEditIsPractice.Checked);
            upQuestions.Update();
        }

        protected void rptQuestions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                QuizQuestion question = (QuizQuestion)e.Item.DataItem;

                // Bind the question type dropdown
                DropDownList ddlQuestionType = (DropDownList)e.Item.FindControl("ddlQuestionType");
                BindQuestionTypeDropdown(ddlQuestionType);
                ddlQuestionType.SelectedValue = question.QuestionTypeID.ToString();

                // Show the correct panel based on type
                ToggleQuestionEditPanels(e.Item, question.QuestionTypeID, question);
            }
        }

        protected void ddlQuestionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Find the RepeaterItem that this dropdown is in
            DropDownList ddl = (DropDownList)sender;
            RepeaterItem item = (RepeaterItem)ddl.NamingContainer;
            int questionTypeID = Convert.ToInt32(ddl.SelectedValue);

            ToggleQuestionEditPanels(item, questionTypeID, null);
            upQuestions.Update();
        }

        private void ToggleQuestionEditPanels(RepeaterItem item, int questionTypeID, QuizQuestion data)
        {
            // *** NEW QuizManager Method Needed ***
            // This method should return a simple object or string, e.g., "MCQ", "TypeInAnswer"
            string typeName = manager.GetQuestionTypeNameById(questionTypeID);

            Panel pnlMcq = (Panel)item.FindControl("pnlMcqOptions");
            Panel pnlText = (Panel)item.FindControl("pnlTextInputAnswer");

            pnlMcq.Visible = false;
            pnlText.Visible = false;

            if (typeName == "MCQ")
            {
                pnlMcq.Visible = true;
                // If data is provided (on databound), bind the inner repeater
                if (data != null)
                {
                    Repeater rptOptions = (Repeater)item.FindControl("rptOptions");
                    rptOptions.DataSource = data.Options.OrderBy(o => o.OptionID);
                    rptOptions.DataBind();
                }
            }
            else if (typeName == "TypeInAnswer" || typeName == "FillInTheBlank")
            {
                pnlText.Visible = true;
            }
        }

        protected void rptQuestions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int questionId = Convert.ToInt32(e.CommandArgument);

            // *** NEW QuizManager Methods Needed for reordering ***
            if (e.CommandName == "MoveUp")
            {
                manager.ReorderQuestion(QuizID, questionId, "UP");
            }
            else if (e.CommandName == "MoveDown")
            {
                manager.ReorderQuestion(QuizID, questionId, "DOWN");
            }
            else if (e.CommandName == "DeleteQuestion")
            {
                // *** NEW QuizManager Method Needed ***
                manager.DeleteQuestion(questionId);
            }
            else if (e.CommandName == "SaveQuestion")
            {
                SaveQuestion(e.Item);
                return; // SaveQuestion handles its own update
            }
            else if (e.CommandName == "AddOption")
            {
                // Adds a new blank option to this question
                // *** NEW QuizManager Method Needed ***
                manager.AddBlankOption(questionId);
            }

            BindAllQuestions();
            upQuestions.Update();
        }

        private void SaveQuestion(RepeaterItem item)
        {
            try
            {
                int questionId = Convert.ToInt32(((HiddenField)item.FindControl("hdnQuestionID")).Value);
                string questionText = ((TextBox)item.FindControl("txtQuestionText")).Text;
                int questionTypeID = Convert.ToInt32(((DropDownList)item.FindControl("ddlQuestionType")).SelectedValue);
                FileUpload uploader = (FileUpload)item.FindControl("fileQuestionImage");
                string imagePath = HandleFileUpload(uploader, "Question");

                string correctAnswer = null;
                List<QuizAnswerOption> options = new List<QuizAnswerOption>();

                string typeName = manager.GetQuestionTypeNameById(questionTypeID);

                if (typeName == "MCQ")
                {
                    Repeater rptOptions = (Repeater)item.FindControl("rptOptions");
                    int correctCount = 0;
                    foreach (RepeaterItem optItem in rptOptions.Items)
                    {
                        string optionText = ((TextBox)optItem.FindControl("txtOptionText")).Text;
                        bool isCorrect = ((RadioButton)optItem.FindControl("rbCorrect")).Checked;
                        if (isCorrect) correctCount++;

                        options.Add(new QuizAnswerOption
                        {
                            QuestionID = questionId,
                            OptionText = optionText,
                            IsCorrect = isCorrect
                        });
                    }

                    // REQUIREMENT: Validate at least 2 options and one is correct
                    if (options.Count < 2)
                    {
                        ShowQuestionError("An MCQ question must have at least 2 options.", item);
                        return;
                    }
                    if (correctCount == 0)
                    {
                        ShowQuestionError("You must select one correct answer for an MCQ.", item);
                        return;
                    }
                }
                else
                {
                    correctAnswer = ((TextBox)item.FindControl("txtCorrectAnswer")).Text;
                }

                // *** NEW QuizManager Method Needed ***
                // This method must be able to update the question text, type, image,
                // and (if MCQ) delete all existing options and re-add the new list.
                manager.UpdateQuestion(questionId, questionText, questionTypeID, imagePath, correctAnswer, options);

                lblQuestionMessage.Text = "Question saved successfully.";
                lblQuestionMessage.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblQuestionMessage.Text = "Error saving question: " + ex.Message;
                lblQuestionMessage.ForeColor = System.Drawing.Color.Red;
            }

            BindAllQuestions();
            upQuestions.Update();
        }

        protected void rptOptions_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "RemoveOption")
            {
                int optionId = Convert.ToInt32(e.CommandArgument);
                // *** NEW QuizManager Method Needed ***
                manager.DeleteOption(optionId);

                BindAllQuestions();
                upQuestions.Update();
            }
        }

        protected void btnAddQuestion_Click(object sender, EventArgs e)
        {
            try
            {
                string questionText = txtNewQuestionText.Text.Trim();
                int questionTypeID = Convert.ToInt32(ddlNewQuestionType.SelectedValue);
                string imagePath = HandleFileUpload(fileNewQuestionImage, "Question");

                if (string.IsNullOrEmpty(questionText))
                {
                    lblQuestionMessage.Text = "New question text cannot be empty.";
                    lblQuestionMessage.ForeColor = System.Drawing.Color.Red;
                    upQuestions.Update();
                    return;
                }

                // *** NEW QuizManager Method Needed ***
                // This method creates a new question. If MCQ, it should also add 2 blank options by default.
                manager.CreateQuestion(QuizID, questionText, questionTypeID, imagePath);

                // Clear new question form
                txtNewQuestionText.Text = "";
                fileNewQuestionImage.Dispose();
                ddlNewQuestionType.SelectedIndex = 0;

                lblQuestionMessage.Text = "New question added.";
                lblQuestionMessage.ForeColor = System.Drawing.Color.Green;

            }
            catch (Exception ex)
            {
                lblQuestionMessage.Text = "Error adding question: " + ex.Message;
                lblQuestionMessage.ForeColor = System.Drawing.Color.Red;
            }

            BindAllQuestions();
            upQuestions.Update();
        }

        #endregion

        #region Helpers

        private void BindCourseDropdown(DropDownList ddl)
        {
            ddl.DataSource = manager.GetAllCourses();
            ddl.DataTextField = "CourseName";
            ddl.DataValueField = "CourseID";
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-- Select a Course --", "0"));
        }

        private void BindLanguageDropdown(DropDownList ddl)
        {
            ddl.DataSource = courseManager.GetAllLanguages();
            ddl.DataTextField = "LanguageName";
            ddl.DataValueField = "LanguageID";
            ddl.DataBind();
        }

        private void BindCourseList(CheckBoxList cbl)
        {
            cbl.DataSource = manager.GetAllCourses();
            cbl.DataTextField = "CourseName";
            cbl.DataValueField = "CourseID";
            cbl.DataBind();
        }

        private void BindTagList(CheckBoxList cbl)
        {
            cbl.DataSource = manager.GetAllTags();
            cbl.DataTextField = "TagName";
            cbl.DataValueField = "TagID";
            cbl.DataBind();
        }

        private void BindQuestionTypeDropdown(DropDownList ddl)
        {
            // *** NEW QuizManager Method Needed ***
            ddl.DataSource = manager.GetQuestionTypes(); // Should return list of {QuestionTypeID, QuestionTypeName}
            ddl.DataTextField = "TypeName";
            ddl.DataValueField = "QuestionTypeID";
            ddl.DataBind();
        }

        private int[] GetSelectedCheckboxIds(CheckBoxList cbl)
        {
            return cbl.Items.Cast<ListItem>()
                .Where(li => li.Selected)
                .Select(li => Convert.ToInt32(li.Value))
                .ToArray();
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
            if (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString()))
            {
                return imagePath.ToString();
            }
            return "/Image/System/placeholder.png";
        }

        private void ShowQuestionError(string message, RepeaterItem item)
        {
            Label lblOptionError = (Label)item.FindControl("lblOptionError");
            if (lblOptionError != null)
            {
                lblOptionError.Text = message;
            }
            else
            {
                lblQuestionMessage.Text = message;
            }
            lblQuestionMessage.ForeColor = System.Drawing.Color.Red;
            upQuestions.Update();
        }

        #endregion
    }
}