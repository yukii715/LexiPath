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
                    lnkJoinHero.Visible = false;

                    lnkRegisterBtn.Text = "Go to My Dashboard";
                    lnkRegisterBtn.NavigateUrl = "~/Profile.aspx";
                }
            }
        }
    }
}