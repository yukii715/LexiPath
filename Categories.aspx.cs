using LexiPath.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LexiPath
{
    public partial class Categories : System.Web.UI.Page
    {
        // Class-level variable to hold the chosen language
        private int currentLanguageId = 1; // Default to Korean
        protected void Page_Load(object sender, EventArgs e)
        {
            // --- NEW: Read the LanguageID from the Session ---
            if (Session["LanguageID"] != null)
            {
                currentLanguageId = (int)Session["LanguageID"];
            }

            if (!IsPostBack)
            {
                BindCategories();
            }
        }

        // NEW: This event fires when the search button is clicked
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // We just re-run the BindCategories method.
            // It will now read the text from the txtSearch box.
            BindCategories();
        }

        private void BindCategories()
        {
            // 1. Get the search term from the textbox
            string searchTerm = txtSearch.Text.Trim();

            // 2. Pass the search term to the manager
            CourseManager manager = new CourseManager();
            // --- UPDATED: Pass the LanguageID to the manager ---
            rptCategories.DataSource = manager.GetAllCategories(searchTerm, currentLanguageId);
            rptCategories.DataBind();
        }

        // Helper function to handle missing images
        public string GetImagePath(object imagePath)
        {
            if (imagePath != null && !string.IsNullOrEmpty(imagePath.ToString()))
            {
                return imagePath.ToString();
            }
            else
            {
                // Return a path to a default placeholder image
                return "/Image/System/placeholder.png";
            }
        }
    }
}