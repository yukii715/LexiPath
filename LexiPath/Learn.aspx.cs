using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class Learn : System.Web.UI.Page
    {
        private int currentCourseId = 0;
        private CourseManager manager = new CourseManager();
        private List<LearningItem> learningItems;
        private User currentUser = null;

        public bool IsUserRegistered { get; set; } = false;
        public bool IsAdmin { get; set; } = false;

        // Stores the code like "ko-KR" or "en-US" to send to JavaScript
        public string CourseLanguageCode { get; set; } = "en-US";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] != null)
            {
                currentUser = (User)Session["User"];
                IsUserRegistered = true;
                if (currentUser.IsAdmin) IsAdmin = true;
            }

            if (Session["ActiveCourseID"] == null)
            {
                ExitLesson();
                return;
            }
            currentCourseId = (int)Session["ActiveCourseID"];

            if (!IsPostBack)
            {
                LoadLearningItems();
                Session["LearningItems"] = learningItems;
                // Store the DB-fetched language code in ViewState
                ViewState["CourseLanguageCode"] = CourseLanguageCode;
                DisplayItem(0);
            }
            else
            {
                learningItems = (List<LearningItem>)Session["LearningItems"];
                if (ViewState["CourseLanguageCode"] != null)
                    CourseLanguageCode = ViewState["CourseLanguageCode"].ToString();
            }
        }

        protected void btnNext_Click(object sender, EventArgs e) { DisplayItem(GetCurrentIndex() + 1); }
        protected void btnPrev_Click(object sender, EventArgs e) { DisplayItem(GetCurrentIndex() - 1); }
        protected void btnEndLesson_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsUserRegistered && currentUser != null && !IsAdmin)
                {
                    AccessManager accessManager = new AccessManager();
                    accessManager.EndCourse(currentUser.UserID, currentCourseId);
                }
                ExitLesson();
            }
            catch (Exception ex) { ShowNotification("error", "Error: " + ex.Message); }
        }
        protected void lnkBack_Click(object sender, EventArgs e) { ExitLesson(); }
        private void ExitLesson()
        {
            Session["ActiveCourseID"] = null; Session["LearningItems"] = null; Session["CurrentIndex"] = null;
            if (Session["ReturnCourseID"] != null) Response.Redirect($"~/CourseDetail.aspx?CourseID={Session["ReturnCourseID"]}");
            else Response.Redirect("~/Courses.aspx");
        }

        private void LoadLearningItems()
        {
            learningItems = new List<LearningItem>();
            Course course = manager.GetCourseDetails(currentCourseId);
            if (course == null) Response.Redirect("Courses.aspx");

            litCourseName.Text = course.CourseName;

            // --- UPDATED: Fetch the code directly from the DB ---
            CourseLanguageCode = manager.GetLanguageCode(course.LanguageID);

            // ... (Keep existing logic for loading Vocab/Phrases) ...
            List<Vocabulary> vocabList = new List<Vocabulary>();
            List<Phrase> phraseList = new List<Phrase>();

            if (course.CourseType == "Vocabulary" || course.CourseType == "Mixed") vocabList = manager.GetVocabByCourseID(currentCourseId);
            if (course.CourseType == "Phrase" || course.CourseType == "Mixed") phraseList = manager.GetPhrasesByCourseID(currentCourseId);

            foreach (var v in vocabList)
            {
                learningItems.Add(new LearningItem { ItemID = v.VocabID, ItemType = "Vocab", SequenceOrder = v.SequenceOrder, VocabText = v.VocabText, VocabMeaning = v.Meaning, VocabImagePath = GetImagePath(v.ImagePath) });
            }
            foreach (var p in phraseList)
            {
                learningItems.Add(new LearningItem { ItemID = p.PhraseID, ItemType = "Phrase", SequenceOrder = p.SequenceOrder, PhraseText = p.PhraseText, PhraseMeaning = p.Meaning, PhraseDetails = p.Details });
            }
            learningItems = learningItems.OrderBy(item => item.SequenceOrder).ToList();
        }

        // ... (Keep DisplayItem, GetCurrentIndex, GetImagePath, ShowNotification) ...
        private void DisplayItem(int index)
        {
            if (learningItems == null || learningItems.Count == 0) return;
            LearningItem item = learningItems[index];

            btnVocabAudio.Visible = IsUserRegistered;
            btnPhraseAudio.Visible = IsUserRegistered;

            if (item.ItemType == "Vocab")
            {
                pnlVocabView.Visible = true; pnlPhraseView.Visible = false;
                imgVocab.ImageUrl = item.VocabImagePath;
                litVocabText.Text = item.VocabText;
                litVocabMeaning.Text = item.VocabMeaning;
            }
            else
            {
                pnlVocabView.Visible = false; pnlPhraseView.Visible = true;
                litPhraseText.Text = item.PhraseText;
                litPhraseMeaning.Text = item.PhraseMeaning;
                rptPhraseDetails.DataSource = item.PhraseDetails;
                rptPhraseDetails.DataBind();
            }
            litCurrentPage.Text = $"{index + 1} / {learningItems.Count}";
            btnPrev.Visible = (index > 0);
            btnNext.Visible = (index < learningItems.Count - 1);
            btnEndLesson.Visible = (index == learningItems.Count - 1);
            ViewState["CurrentIndex"] = index;
        }

        private int GetCurrentIndex() { return (int?)ViewState["CurrentIndex"] ?? 0; }
        public string GetImagePath(object imagePath) { return (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString())) ? imagePath.ToString() : "/Image/System/placeholder.png"; }
        private void ShowNotification(string type, string message)
        {
            string script = $"showNotification('{type}', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}