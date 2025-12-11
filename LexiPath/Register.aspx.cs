using System;
using System.Web.UI;
using LexiPath.Data;
using System.Security.Cryptography;
using System.Text;

namespace LexiPath
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

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
            if (Page.IsValid)
            {
                string username = txtUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                UserManager manager = new UserManager();

                if (manager.IsUsernameTaken(username))
                {
                    lblMessage.Text = "That username is already taken. Please choose another.";
                    lblMessage.CssClass = "d-block mb-3 text-danger font-weight-bold text-center";
                    return; 
                }

                if (manager.IsEmailTaken(email))
                {
                    lblMessage.Text = "That email is already registered. Please sign in.";
                    lblMessage.CssClass = "d-block mb-3 text-danger font-weight-bold text-center";
                    return; 
                }

                string hashedPassword = HashPassword(password);
                bool registrationSuccess = manager.RegisterUser(username, email, hashedPassword);

                if (registrationSuccess)
                {
                    lblMessage.Text = "Registration successful! You can now sign in.";
                    lblMessage.CssClass = "d-block mb-3 text-success font-weight-bold text-center";

                    txtUsername.Text = string.Empty;
                    txtEmail.Text = string.Empty;
                    txtPassword.Text = string.Empty;
                    txtConfirmPassword.Text = string.Empty;
                }
                else
                {
                    lblMessage.Text = "Registration failed. Please try again.";
                    lblMessage.CssClass = "d-block mb-3 text-danger font-weight-bold text-center";
                }
            }
        }
    }
}