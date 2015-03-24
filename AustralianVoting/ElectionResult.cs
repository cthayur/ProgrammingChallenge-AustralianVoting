using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustralianVoting
{
    public class ElectionResult
    {
        public int ElectionNumber { get; set; }
        public IEnumerable<Candidate> Winners { get; set; }
        
        public override string ToString()
        {
            var sbResult = new StringBuilder();

            sbResult.Append("Election #: " + this.ElectionNumber + System.Environment.NewLine);
            sbResult.Append("-------------------------------------------------" + System.Environment.NewLine);

            foreach(var winner in this.Winners)
            {
                sbResult.Append(winner.Name + System.Environment.NewLine);
            }

            sbResult.Append("-------------------------------------------------" + System.Environment.NewLine);
            
            
            return sbResult.ToString();
        }
    }
}
