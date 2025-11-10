using LexiPath.Data;
using System;
using System.IO; // Needed for file uploads
using System.Security.Cryptography; // Needed for password hashing
using System.Text; // Needed for password hashing
using System.Web.UI;

namespace LexiPath
{
    public partial class Profile : System.Web.UI.Page
    {
        private User currentUser;
        private UserManager userManager = new UserManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            // --- SECURITY CHECK ---
            // If no user is in the session, redirect to login
            if (Session["User"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            currentUser = (User)Session["User"];

            if (!IsPostBack)
            {
                // First time loading the page, fill in all the data
                BindProfileData();
                BindStatsData();
            }
        }

        /**
         * Fills in the user's details
         */
        private void BindProfileData()
        {
            // Get fresh data from DB
            User profileData = userManager.GetUserProfile(currentUser.UserID);
            if (profileData != null)
            {
                txtUsername.Text = profileData.Username;
                txtEmail.Text = profileData.Email;

                // Use a placeholder if no image is set
                imgProfilePic.ImageUrl = GetImagePath(profileData.ProfilePicPath);
            }
        }

        /**
         * Fills in the user's stats
         */
        private void BindStatsData()
        {
            AccessManager accessManager = new AccessManager();
            QuizManager quizManager = new QuizManager();

            litCoursesCompleted.Text = accessManager.GetCompletedCourseCount(currentUser.UserID).ToString();
            litQuizzesTaken.Text = quizManager.GetQuizAttemptCount(currentUser.UserID).ToString();
        }

        /**
         * Event: User clicks "Upload New Picture"
         */
        protected void btnUploadPic_Click(object sender, EventArgs e)
        {
            if (fileUploadPic.HasFile)
            {
                try
                {
                    // 1. Create a secure, unique filename
                    string extension = Path.GetExtension(fileUploadPic.FileName);
                    string fileName = $"{currentUser.UserID}_{Guid.NewGuid()}{extension}";

                    // 2. Define the path on the server
                    string savePath = Server.MapPath("~/Image/Profile/");
                    string fullPath = Path.Combine(savePath, fileName);

                    // 3. Save the file
                    Directory.CreateDirectory(savePath); // Ensure the folder exists
                    fileUploadPic.SaveAs(fullPath);

                    // 4. Save the *relative* path to the database
                    string dbPath = $"/Image/Profile/{fileName}";
                    userManager.UpdateProfilePicture(currentUser.UserID, dbPath);

                    // 5. Update the image on the page
                    imgProfilePic.ImageUrl = dbPath;
                    lblPicMessage.Text = "Picture updated!";
                    lblPicMessage.ForeColor = System.Drawing.Color.Green;

                    currentUser.ProfilePicPath = dbPath;
                    Session["User"] = currentUser;
                }
                catch (Exception ex)
                {
                    lblPicMessage.Text = "File upload failed: " + ex.Message;
                    lblPicMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblPicMessage.Text = "Please select a file to upload.";
                lblPicMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        /**
         * Event: User clicks "Save Details"
         */
        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            string newUsername = txtUsername.Text.Trim();
            string newEmail = txtEmail.Text.Trim();

            if (userManager.UpdateUserProfile(currentUser.UserID, newUsername, newEmail))
            {
                lblProfileMessage.Text = "Profile updated successfully!";
                lblProfileMessage.ForeColor = System.Drawing.Color.Green;

                // --- IMPORTANT ---
                // Update the Session object, otherwise the master page
                // will still show the old username!
                currentUser.Username = newUsername;
                currentUser.Email = newEmail;
                Session["User"] = currentUser;
            }
            else
            {
                lblProfileMessage.Text = "Update failed. The username or email might be taken.";
                lblProfileMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        /**
         * Event: User clicks "Change Password"
         */
        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            // We only check the validators for this specific group
            Page.Validate("PasswordGroup"); // You need to add this ValidationGroup to your controls
            if (!Page.IsValid) return;

            string newPassword = txtNewPassword.Text;
            if (string.IsNullOrEmpty(newPassword))
            {
                lblPasswordMessage.Text = "Password cannot be empty.";
                lblPasswordMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            // 1. Hash the new password
            string newHash = HashPassword(newPassword);

            // 2. Save to database
            if (userManager.UpdatePassword(currentUser.UserID, newHash))
            {
                lblPasswordMessage.Text = "Password changed successfully!";
                lblPasswordMessage.ForeColor = System.Drawing.Color.Green;
                txtNewPassword.Text = string.Empty;
                txtConfirmPassword.Text = string.Empty;
            }
            else
            {
                lblPasswordMessage.Text = "Password change failed.";
                lblPasswordMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        // --- Helper Methods ---

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GetImagePath(object imagePath)
        {
            if (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString()))
            {
                return imagePath.ToString();
            }
            else
            {
                // You should create a placeholder image here
                return "/Image/System/placeholder_profile.png";
            }
        }
    }
}