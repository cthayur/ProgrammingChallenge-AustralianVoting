using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustralianVoting
{
    public class ElectionAnalyzer
    {
        int ElectionNumber = 0;
        int NumOfCandidates = 0;
        int TotalVoters = 0;
        List<string> ElectionInfoLines;
        List<Candidate> CandidateArray;
        List<Candidate> Winners;

        /// <summary>
        /// Required votes set to 50% of total voters
        /// </summary>
        double RequiredVotesToWin { get { return 0.5 * TotalVoters; } }

        /// <summary>
        /// Indicates if there is a single winner with votes > 50%
        /// Or if there are two with 50% each
        /// </summary>
        bool AnyClearWinners { get { return CandidateArray.Any(x => x.VoteCount >= RequiredVotesToWin); } }

        /// <summary>
        /// Returns a list of candidate winners
        /// </summary>
        IEnumerable<Candidate> GetClearWinners { get { return CandidateArray.Where(y => y.VoteCount >= RequiredVotesToWin); } }

        public ElectionAnalyzer(List<string> electionInfoLines, int electionNumber)
        {
            this.ElectionInfoLines = electionInfoLines;
            this.CandidateArray = new List<Candidate>();
            this.ElectionNumber = electionNumber;
        }

        /// <summary>
        /// Main entry point for analysis of an election
        /// </summary>
        /// <returns></returns>
        public ElectionResult GetWinner()
        {
            IdentifyBasicInfo();

            SetupCandidateArray();

            if (AnyClearWinners)
            {
                return GetElectionResult(GetClearWinners);
            }
            else
            {
               return SettleTies();
            }            
        }

        /// <summary>
        /// Responsible for:
        /// 1. Eliminating the lowest ranked candidates
        /// 2. Assigning their votes to the next preferred non eliminated candidate
        /// 3. Perform this operation recursively till there is a clear winner
        /// </summary>
        /// <returns></returns>
        private ElectionResult SettleTies()
        {
            var minVote = CandidateArray.Min(x => x.VoteCount);
            var eliminatedCandidates = CandidateArray.Where(y => y.VoteCount == minVote).Select(z => { z.IsEliminated = true; return z; }).ToList();            
            
            //Remove all eleminated candidates
            CandidateArray.RemoveAll(a => a.VoteCount == minVote);

            //If only one candidate remains, that person is the winner
            if(CandidateArray.Count == 1)
            {
                return GetElectionResult(CandidateArray);
            }

            //Identify who remains in the election
            var remainingPositions = CandidateArray.Select(b => b.ListPosition);

            //For ever eliminated candidate
            foreach (var eleminatedCandidate in eliminatedCandidates)
            {
                //Remove the first vote since its for self
                eleminatedCandidate.FirstPreferenceVoteList.ForEach(x => x.RemoveAt(0));

                //Loop throug every prefrence list of the eliminated candidate
                foreach (var vote in eleminatedCandidate.FirstPreferenceVoteList)
                {
                    //Get a list of preference votes with the first preference being one of the candidates not eliminated
                    var validVoteList = GetValidVoteList(vote, remainingPositions);

                    //If we get back a list, it means we have found the next best preference in this series of preferences for a valid candidate
                    if (validVoteList.Count > 0)
                    {
                        //Find the preferred candidate and add this list to their preference vote list
                        CandidateArray.Where(x => x.ListPosition == validVoteList[0]).First().FirstPreferenceVoteList.Add(validVoteList);
                    }
                }
            }

            if (AnyClearWinners)
            {
                return GetElectionResult(GetClearWinners);
            }
            else
            {
                return SettleTies();
            }
        }

        /// <summary>
        /// Returns a formatted ElectionResult
        /// </summary>
        /// <param name="winners"></param>
        /// <returns></returns>
        private ElectionResult GetElectionResult(IEnumerable<Candidate> winners)
        {
            return new ElectionResult { Winners = winners, ElectionNumber = ElectionNumber };
        }

        /// <summary>
        /// Gets a pared down list of preferrence vote list depending on if the candidate in that list has been eliminated or not
        /// </summary>
        /// <param name="votes"></param>
        /// <param name="remainingPositions"></param>
        /// <returns></returns>
        private List<int> GetValidVoteList(List<int> votes, IEnumerable<int> remainingPositions)
        {
            if(remainingPositions.Contains(votes[0]))
            {
                return votes;
            }
            else
            {
                votes.RemoveAt(0);
                return GetValidVoteList(votes, remainingPositions);
            }
        }

        /// <summary>
        /// Sets up NumOfCandidates and TotalVoters
        /// </summary>
        private void IdentifyBasicInfo()
        {
            NumOfCandidates = Convert.ToInt32(ElectionInfoLines[0]);
            TotalVoters = ElectionInfoLines.Count - NumOfCandidates - 1;
        }

        /// <summary>
        /// Creates new candidate objects and adds appropriate voting preference lists for each candidate based on first preference
        /// </summary>
        private void SetupCandidateArray()
        {
            for (var i = 1; i < ElectionInfoLines.Count; i++)
            {
                if (i < (NumOfCandidates + 1))
                {
                    CandidateArray.Add(new Candidate { Name = ElectionInfoLines[i], ListPosition = i, FirstPreferenceVoteList = new List<List<int>>() });
                }
                else
                {
                    var voteArray = ElectionInfoLines[i].Split(' ').Select(vote => Convert.ToInt32(vote)).ToList();
                    CandidateArray.First(candidate => candidate.ListPosition == voteArray.First()).FirstPreferenceVoteList.Add(voteArray);
                }
            }
        }        
    }
}
