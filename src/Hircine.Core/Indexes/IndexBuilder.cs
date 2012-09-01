using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Hircine.Core.Runtime;
using Raven.Client;
using Raven.Client.Indexes;

namespace Hircine.Core.Indexes
{
    /// <summary>
    /// Class responsible for managing an entire index building job against a database
    /// </summary>
    public class IndexBuilder : IDisposable
    {
        private readonly IDocumentStore _documentStore;
        private readonly Assembly _indexAssembly;

        public IndexBuilder(IDocumentStore store, Assembly indexAssembly)
        {
            _documentStore = store;
            _indexAssembly = indexAssembly;
        }

        /// <summary>
        /// Build a single index asynchronously
        /// </summary>
        /// <param name="indexInstance">An activated AbstractIndexCreationTask instance</param>
        /// <param name="progressCallBack">A callback we can use to report on the progress of a batch job; default is null</param>
        /// <returns>A task containing an IndexBuildResult report for this specific index</returns>
        public Task<IndexBuildResult> BuildIndexAsync(AbstractIndexCreationTask indexInstance, Action<IndexBuildResult> progressCallBack = null)
        {
            return Task.Factory.StartNew(() => indexInstance.Execute(_documentStore))
                .ContinueWith(result =>
                                  {
                                      var indexBuildResult = new IndexBuildResult()
                                                                 {IndexName = indexInstance.IndexName};

                                      if (result.IsCompleted && result.Exception == null)
                                      {
                                          indexBuildResult.Result = BuildResult.Success;
                                      }
                                      else if (result.IsCanceled)
                                      {
                                          indexBuildResult.Result = BuildResult.Cancelled;
                                      }
                                      else
                                      {
                                          indexBuildResult.Result = BuildResult.Failed;
                                          indexBuildResult.BuildException = result.Exception != null ? result.Exception.Flatten() : null;
                                      }

                                      if (progressCallBack != null)
                                      {
                                          progressCallBack.Invoke(indexBuildResult);
                                      }

                                      return indexBuildResult;
                                  });
        }

        /// <summary>
        /// Build a single index synchronously
        /// </summary>
        /// <param name="indexInstance">An activated AbstractIndexCreationTask instance</param>
        /// <returns>An IndexBuildResult report for this specific index</returns>
        public IndexBuildResult BuildIndex(AbstractIndexCreationTask indexInstance)
        {
            var buildIndexTask = BuildIndexAsync(indexInstance, null);

            buildIndexTask.Wait();

            return buildIndexTask.Result;
        }

        /// <summary>
        /// Synchronous method for running a batch index creation job against a database
        /// </summary>
        /// <param name="progressCallBack">callback method for reporting on the progress of the job</param>
        /// <returns>A completed IndexBuildReport covering all of the indexes found in the assembly</returns>
        public IndexBuildReport Run(Action<IndexBuildResult> progressCallBack)
        {
            var task = RunAsync(progressCallBack);

            //Wait out all of the tasks
            task.Wait();
            return task.Result;
        }

        /// <summary>
        /// Asynchronous method for running a batch index creation job against a database
        /// </summary>
        /// <param name="progressCallBack">callback method for reporting on the progress of the job</param>
        /// <returns>A task which returns a completed IndexBuildReport covering all of the indexes found in the assembly</returns>
        public Task<IndexBuildReport> RunAsync(Action<IndexBuildResult> progressCallBack)
        {
            //Load our indexes
            var indexes = AssemblyRuntimeLoader.GetRavenDbIndexes(_indexAssembly);
            var tasks = new List<Task<IndexBuildResult>>();

            foreach (var index in indexes)
            {
                var indexInstance = (AbstractIndexCreationTask)Activator.CreateInstance(index);
                tasks.Add(BuildIndexAsync(indexInstance, progressCallBack));
            }

            return Task.Factory.ContinueWhenAll(tasks.ToArray(), cont =>
            {
                var buildReport = new IndexBuildReport()
                {
                    BuildResults = tasks.Select(x => x.Result).ToList()
                };

                return buildReport;
            });
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Disposes the documentstore contained inside the IndexBuilder
        /// </summary>
        public void Dispose()
        {
            if (_documentStore != null)
                _documentStore.Dispose();
        }

        #endregion
    }
}
