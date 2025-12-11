using System;
using System.Web.UI;
using LexiPath.Data; 
using System.Security.Cryptography; 
using System.Text; 

namespace LexiPath
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Helper function to hash the password using SHA256
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

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text; 

                string hashedPassword = HashPassword(password);

                UserManager manager = new UserManager();
                User authenticatedUser = manager.AuthenticateUser(username, hashedPassword);

                if (authenticatedUser != null)
                {
                    Session["User"] = authenticatedUser;

                    if (authenticatedUser.IsAdmin)
                    {
                        Response.Redirect("~/Admin/AdminDashboard.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/Home.aspx");
                    }
                }
                else
                {
                    lblMessage.Text = "Invalid username or password.";
                }
            }
        }
    }
}