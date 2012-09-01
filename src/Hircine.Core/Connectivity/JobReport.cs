using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.Core.Connectivity
{
    public class JobReport
    {
        public IList<JobResult> JobResults { get; set; }
        public int Successes {get { return JobResults.Count(x => x.WasFound); }}
        public int Failures { get { return JobResults.Count(x => !x.WasFound); } }

        public JobReport()
        {
            JobResults = new List<JobResult>();
        }

    }

    public class JobResult
    {
        public string ResourceName { get; set; }
        public bool WasFound { get; set; }
        public Exception JobException { get; set; }
    }
}
