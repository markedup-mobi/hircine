using System;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;

namespace Hircine.Core.Connectivity
{
    public class DefaultRavenInstanceFactory : IRavenInstanceFactory
    {
        #region Implementation of IRavenInstanceFactory

        public IDocumentStore GetRavenConnection(string connectionString)
        {
            //If RavenDB finds any connection string errors it will throw them here, and we will pass that back to the client as is.
            var connectionStringOptions = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            //create a new document store from the connection string
            return new DocumentStore()
                       {
                           ApiKey = connectionStringOptions.ApiKey,
                           Credentials = connectionStringOptions.Credentials,
                           Url = connectionStringOptions.Url,
                           EnlistInDistributedTransactions = connectionStringOptions.EnlistInDistributedTransactions,
                           DefaultDatabase = connectionStringOptions.DefaultDatabase,
                           ResourceManagerId = connectionStringOptions.ResourceManagerId
                       };
        }

        public IDocumentStore GetEmbeddedInstance(bool runInMemory = true)
        {
            return new EmbeddableDocumentStore(){RunInMemory = runInMemory};
        }

        #endregion
    }
}