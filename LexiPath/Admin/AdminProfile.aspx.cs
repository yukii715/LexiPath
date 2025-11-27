using LexiPath.Data;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace LexiPath.Admin
{
    public partial class AdminProfile : System.Web.UI.Page
    {
        private User currentUser;
        private UserManager userManager = new UserManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["User"] == null) { Response.Redirect("~/Login.aspx"); return; }
            currentUser = (User)Session["User"];
            if (!currentUser.IsAdmin) { Response.Redirect("~/Home.aspx"); return; }

            if (!IsPostBack) { BindProfileData(); }
        }

        private void BindProfileData()
        {
            User profileData = userManager.GetUserProfile(currentUser.UserID);
            if (profileData != null)
            {
                txtUsername.Text = profileData.Username;
                txtEmail.Text = profileData.Email;
                if (!string.IsNullOrEmpty(profileData.ProfilePicPath))
                    imgProfilePic.ImageUrl = profileData.ProfilePicPath;
            }
        }

        protected void btnUploadPic_Click(object sender, EventArgs e)
        {
            if (fileUploadPic.HasFile)
            {
                try
                {
                    string extension = Path.GetExtension(fileUploadPic.FileName);
                    string fileName = $"Admin_{currentUser.UserID}_{Guid.NewGuid()}{extension}";
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
                    ShowNotification("error", "Error: " + ex.Message);
                }
            }
            else
            {
                ShowNotification("warning", "Please select a file.");
            }
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            string newUsername = txtUsername.Text.Trim();
            string newEmail = txtEmail.Text.Trim();

            if (userManager.UpdateUserProfile(currentUser.UserID, newUsername, newEmail))
            {
                ShowNotification("success", "Details updated successfully.");
                currentUser.Username = newUsername;
                currentUser.Email = newEmail;
                Session["User"] = currentUser;
            }
            else
            {
                ShowNotification("error", "Username or Email is already taken.");
            }
        }

        protected void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            string currentPass = txtCurrentPassword.Text;
            string newPass = txtNewPassword.Text;

            if (string.IsNullOrEmpty(currentPass) || string.IsNullOrEmpty(newPass))
            {
                ShowNotification("warning", "All password fields are required.");
                return;
            }

            string currentHash = HashPassword(currentPass);
            User verifiedUser = userManager.AuthenticateUser(currentUser.Username, currentHash);

            if (verifiedUser == null)
            {
                ShowNotification("error", "Incorrect Current Password.");
                return;
            }

            string newHash = HashPassword(newPass);
            if (userManager.UpdatePassword(currentUser.UserID, newHash))
            {
                ShowNotification("success", "Password changed successfully.");
                txtCurrentPassword.Text = "";
                txtNewPassword.Text = "";
                txtConfirmPassword.Text = "";
            }
            else
            {
                ShowNotification("error", "Failed to update password.");
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

        private void ShowNotification(string type, string message)
        {
            // Inject SweetAlert script
            string script = $"showNotification('{type}', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }
    }
}