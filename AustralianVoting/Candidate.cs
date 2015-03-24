using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustralianVoting
{
    public class Candidate
    {
        public string Name { get; set; }
        public int ListPosition { get; set; }
        public List<List<int>> FirstPreferenceVoteList { get; set; }
        public int VoteCount { get { return FirstPreferenceVoteList.Count; } }
        public bool IsEliminated { get; set; }
    }
}
