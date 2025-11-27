using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        public int LanguageID { get; set; }
        public bool isActivated { get; set; }
    }
}
