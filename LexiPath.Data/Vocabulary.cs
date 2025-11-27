using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class Vocabulary
    {
        public int VocabID { get; set; }
        public string VocabText { get; set; }
        public string Meaning { get; set; }
        public string ImagePath { get; set; }
        public int SequenceOrder { get; set; }
        public bool isActivated { get; set; }
    }
}
