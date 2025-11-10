using LexiPath.Data; // Make sure this namespace is correct
using System;
using System.Web.UI;

namespace LexiPath.Admin
{
    // This is the new base class for all admin pages
    // It inherits from System.Web.UI.Page
    public class AdminBasePage : Page
    {
        /**
         * We override the OnLoad event. This code will run
         * BEFORE the Page_Load of any admin page.
         */
        protected override void OnLoad(EventArgs e)
        {
            // 1. Check if user is logged in
            if (Session["User"] == null)
            {
                // Not logged in, send to login page
                Response.Redirect("~/Login.aspx?ReturnUrl=" + Request.Path);
                return;
            }

            // 2. Check if user is an Administrator
            User user = (User)Session["User"];
            if (!user.IsAdmin)
            {
                // Logged in, but NOT an admin. Send to home page.
                Response.Redirect("~/Home.aspx");
                return;
            }

            // If both checks pass, let the page load normally
            base.OnLoad(e);
        }
    }
}