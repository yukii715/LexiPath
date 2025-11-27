using LexiPath.Data;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace LexiPath
{
    public partial class Profile : System.Web.UI.Page
    {
        private User currentUser;
        private UserManager userManager = new UserManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null) { Response.Redirect("~/Login.aspx"); return; }
            currentUser = (User)Session["User"];
            if (!IsPostBack) { BindProfileData(); BindStatsData(); }
        }

        private void BindProfileData()
        {
            User profileData = userManager.GetUserProfile(currentUser.UserID);
            if (profileData != null)
            {
                txtUsername.Text = profileData.Username;
                txtEmail.Text = profileData.Email;
                if (!string.IsNullOrEmpty(profileData.ProfilePicPath)) imgProfilePic.ImageUrl = profileData.ProfilePicPath;
            }
        }

        private void BindStatsData()
        {
            AccessManager accessManager = new AccessManager();
            QuizManager quizManager = new QuizManager();
            int userId = currentUser.UserID;
            double totalMinutes = accessManager.GetTotalLearningMinutes(userId);
            double totalHours = totalMinutes / 60.0;

            litCoursesCompleted.Text = accessManager.GetCompletedCourseCount(userId).ToString();
            litQuizzesTaken.Text = quizManager.GetRealQuizAttemptCount(userId).ToString();
            litLearningHours.Text = $"{totalHours:0.00}";
            BindUserCourses();
        }

        private void BindUserCourses()
        {
            int userId = currentUser.UserID;
            string searchTerm = txtCourseSearch.Text.Trim();
            string orderBy = ddlCourseSort.SelectedValue;

            var likedCourses = userManager.GetLikedCourses(userId, searchTerm, orderBy);
            rptLikedCourses.DataSource = likedCourses;
            rptLikedCourses.DataBind();
            lblNoLiked.Visible = !likedCourses.Any();

            var collectedCourses = userManager.GetCollectedCourses(userId, searchTerm, orderBy);
            rptCollectedCourses.DataSource = collectedCourses;
            rptCollectedCourses.DataBind();
            lblNoCollected.Visible = !collectedCourses.Any();

            if (IsPostBack && btnSearchCourses.Visible)
            {
                ShowNotification("success", $"Filtered. Found {likedCourses.Count + collectedCourses.Count} saved courses.");
            }
        }

        protected void btnUploadPic_Click(object sender, EventArgs e)
        {
            if (fileUploadPic.HasFile)
            {
                try
                {
                    string extension = Path.GetExtension(fileUploadPic.FileName);
                    string fileName = $"{currentUser.UserID}_{Guid.NewGuid()}{extension}";
                    string savePath = Server.MapPath("~/Image/Profile/");
                    Directory.CreateDirectory(savePath);
                    string fullPath = Path.Combine(savePath, fileName);
                    fileUploadPic.SaveAs(fullPath);

                    string dbPath = $"/Image/Profile/{fileName}";
                    userManager.UpdateProfilePicture(currentUser.UserID, dbPath);

                    imgProfilePic.ImageUrl = dbPath;
                    ShowNotification("success", "Picture updated successfully.");

                    currentUser.ProfilePicPath = dbPath;
                    Session["User"] = currentUser;
                }
                catch (Exception ex)
                {
                    ShowNotification("error", "Upload failed: " + ex.Message);
                }
            }
            else
            {
                ShowNotification("warning", "Please select a file.");
            }
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            Page.Validate("ProfileDetails");
            if (!Page.IsValid) return;

            string newUsername = txtUsername.Text.Trim();
            string newEmail = txtEmail.Text.Trim();

            // --- NEW: SERVER-SIDE VALIDATION ---

            // 1. Check if Username is taken by someone else
            if (userManager.IsUsernameTaken(newUsername, currentUser.UserID))
            {
                ShowNotification("error", "That username is already taken.");
                return;
            }

            // 2. Check if Email is taken by someone else
            if (userManager.IsEmailTaken(newEmail, currentUser.UserID))
            {
                ShowNotification("error", "That email is already in use.");
                return;
            }

            // 3. Update
            if (userManager.UpdateUserProfile(currentUser.UserID, newUsername, newEmail))
            {
                ShowNotification("success", "Profile updated successfully!");
                currentUser.Username = newUsername;
                currentUser.Email = newEmail;
                Session["User"] = currentUser;
            }
            else
            {
                ShowNotification("error", "Update failed. Please try again.");
            }
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            string currentPass = txtCurrentPassword.Text;
            string newPass = txtNewPassword.Text;
            string confirmPass = txtConfirmPassword.Text;

            // 1. Manual Check: Are fields empty?
            if (string.IsNullOrEmpty(currentPass) || string.IsNullOrEmpty(newPass) || string.IsNullOrEmpty(confirmPass))
            {
                ShowNotification("warning", "All password fields are required.");
                return;
            }

            // 2. Manual Check: Do passwords match?
            if (newPass != confirmPass)
            {
                ShowNotification("error", "New passwords do not match.");
                return;
            }

            // 3. Manual Check: Complexity (Regex)
            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(newPass, passwordPattern))
            {
                ShowNotification("error", "Password must be 8-20 chars with uppercase, lowercase, number, and symbol.");
                return;
            }

            // 4. Verify Current Password (Database Check)
            string currentHash = HashPassword(currentPass);
            User verifiedUser = userManager.AuthenticateUser(currentUser.Username, currentHash);

            if (verifiedUser == null)
            {
                ShowNotification("error", "Incorrect Current Password.");
                return;
            }

            // 5. Save to Database
            string newHash = HashPassword(newPass);
            if (userManager.UpdatePassword(currentUser.UserID, newHash))
            {
                ShowNotification("success", "Password changed successfully!");

                // Clear fields on success
                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            else
            {
                ShowNotification("error", "Password change failed.");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        protected void btnSearchCourses_Click(object sender, EventArgs e)
        {
            BindUserCourses();
            string script = "document.getElementById('savedCoursesAnchor').scrollIntoView({behavior: 'smooth'});";
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScrollToResults", script, true);
        }

        private void ShowNotification(string type, string message)
        {
            string script = $"showNotification('{type}', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}