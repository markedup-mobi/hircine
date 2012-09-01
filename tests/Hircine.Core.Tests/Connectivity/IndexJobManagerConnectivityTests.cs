using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Hircine.Core.Tests.Connectivity
{
    [TestFixture(Description = "Test fixture for asserting that our ")]
    public class IndexJobManagerConnectivityTests
    {
        #region Setup / Teardown
        #endregion

        #region Tests

        [Test(Description = "Should be able to establish and accurately report a connection to an embedded database")]
        public void Should_Have_Connection_to_Embedded_Database()
        {
            var commandObject = new IndexBuildCommand()
                                    {
                                        AssemblyPaths = new string[] {TestHelper.ValidTestAssemblyPath},
                                        UseEmbedded = true
                                    };

            var indexManager = new IndexJobManager(commandObject);

            try
            {
                //Attempt to connect to our databases
                var connectionReport = indexManager.CanConnectToDbs();
                Assert.AreEqual(1, connectionReport.ConnectivityResults.Count);
                Assert.AreEqual(connectionReport.ConnectivityResults.Count, connectionReport.SuccessfulConnections);
            }catch(InvalidOperationException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexManager.Dispose();
            }
        }

        #endregion
    }
}
