using Hircine.Core.Connectivity;
using NUnit.Framework;

namespace Hircine.Core.Tests.Connectivity
{
    [TestFixture(Description = "Test fixture which asserts that our RavenDB connection string parser behaves as expected for the defaults we show on the command-line")]
    public class ConnectionStringParsingTests
    {
        #region Setup / Teardown



        #endregion

        #region Tests

        [Test(Description = "Should accept a connection string that has just a Url in it")]
        public void Should_Accept_ConnectionString_with_Just_Url()
        {
            //valid connection string
            var connectionStringUrl = "http://localhost:8080";
            var connectionString = string.Format("Url={0}", connectionStringUrl);

            var parsedConnectionString = RavenConnectionStringParser.ParseNetworkedDbOptions(connectionString);

            Assert.AreEqual(connectionStringUrl, parsedConnectionString.Url);
        }

        #endregion
    }
}
