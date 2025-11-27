using System;
using System.Web.UI;
using LexiPath.Data; 
using System.Security.Cryptography; // For password hashing
using System.Text; // For password hashing

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
                // 1. Get user input
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text; // Password should not be trimmed for security

                // 2. Hash the password for comparison (MUST match how it was hashed during registration)
                string hashedPassword = HashPassword(password);

                // 3. Authenticate using your UserManager
                UserManager manager = new UserManager();
                User authenticatedUser = manager.AuthenticateUser(username, hashedPassword);

                if (authenticatedUser != null)
                {
                    // 4. Authentication SUCCESS! Create the user session.
                    Session["User"] = authenticatedUser;

                    // Redirect the user based on role
                    if (authenticatedUser.IsAdmin)
                    {
                        // Redirect to the Admin Panel for administrators
                        Response.Redirect("~/Admin/AdminDashboard.aspx");
                    }
                    else
                    {
                        // Redirect to the Profile or Home page for regular users
                        Response.Redirect("~/Home.aspx");
                    }
                }
                else
                {
                    // 5. Authentication FAILURE
                    lblMessage.Text = "Invalid username or password.";
                }
            }
        }
    }
}