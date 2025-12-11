using System;
using LexiPath.Data;
using System.Web.UI;

namespace LexiPath
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["lang"]))
            {
                int langId = 0;
                if (int.TryParse(Request.QueryString["lang"], out langId))
                {
                    Session["LanguageID"] = langId;
                }

                Response.Redirect(Request.Path);
                return;
            }

            if (Session["LanguageID"] == null)
            {
                Session["LanguageID"] = 1; // Default to Korean (ID 1)
            }

            if ((int)Session["LanguageID"] == 1)
            {
                litCurrentLanguage.Text = "Korean";
            }
            else
            {
                litCurrentLanguage.Text = "English";
            }

            if (Session["User"] != null)
            {
                User user = (User)Session["User"];

                if (user.IsAdmin)
                {
                    pnlGuest.Visible = false;
                    pnlUser.Visible = false; 

                    return;
                }
                
                pnlGuest.Visible = false;
                pnlUser.Visible = true;

                if (!string.IsNullOrEmpty(user.ProfilePicPath))
                {
                    imgAvatar.ImageUrl = user.ProfilePicPath;
                }
                else
                {
                    imgAvatar.ImageUrl = "~/Image/System/placeholder_profile.png"; // Default avatar
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