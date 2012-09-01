using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hircine.Core.Connectivity;
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
        public void Should_Report_Successful_Connection_to_Embedded_Database()
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

        [Test(Description = "Should report that the connection was unsuccessful to a database that does not exist")]
        public void Should_Report_Unsuccessful_Connection_to_Nonexistant_Database()
        {
            //Create a connection string using a random guid as the name of a local database
            var connectionString = RavenConnectionStringBuilder.BuildConnectionString("http://localhost:8080",
                                                                                      Guid.NewGuid().ToString());
            //Build our command object
            var commandObject = new IndexBuildCommand()
            {
                AssemblyPaths = new string[] { TestHelper.ValidTestAssemblyPath },
                ConnectionStrings = new string[]{connectionString}
            };

            var indexManager = new IndexJobManager(commandObject);

            try
            {
                //Attempt to connect to our databases
                var connectionReport = indexManager.CanConnectToDbs();
                Assert.AreEqual(1, connectionReport.ConnectivityResults.Count);
                Assert.AreEqual(0, connectionReport.SuccessfulConnections);
                Assert.AreEqual(1, connectionReport.FailedConnections);
            }
            catch (InvalidOperationException ex)
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
