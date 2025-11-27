using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    public class Phrase
    {
        public int PhraseID { get; set; }
        public string PhraseText { get; set; }
        public string Meaning { get; set; }
        public int SequenceOrder { get; set; }
        public bool isActivated { get; set; }
        // This will hold the list of details for this phrase
        public List<PhraseDetail> Details { get; set; }
    }
}