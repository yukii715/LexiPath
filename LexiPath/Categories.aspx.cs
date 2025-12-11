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
        private int currentLanguageId = 1; // Default to Korean
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LanguageID"] != null)
            {
                currentLanguageId = (int)Session["LanguageID"];
            }

            if (!IsPostBack)
            {
                BindCategories();
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindCategories();
        }

        private void BindCategories()
        {
            string searchTerm = txtSearch.Text.Trim();

            CourseManager manager = new CourseManager();
            
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