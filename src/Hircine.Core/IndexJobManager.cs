using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hircine.Core.Connectivity;
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

        public IndexJobManager(IndexBuildCommand command) : this(command, new DefaultRavenInstanceFactory()){}

        /// <summary>
        /// Internal method for connecting to all of the specified RavenDB servers
        /// </summary>
        private void BuildDbInstances()
        {
            //If we haven't attempted to add to the contents of the collection yet
            if(RavenInstances.Count == 0)
            {
                //If we've been instructed to use an embedded instance
                if(BuildInstructions.UseEmbedded)
                {
                    var instance = _ravenInstanceFactory.GetEmbeddedInstance();
                    instance.Initialize();
                    RavenInstances.Add("Embedded", instance);

                } else //otherwise, build out all of the connections specified in the build instructions
                {
                    foreach(var connection in BuildInstructions.ConnectionStrings)
                    {
                        var instance =_ravenInstanceFactory.GetRavenConnection(connection);
                        instance.Initialize();
                        RavenInstances.Add(connection, instance);
                    }
                }
            }
        }

        /// <summary>
        /// Validates all of our connection strings before we run the job
        /// </summary>
        /// <returns>true if we were able to connect to all of the RavenDb instances</returns>
        public ConnectivityReport CanConnectToDbs()
        {
            //If there are any errors with the connection string syntax themselves, those will be passed directly to the caller
            BuildDbInstances();

            var connectivityReport = new ConnectivityReport();

            foreach(var db in RavenInstances)
            {
                var connectivityResult = new ConnectivityResult(){ConnectionString = db.Key};
                try
                {
                    //Attempt to open a session
                    var databaseStatistics = db.Value.DatabaseCommands.GetStatistics();

                    //See if we can get the store identifier
                    if(databaseStatistics != null)
                    {
                        connectivityResult.CanConnect = true;
                    }
                }catch(Exception ex)
                {
                    //If there was an exception thrown here, it means there was probably something wrong with our database connection string
                    connectivityResult.CanConnect = false;
                    connectivityResult.ConnectivityException = ex;
                }

                //Add the result of this particular connection attempt to the report
                connectivityReport.ConnectivityResults.Add(connectivityResult);
            }

            return connectivityReport;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            //Dispose all of the RavenDb instances if they haven't been already
            foreach(var db in RavenInstances)
            {
                //Check to see if the database has been disposed already
                if(!db.Value.WasDisposed)
                {
                    db.Value.Dispose();
                }
            }
        }

        #endregion
    }
}
