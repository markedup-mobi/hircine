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
        public bool ExecuteJobsSynchronously { get; set; }

        /// <summary>
        /// If a single index build result comes back negative, the job will stop will stop if we're running synchronously
        /// </summary>
        public bool TermineJobOnFailure { get; set; }

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
