using LexiPath.Data;
using System;
using System.Web.UI;

namespace LexiPath.Admin
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // This logic shows the avatar in the new top-nav
            if (Session["User"] != null)
            {
                pnlUser.Visible = true;
                User user = (User)Session["User"];

                if (!string.IsNullOrEmpty(user.ProfilePicPath))
                {
                    imgAvatar.ImageUrl = user.ProfilePicPath;
                }
                else
                {
                    imgAvatar.ImageUrl = "/Image/System/placeholder_profile.png";
                }
            }
            else
            {
                // This shouldn't happen because of AdminBasePage,
                // but it's good security.
                pnlUser.Visible = false;
            }
        }
    }
}