using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DashTechCRM.Models.Marketing
{
    public class AssignedCandidateObject
    {
        public CandidateAssign candidateAssign { get; set; }
        public CandidateMaster candidateMaster { get; set; }
        public CandidateMarketingDetail candidateMarketing { get; set; }
        public TeamDetail teamDetail { get; set; }
    }
}