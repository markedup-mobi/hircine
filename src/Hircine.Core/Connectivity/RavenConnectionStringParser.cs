using Raven.Abstractions.Data;

namespace Hircine.Core.Connectivity
{
    /// <summary>
    /// Static utility class designed to simplify the current awkwardness around parsing RavenDB connection strings.
    /// </summary>
    public static class RavenConnectionStringParser
    {
        public static RavenConnectionStringOptions ParseNetworkedDbOptions(string connectionString)
        {
            var connectionStringParser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);
            connectionStringParser.Parse();
            return connectionStringParser.ConnectionStringOptions;
        }
    }
}
