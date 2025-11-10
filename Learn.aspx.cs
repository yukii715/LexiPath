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
        public bool IsUserRegistered { get; set; } = false; // Used by JavaScript

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] != null)
            {
                currentUser = (User)Session["User"];
                IsUserRegistered = true;
            }

            if (Session["ActiveCourseID"] == null)
            {
                // No course was started, close this tab immediately.
                ClientScript.RegisterStartupScript(this.GetType(), "close", "window.close();", true);
                return;
            }
            currentCourseId = (int)Session["ActiveCourseID"];

            if (!IsPostBack)
            {
                LoadLearningItems();
                Session["LearningItems"] = learningItems;
                DisplayItem(0);
            }
            else
            {
                learningItems = (List<LearningItem>)Session["LearningItems"];
            }
        }

        // --- Button Click Events ---

        protected void btnNext_Click(object sender, EventArgs e)
        {
            int currentIndex = GetCurrentIndex();
            if (currentIndex < learningItems.Count - 1)
            {
                DisplayItem(currentIndex + 1);
            }
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            int currentIndex = GetCurrentIndex();
            if (currentIndex > 0)
            {
                DisplayItem(currentIndex - 1);
            }
        }

        /**
         * This event fires when the "End Lesson" button is clicked.
         * It saves progress and closes the tab.
         */
        protected void btnEndLesson_Click(object sender, EventArgs e)
        {
            if (IsUserRegistered && currentUser != null)
            {
                AccessManager accessManager = new AccessManager();
                accessManager.EndCourse(currentUser.UserID, currentCourseId);
            }

            CloseAndRefreshOpener();
        }

        /**
         * *** FIX ***
         * This event fires when the "X" button (lnkBack) is clicked.
         * It does NOT save progress, but still closes the tab and refreshes the opener.
         */
        protected void lnkBack_Click(object sender, EventArgs e)
        {
            // This C# code runs *after* the "confirmEndLesson()" JavaScript prompt.
            // We just close the tab without saving progress.
            CloseAndRefreshOpener();
        }

        /**
         * *** FIX ***
         * This helper function now uses a 'clean' refresh
         * to prevent the form resubmission loop.
         */
        private void CloseAndRefreshOpener()
        {
            // Clear the session so this lesson can't be re-opened
            Session["ActiveCourseID"] = null;
            Session["LearningItems"] = null;

            // This is the new, safer script
            string script = @"
                if (window.opener && !window.opener.closed) {
                    // This forces a GET request and AVOIDS form resubmission
                    window.opener.location.href = window.opener.location.href;
                }
                var isLessonComplete = true; // Stop the onbeforeunload prompt
                window.onbeforeunload = null; 
                window.close();
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshAndClose", script, true);
        }

        // --- Core Logic Methods (Unchanged) ---

        private void LoadLearningItems()
        {
            learningItems = new List<LearningItem>();
            Course course = manager.GetCourseDetails(currentCourseId);
            if (course == null) Response.Redirect("Courses.aspx");
            litCourseName.Text = course.CourseName;
            List<Vocabulary> vocabList = new List<Vocabulary>();
            List<Phrase> phraseList = new List<Phrase>();
            if (course.CourseType == "Vocabulary" || course.CourseType == "Mixed")
            {
                vocabList = manager.GetVocabByCourseID(currentCourseId);
            }
            if (course.CourseType == "Phrase" || course.CourseType == "Mixed")
            {
                phraseList = manager.GetPhrasesByCourseID(currentCourseId);
            }
            foreach (var v in vocabList)
            {
                learningItems.Add(new LearningItem
                {
                    ItemID = v.VocabID,
                    ItemType = "Vocab",
                    SequenceOrder = v.SequenceOrder,
                    VocabText = v.VocabText,
                    VocabMeaning = v.Meaning,
                    VocabImagePath = GetImagePath(v.ImagePath)
                });
            }
            foreach (var p in phraseList)
            {
                learningItems.Add(new LearningItem
                {
                    ItemID = p.PhraseID,
                    ItemType = "Phrase",
                    SequenceOrder = p.SequenceOrder,
                    PhraseText = p.PhraseText,
                    PhraseMeaning = p.Meaning,
                    PhraseDetails = p.Details
                });
            }
            learningItems = learningItems.OrderBy(item => item.SequenceOrder).ToList();
        }

        private void DisplayItem(int index)
        {
            if (learningItems == null || learningItems.Count == 0)
            {
                pnlVocabView.Visible = false;
                pnlPhraseView.Visible = false;
                litCurrentPage.Text = "0 / 0";
                btnNext.Visible = false;
                btnPrev.Visible = false;
                btnEndLesson.Visible = true;
                return;
            }
            LearningItem item = learningItems[index];
            if (item.ItemType == "Vocab")
            {
                pnlVocabView.Visible = true;
                pnlPhraseView.Visible = false;
                imgVocab.ImageUrl = item.VocabImagePath;
                litVocabText.Text = item.VocabText;
                litVocabMeaning.Text = item.VocabMeaning;
            }
            else
            {
                pnlVocabView.Visible = false;
                pnlPhraseView.Visible = true;
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

        private int GetCurrentIndex()
        {
            if (ViewState["CurrentIndex"] != null)
            {
                return (int)ViewState["CurrentIndex"];
            }
            return 0;
        }

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