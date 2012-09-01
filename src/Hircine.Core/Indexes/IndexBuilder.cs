﻿using System;
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

        public IndexBuildReport Run(Action<IndexBuildResult> progressCallBack)
        {
            //Load our indexes
            var indexes = AssemblyRuntimeLoader.GetRavenDbIndexes(_indexAssembly);
            var tasks = new List<Task<IndexBuildResult>>();

            foreach (var index in indexes)
            {
                var indexInstance = (AbstractIndexCreationTask)Activator.CreateInstance(index);
                tasks.Add(Task.Factory.StartNew(() => indexInstance.Execute(_documentStore))
                                          .ContinueWith(result =>
                                                            {
                                                                var indexBuildResult = new IndexBuildResult() { IndexName = indexInstance.IndexName };

                                                                if (result.IsCompleted)
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
                                                                }

                                                                if (progressCallBack != null)
                                                                {
                                                                    progressCallBack.Invoke(indexBuildResult);
                                                                }

                                                                return indexBuildResult;
                                                            }));
            }

            //Wait out all of the tasks
            Task.WaitAll(tasks.ToArray());

            return new IndexBuildReport() {BuildResults = tasks.Select(x => x.Result).ToList()};
        }

        public Task<IndexBuildReport> RunAsync(Action<IndexBuildResult> progressCallBack)
        {
            //Load our indexes
            var indexes = AssemblyRuntimeLoader.GetRavenDbIndexes(_indexAssembly);
            var tasks = new List<Task<IndexBuildResult>>();

            foreach (var index in indexes)
            {
                var indexInstance = (AbstractIndexCreationTask)Activator.CreateInstance(index);

                //Asynchronously work on executing the index
                tasks.Add(indexInstance
                    .ExecuteAsync(_documentStore.AsyncDatabaseCommands, _documentStore.Conventions)
                    .ContinueWith(result =>
                                      {
                                          var indexBuildResult = new IndexBuildResult() { IndexName = indexInstance.IndexName };

                                          if (result.IsCompleted)
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
                                          }

                                          if (progressCallBack != null)
                                          {
                                              progressCallBack.Invoke(indexBuildResult);
                                          }

                                          return indexBuildResult;
                                      }));
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

        public void Dispose()
        {
            if (_documentStore != null)
                _documentStore.Dispose();
        }

        #endregion
    }
}
