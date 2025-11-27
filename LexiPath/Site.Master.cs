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
                User user = (User)Session["User"];

                // *** NEW LOGIC: Hide Regular Profile Panel for Admins ***
                if (user.IsAdmin)
                {
                    // If they are an Admin, we hide the regular user profile panel (pnlUser)
                    // but still show the Admin link (liAdmin).
                    pnlGuest.Visible = false;
                    pnlUser.Visible = false; // CRITICAL: Hide the regular user panel


                    // We stop here to prevent showing the regular user avatar dropdown.
                    return;
                }
                // *** END NEW LOGIC ***

                // --- Original Logic for Regular Users (IsAdmin = false) ---
                pnlGuest.Visible = false;
                pnlUser.Visible = true;


                // Bind avatar (only for regular users now)
                if (!string.IsNullOrEmpty(user.ProfilePicPath))
                {
                    imgAvatar.ImageUrl = user.ProfilePicPath;
                }
                else
                {
                    // Set a default placeholder if they haven't uploaded one
                    imgAvatar.ImageUrl = "~/Image/System/placeholder_profile.png";
                }
            }
            else
            {
                pnlGuest.Visible = true;
                pnlUser.Visible = false;
            }
        }
    }
}