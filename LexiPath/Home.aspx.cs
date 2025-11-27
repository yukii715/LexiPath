using System;
using System.Web.UI;
using LexiPath.Data;

namespace LexiPath
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["User"] != null)
                {
                    // User is logged in
                    // 1. Hide "Join" in Hero
                    lnkJoinHero.Visible = false;

                    // 2. Change "Create Account" in Benefits section to "Go to Profile"
                    lnkRegisterBtn.Text = "Go to My Dashboard";
                    lnkRegisterBtn.NavigateUrl = "~/Profile.aspx";
                }
            }
        }
    }
}