using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hircine.Core.Indexes
{

    /// <summary>
    /// Reports on the success or failure of a batch index build job
    /// </summary>
    public class IndexBuildReport
    {
        /// <summary>
        /// The reports for each individual index
        /// </summary>
        public List<IndexBuildResult> BuildResults { get; set; }

        /// <summary>
        /// Total number of indexes successfully created
        /// </summary>
        public int Completed
        {
            get { return BuildResults.Count(x => x.Result == BuildResult.Success); }
        }

        /// <summary>
        /// Index build jobs which were cancelled prior to completion
        /// </summary>
        public int Cancelled
        {
            get { return BuildResults.Count(x => x.Result == BuildResult.Cancelled); }
        }

        /// <summary>
        /// Index build jobs which failed
        /// </summary>
        public int Failed
        {
            get { return BuildResults.Count(x => x.Result == BuildResult.Failed); }
        }

        public IndexBuildReport()
        {
            BuildResults = new List<IndexBuildResult>();
        }
    }

    /// <summary>
    /// Success report for the creation of a single index
    /// </summary>
    public class IndexBuildResult
    {
        /// <summary>
        /// Name of the connection string for the current database
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the index being built (determined by RavenDB and the AbstractIndexCreationTask definition)
        /// </summary>
        public string IndexName { get; set; }

        /// <summary>
        /// The completed state of the task
        /// </summary>
        public BuildResult Result { get; set; }

        /// <summary>
        /// Any exceptions which were raised in the course of building the index
        /// </summary>
        public Exception BuildException { get; set; }
    }

    public enum BuildResult
    {
        Success = 0,
        Failed = 1,
        Cancelled = 2
    };
}
