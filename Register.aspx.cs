using System;
using System.Web.UI;
using LexiPath.Data; 
using System.Security.Cryptography; // For password hashing
using System.Text; // For password hashing

namespace LexiPath
{
    public partial class Register : System.Web.UI.Page
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

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            // Check if all page validators passed successfully
            if (Page.IsValid)
            {
                // 1. Get user input
                string username = txtUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                // 2. Hash the password before storing it (NEVER store raw passwords!)
                string hashedPassword = HashPassword(password);

                // 3. Register using your UserManager
                UserManager manager = new UserManager();

                bool registrationSuccess = manager.RegisterUser(username, email, hashedPassword);

                if (registrationSuccess)
                {
                    // 4. Registration SUCCESS!
                    lblMessage.Text = "Registration successful! You can now sign in.";
                    lblMessage.CssClass = "d-block mb-3 text-success font-weight-bold";

                    // Clear the form fields
                    txtUsername.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtPassword.Text = string.Empty;
                    txtConfirmPassword.Text = string.Empty;
                }
                else
                {
                    // 5. Registration FAILURE (likely due to duplicate username or email)
                    lblMessage.Text = "Registration failed. Username or Email may already exist.";
                    lblMessage.CssClass = "d-block mb-3 text-danger font-weight-bold";
                }
            }
        }
    }
}