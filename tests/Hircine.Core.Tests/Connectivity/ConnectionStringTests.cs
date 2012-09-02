using Hircine.Core.Connectivity;
using NUnit.Framework;

namespace Hircine.Core.Tests.Connectivity
{
    [TestFixture(Description = "Test fixture which asserts that our RavenDB connection string parser behaves as expected for the defaults we show on the command-line")]
    public class ConnectionStringTests
    {
        #region Setup / Teardown



        #endregion

        #region Tests

        [Test(Description = "Should accept a connection string that has just a Url in it")]
        public void Should_Accept_ConnectionString_with_Just_Url()
        {
            //valid connection string
            var connectionStringUrl = "http://localhost:8080";
            var connectionString = RavenConnectionStringBuilder.BuildConnectionString(connectionStringUrl);

            var parsedConnectionString = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            Assert.AreEqual(connectionStringUrl, parsedConnectionString.Url);
        }

        [Test(Description = "Should accept a connection string that has both a Url and an API key specified - typically used for RavenHQ")]
        public void Should_Accept_ConnectionString_with_Url_and_ApiKey()
        {
            //valid connection string
            var connectionStringUrl = "http://localhost:8080";
            var apiKey = "45034-32330-epic-api-key-action-right-hur";
            var connectionString = RavenConnectionStringBuilder.BuildConnectionStringWithApiKey(connectionStringUrl, apiKey);

            var parsedConnectionString = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            Assert.AreEqual(connectionStringUrl, parsedConnectionString.Url);
            Assert.AreEqual(apiKey, parsedConnectionString.ApiKey);
        }

        [Test(Description = "Should accept a connection string that has both a url and a default database specified")]
        public void Should_Accept_ConnectionString_with_Url_and_DefaultDatabase()
        {
            //valid connection string
            var connectionStringUrl = "http://localhost:8080";
            var defaultDb = "magicalDb";
            var connectionString = RavenConnectionStringBuilder.BuildConnectionString(connectionStringUrl, defaultDb);

            var parsedConnectionString = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            Assert.AreEqual(connectionStringUrl, parsedConnectionString.Url);
            Assert.AreEqual(defaultDb, parsedConnectionString.DefaultDatabase);
        }

        [Test(Description = "Should be able to parse a connection string that has both a username and password in it in addition to the Url")]
        public void Should_Accept_ConnectionString_with_Url_and_UserNamePassword()
        {
            //valid connection string
            var connectionStringUrl = "http://localhost:8080";
            var userName = "magicalUser";
            var password = "magicalPassword";

            var connectionString = RavenConnectionStringBuilder.BuildConnectionString(connectionStringUrl, userName, password);

            var parsedConnectionString = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            Assert.AreEqual(connectionStringUrl, parsedConnectionString.Url);
            Assert.AreEqual(userName, parsedConnectionString.Credentials.UserName);
            Assert.AreEqual(password, parsedConnectionString.Credentials.Password);
        }

        [Test(Description = "Should accept a connection sting that has a Url, a username / password, and a default database specified")]
        public void Should_Accept_ConnectionString_with_Url_UserNamePassword_and_DefaultDb()
        {
            //valid connection string
            var connectionStringUrl = "http://localhost:8080";
            var userName = "magicalUser";
            var password = "magicalPassword";
            var defaultDb = "magicalDb";

            var connectionString = RavenConnectionStringBuilder.BuildConnectionString(connectionStringUrl, userName, password, defaultDb);

            var parsedConnectionString = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            Assert.AreEqual(connectionStringUrl, parsedConnectionString.Url);
            Assert.AreEqual(userName, parsedConnectionString.Credentials.UserName);
            Assert.AreEqual(password, parsedConnectionString.Credentials.Password);
            Assert.AreEqual(defaultDb, parsedConnectionString.DefaultDatabase);
        }

        #endregion
    }
}
