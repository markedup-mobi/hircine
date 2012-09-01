using Raven.Client;

namespace Hircine.Core.Connectivity
{
    /// <summary>
    /// Interface used for our IDocumentStore / RavenDB instance Factory
    /// </summary>
    public interface IRavenInstanceFactory
    {
        /// <summary>
        /// Get a RavenDB instance for a database we access over HTTP
        /// </summary>
        /// <param name="connectionString">The RavenDB connection string itself</param>
        /// <returns>An UN-INITIALIZED (.Initialize() has not been called) RavenDB IDocumentStore for the provided connection string</returns>
        IDocumentStore GetRavenConnection(string connectionString);

        /// <summary>
        /// Gets an Embedded RavenDB instance
        /// </summary>
        /// <param name="runInMemory">Flag that can be set to determine if this instance is in-memory only or not. Defaults to true.</param>
        /// <returns>An UN-INITIALIZED (.Initialize() has not been called) Embedded RavenDB IDocumentStore</returns>
        IDocumentStore GetEmbeddedInstance(bool runInMemory = true);
    }
}
