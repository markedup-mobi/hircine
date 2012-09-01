using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Hircine.Core.Connectivity;
using Hircine.Core.Runtime;
using Raven.Client;
using Raven.Client.Document;

namespace Hircine.Core
{
    /// <summary>
    /// Manager class which accepts an IndexBuildCommand from a client and executes it
    /// </summary>
    public class IndexJobManager : IDisposable
    {
        private readonly IRavenInstanceFactory _ravenInstanceFactory;

        public IDictionary<string, IDocumentStore> RavenInstances { get; private set; }
        public IList<Assembly> IndexAssemblies { get; private set; }

        protected IndexBuildCommand BuildInstructions { get; private set; }

        public IndexJobManager(IndexBuildCommand command, IRavenInstanceFactory ravenInstanceFactory)
        {
            BuildInstructions = command;
            _ravenInstanceFactory = ravenInstanceFactory;

            RavenInstances = new Dictionary<string, IDocumentStore>();
            IndexAssemblies = new List<Assembly>();
        }

        public IndexJobManager(IndexBuildCommand command) : this(command, new DefaultRavenInstanceFactory()) { }

        /// <summary>
        /// Internal method for connecting to all of the specified RavenDB servers
        /// </summary>
        private void BuildDbInstances()
        {
            //If we haven't attempted to add to the contents of the collection yet
            if (RavenInstances.Count == 0)
            {
                //If we've been instructed to use an embedded instance
                if (BuildInstructions.UseEmbedded)
                {
                    var instance = _ravenInstanceFactory.GetEmbeddedInstance();
                    instance.Initialize();
                    RavenInstances.Add("Embedded", instance);

                }
                else //otherwise, build out all of the connections specified in the build instructions
                {
                    foreach (var connection in BuildInstructions.ConnectionStrings)
                    {
                        var instance = _ravenInstanceFactory.GetRavenConnection(connection);
                        instance.Initialize();
                        RavenInstances.Add(connection, instance);
                    }
                }
            }
        }

        /// <summary>
        /// Internal method for loading to all assemblies specified in the IndexBuildCommand
        /// </summary>
        private void LoadAssemblies()
        {
            //Only run this task if we haven't done it before
            if (IndexAssemblies.Count == 0)
            {
                foreach (var assemblyPath in BuildInstructions.AssemblyPaths)
                {

                }
            }
        }

        /// <summary>
        /// Validates that all of our assembly paths point to valid assemblies containing RavenDB index definitions
        /// </summary>
        /// <returns>A JobReport containing the results of each particular assembly path</returns>
        public JobReport CanLoadAssemblies()
        {
            var assemblyReport = new JobReport();

            foreach (var assemblyPath in BuildInstructions.AssemblyPaths)
            {
                var jobResult = new JobResult() { ResourceName = assemblyPath };

                //Check to see if we can locate the assembly in the GAC / filesystem
                if (AssemblyRuntimeLoader.CanFindAssembly(assemblyPath))
                {
                    //Check to see if the assembly has any indexes in it
                    if (AssemblyRuntimeLoader.HasRavenDbIndexes(AssemblyRuntimeLoader.LoadAssembly(assemblyPath)))
                    {
                        //Success!
                        jobResult.WasFound = true;
                    }else
                    {
                        //Fail - wasn't able to find any RavenDB indexes in this assembly
                        jobResult.WasFound = false;
                        jobResult.JobException = new InvalidOperationException(string.Format("Was able to load the assembly at {0}, but didn't find any RavenDB indexes", assemblyPath));
                    }
                }
                else //we were unable to find the assembly
                {
                    jobResult.WasFound = false;
                    jobResult.JobException = new FileNotFoundException(string.Format("Unable to find assembly located at {0}", assemblyPath));
                }
                
                assemblyReport.JobResults.Add(jobResult);
            }

            return assemblyReport;
        }

        /// <summary>
        /// Validates all of our connection strings before we run the job
        /// </summary>
        /// <returns>a JobReport containing the results of each particular connection string</returns>
        public JobReport CanConnectToDbs()
        {
            //If there are any errors with the connection string syntax themselves, those will be passed directly to the caller
            BuildDbInstances();

            var connectivityReport = new JobReport();

            foreach (var db in RavenInstances)
            {
                var connectivityResult = new JobResult() { ResourceName = db.Key };
                try
                {
                    //Attempt to open a session
                    var databaseStatistics = db.Value.DatabaseCommands.GetStatistics();

                    //See if we can get the store identifier
                    if (databaseStatistics != null)
                    {
                        connectivityResult.WasFound = true;
                    }
                }
                catch (Exception ex)
                {
                    //If there was an exception thrown here, it means there was probably something wrong with our database connection string
                    connectivityResult.WasFound = false;
                    connectivityResult.JobException = ex;
                }

                //Add the result of this particular connection attempt to the report
                connectivityReport.JobResults.Add(connectivityResult);
            }

            return connectivityReport;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            //Dispose all of the RavenDb instances if they haven't been already
            foreach (var db in RavenInstances)
            {
                //Check to see if the database has been disposed already
                if (!db.Value.WasDisposed)
                {
                    db.Value.Dispose();
                }
            }
        }

        #endregion
    }
}
