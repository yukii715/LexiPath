using LexiPath.Data;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath.Admin
{
    // *** IMPORTANT: Change inheritance to AdminBasePage ***
    public partial class ManageUsers : AdminBasePage
    {
        private UserManager userManager = new UserManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        /**
         * Fills the GridView with all users from the database
         */
        private void BindGrid()
        {
            gvUsers.DataSource = userManager.GetAllUsers();
            gvUsers.DataBind();
        }

        /**
         * This event fires when any button inside the GridView is clicked
         */
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Check if it was our "ToggleStatus" button
            if (e.CommandName == "ToggleStatus")
            {
                try
                {
                    // Get the UserID from the button's CommandArgument
                    int userId = Convert.ToInt32(e.CommandArgument);

                    // Call our manager to flip the status
                    userManager.ToggleUserStatus(userId);

                    // Refresh the grid to show the change
                    BindGrid();
                }
                catch (Exception ex)
                {
                    // TODO: Show an error message
                }
            }
        }
    }
}