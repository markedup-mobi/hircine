namespace Hircine.Core
{
    /// <summary>
    /// Command object used to represent an index creation job requested by the client
    /// </summary>
    public class IndexBuildCommand
    {
        /// <summary>
        /// Tells the IndexJobManager to execute the result against an embedded database only (thus, ignoring any connection strings)
        /// </summary>
        public bool UseEmbedded { get; set; }

        /// <summary>
        /// Tells the IndexJobManager to only execute against one database at a time
        /// </summary>
        public bool ExecuteJobsSequentially { get; set; }

        /// <summary>
        /// If a single index build result comes back negative, the job will stop will stop if we're running sequentially.
        /// 
        /// This flag will override that behavior and continue building even if there are failures
        /// </summary>
        public bool ContinueJobOnFailure { get; set; }

        /// <summary>
        /// The set of all connection strings for each server we want to build against"
        /// </summary>
        public string[] ConnectionStrings { get; set; }

        /// <summary>
        /// The paths to each index-containing assembly we want to build against for each server
        /// </summary>
        public string[] AssemblyPaths { get; set; }
    }
}
