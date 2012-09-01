using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.Core.Indexes
{
    public class IndexBuildReport
    {
        public List<IndexBuildResult> BuildResults { get; set; }
        public int Completed
        {
            get { return BuildResults.Count(x => x.Result == BuildResult.Success); }
        }

        public int Cancelled
        {
            get { return BuildResults.Count(x => x.Result == BuildResult.Cancelled); }
        }

        public int Failed
        {
            get { return BuildResults.Count(x => x.Result == BuildResult.Failed); }
        }

        public IndexBuildReport()
        {
            BuildResults = new List<IndexBuildResult>();
        }
    }

    public class IndexBuildResult
    {
        public string IndexName { get; set; }
        public BuildResult Result { get; set; }
        public Exception BuildException { get; set; }
    }

    public enum BuildResult
    {
        Success = 0,
        Failed = 1,
        Cancelled = 2
    };
}
