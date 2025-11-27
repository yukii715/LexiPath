using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexiPath.Data
{
    // This class is a "wrapper" that can hold either a Vocab or a Phrase
    public class LearningItem
    {
        public int ItemID { get; set; }
        public string ItemType { get; set; } // "Vocab" or "Phrase"
        public int SequenceOrder { get; set; }

        // Vocab properties
        public string VocabText { get; set; }
        public string VocabMeaning { get; set; }
        public string VocabImagePath { get; set; }

        // Phrase properties
        public string PhraseText { get; set; }
        public string PhraseMeaning { get; set; }
        public List<PhraseDetail> PhraseDetails { get; set; }
    }
}