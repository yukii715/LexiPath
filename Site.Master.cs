using System;
using LexiPath.Data; // Ensure this is at the top
using System.Web.UI;

namespace LexiPath
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- NEW LANGUAGE SELECTION LOGIC ---

            // 1. Check if the user is *changing* the language (e.g., ?lang=1)
            if (!string.IsNullOrEmpty(Request.QueryString["lang"]))
            {
                int langId = 0;
                if (int.TryParse(Request.QueryString["lang"], out langId))
                {
                    // 2. Save the new language choice to the Session
                    Session["LanguageID"] = langId;
                }

                // 3. Reload the page *without* the ?lang=... in the URL
                // Request.Path returns the current page (e.g., /Categories.aspx)
                Response.Redirect(Request.Path);
                return;
            }

            // 4. On normal page load, set the default language if one isn't chosen
            if (Session["LanguageID"] == null)
            {
                Session["LanguageID"] = 1; // Default to Korean (ID 1)
            }

            // 5. Update the menu text to show the current language
            if ((int)Session["LanguageID"] == 1)
            {
                litCurrentLanguage.Text = "Korean";
            }
            else
            {
                litCurrentLanguage.Text = "English";
            }

            // --- END OF LANGUAGE LOGIC ---


            // --- Your existing Avatar/Login logic ---
            if (Session["User"] != null)
            {
                pnlGuest.Visible = false;
                pnlUser.Visible = true;
                User user = (User)Session["User"];
                if (user.IsAdmin)
                {
                    liAdmin.Visible = true;
                }

                if (!string.IsNullOrEmpty(user.ProfilePicPath))
                {
                    imgAvatar.ImageUrl = user.ProfilePicPath;
                }
                else
                {
                    // Set a default placeholder if they haven't uploaded one
                    imgAvatar.ImageUrl = "/Image/System/placeholder_profile.png";
                }
            }
            else
            {
                pnlGuest.Visible = true;
                pnlUser.Visible = false;
                liAdmin.Visible = false;
            }
        }
    }
}