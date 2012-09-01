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
            var connectionString = "http://localhost:8080";

            var parsedConnectionString = 
        }

        #endregion
    }
}
