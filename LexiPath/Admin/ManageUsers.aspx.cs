using LexiPath.Data;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath.Admin
{
    public partial class ManageUsers : AdminBasePage
    {
        private UserManager userManager = new UserManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            gvUsers.DataSource = userManager.GetAllUsers();
            gvUsers.DataBind();
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(e.CommandArgument);

                if (e.CommandName == "ToggleStatus")
                {
                    userManager.ToggleUserStatus(userId);
                    BindGrid();
                    ShowNotification("success", "User status updated.");
                }
                else if (e.CommandName == "ResetPassword")
                {
                    string newPass = GenerateRandomPassword(10);
                    string hashedPass = HashPassword(newPass);

                    if (userManager.UpdatePassword(userId, hashedPass))
                    {
                        ShowPasswordModal(newPass);
                    }
                    else
                    {
                        ShowNotification("error", "Failed to reset password.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowNotification("error", "Error: " + ex.Message);
            }
        }

        private string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%";
            StringBuilder res = new StringBuilder();
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];
                while (length-- > 0)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    res.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }
            return res.ToString();
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

        public string GetProfileImage(object imagePath)
        {
            if (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString()))
            {
                return imagePath.ToString();
            }
            return "/Image/System/placeholder_profile.png";
        }

        private void ShowNotification(string type, string message)
        {
            string script = $"showNotification('{type}', '{message.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "toast", script, true);
        }

        private void ShowPasswordModal(string password)
        {
            string script = $"showPasswordModal('{password}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "passwordModal", script, true);
        }
    }
}