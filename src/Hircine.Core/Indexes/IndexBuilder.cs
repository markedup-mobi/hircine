using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Hircine.Core.Connectivity;
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

        private readonly Assembly[] _assemblies;

        private readonly string _connectionString;

        public IndexBuilder(IDocumentStore store, Assembly[] indexAssemblies) : this(store, indexAssemblies, string.Empty)
        {
        }

        public IndexBuilder(IDocumentStore store, Assembly indexAssembly)
            : this(store, new[] { indexAssembly })
        {
        }

        public IndexBuilder(IDocumentStore store, Assembly[] indexAssemblies, string connectionString)
        {
            _documentStore = store;
            _assemblies = indexAssemblies;
            _connectionString = connectionString;
        }

        public IndexBuilder(IDocumentStore store, Assembly indexAssembly, string connectionString) : this(store, new[]{indexAssembly}, connectionString)
        {
        }

        public void StartIndexing(Action<IndexBuildResult> progressCallBack = null)
        {
            var indexBuildResult = new IndexBuildResult() { IndexName = "StartIndexing", ConnectionString = _documentStore.Identifier };

            if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrWhiteSpace(_connectionString))
            {
                if (progressCallBack != null)
                {
                    indexBuildResult.Result = BuildResult.Cancelled;
                    indexBuildResult.BuildException = new Exception("No connection string provided");
                    progressCallBack.Invoke(indexBuildResult);
                }

                return;
            }
 
            //If RavenDB finds any connection string errors it will throw them here, and we will pass that back to the client as is.
            var connectionStringOptions = RavenConnectionStringParser.ParseNetworkedDbOptions(_connectionString);
            try
            {
                using (var webClient = new WebClient())
                {
                    if (connectionStringOptions.Credentials == null)
                    {
                        webClient.UseDefaultCredentials = true;
                    }
                    else
                    {
                        webClient.Credentials = connectionStringOptions.Credentials;
                    }
                    var result = webClient.UploadString(new Uri(new Uri(_documentStore.Url), "/admin/startindexing"), "POST", "");

                    indexBuildResult.Result = BuildResult.Success;
                    if (progressCallBack != null)
                    {
                        progressCallBack.Invoke(indexBuildResult);
                    }
                }
            }
            catch (Exception e)
            {
                if (progressCallBack != null)
                {
                    indexBuildResult.Result = BuildResult.Failed;
                    indexBuildResult.BuildException = e;
                    progressCallBack.Invoke(indexBuildResult);
                }
            }
        }

        public void StopIndexing(Action<IndexBuildResult> progressCallBack = null)
        {
            var indexBuildResult = new IndexBuildResult() { IndexName = "StopIndexing", ConnectionString = _documentStore.Identifier };

            if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrWhiteSpace(_connectionString))
            {
                if (progressCallBack != null)
                {
                    indexBuildResult.Result = BuildResult.Cancelled;
                    indexBuildResult.BuildException = new Exception("No connection string provided");
                    progressCallBack.Invoke(indexBuildResult);
                }

                return;
            }

            //If RavenDB finds any connection string errors it will throw them here, and we will pass that back to the client as is.
            var connectionStringOptions = RavenConnectionStringParser.ParseNetworkedDbOptions(_connectionString);
            try
            {
                using (var webClient = new WebClient())
                {
                    if (connectionStringOptions.Credentials == null)
                    {
                        webClient.UseDefaultCredentials = true;
                    }
                    else
                    {
                        webClient.Credentials = connectionStringOptions.Credentials;
                    }
                    var result = webClient.UploadString(new Uri(new Uri(_documentStore.Url), "/admin/stopindexing"), "POST", "");

                    indexBuildResult.Result = BuildResult.Success;
                    if (progressCallBack != null)
                    {
                        progressCallBack.Invoke(indexBuildResult);
                    }
                }
            }
            catch (Exception e)
            {
                if (progressCallBack != null)
                {
                    indexBuildResult.Result = BuildResult.Failed;
                    indexBuildResult.BuildException = e;
                    progressCallBack.Invoke(indexBuildResult);
                }
            }
        }

        /// <summary>
        /// Builds the full set of indexes we're going to build from all of the designated assemblies
        /// </summary>
        /// <returns>A list of Types derived from AbstractIndexCreationTask</returns>
        public IList<Type> GetIndexesFromLoadedAssemblies()
        {
            var indexes = new List<Type>();
            foreach(var assembly in _assemblies)
            {
                indexes = indexes.Concat(AssemblyRuntimeLoader.GetRavenDbIndexes(assembly)).ToList();
            }
            return indexes;
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
                                                                 {IndexName = indexInstance.IndexName, ConnectionString = _documentStore.Identifier};

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
            var indexes = GetIndexesFromLoadedAssemblies();
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
            if (_documentStore != null && !_documentStore.WasDisposed)
                _documentStore.Dispose();
        }

        #endregion
    }
}
