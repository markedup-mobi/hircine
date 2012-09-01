using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Hircine.Core.Tests.Runtime
{
    [TestFixture(Description = "Test fixture for asserting that our IndexJobManager can properly load and verify valid assemblies containing RavenDB index definitions")]
    public class IndexJobManagerAssemblyLoadTests
    {
        #region Setup / Teardown
        #endregion

        #region Tests

        [Test(Description = "Should report a successful load event for a valid assembly that actually has RavenDB index definitions")]
        public void Should_Report_Successful_Load_For_Valid_Assembly_Containing_RavenDB_Index_Definitions()
        {
            var commandObject = new IndexBuildCommand()
            {
                AssemblyPaths = new string[] { TestHelper.ValidTestAssemblyPath },
                UseEmbedded = true
            };

            var indexManager = new IndexJobManager(commandObject);
            try
            {
                var assemblyLoadReport = indexManager.CanLoadAssemblies();
                Assert.AreEqual(commandObject.AssemblyPaths.Count(), assemblyLoadReport.JobResults.Count);
                Assert.AreEqual(commandObject.AssemblyPaths.Count(), assemblyLoadReport.Successes);
            }catch(InvalidOperationException ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexManager.Dispose();
            }
        }

        [Test(Description = "Should report an unsuccessful load job when we attempt to load an assembly that does not exist")]
        public void Should_Report_Unsuccessful_Load_For_NonExistant_Assembly()
        {
            var commandObject = new IndexBuildCommand()
            {
                AssemblyPaths = new string[] { TestHelper.InvalidAssemblyPath },
                UseEmbedded = true
            };

            var indexManager = new IndexJobManager(commandObject);
            try
            {
                var assemblyLoadReport = indexManager.CanLoadAssemblies();
                Assert.AreEqual(commandObject.AssemblyPaths.Count(), assemblyLoadReport.JobResults.Count);
                Assert.AreEqual(0, assemblyLoadReport.Successes);
                Assert.AreEqual(commandObject.AssemblyPaths.Count(), assemblyLoadReport.Failures);
                Assert.IsNotNull(assemblyLoadReport.JobResults.First().JobException);
                Assert.IsInstanceOf<FileNotFoundException>(assemblyLoadReport.JobResults.First().JobException);
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

        [Test(Description = "Should report an unsuccessful load job when we attempt to load an assembly that exists, but has no index definitions")]
        public void Should_Report_Unsuccessful_Load_For_Assembly_with_No_Index_Definitions()
        {
            var commandObject = new IndexBuildCommand()
            {
                AssemblyPaths = new string[] { "System.Diagnostics" },
                UseEmbedded = true
            };

            var indexManager = new IndexJobManager(commandObject);
            try
            {
                var assemblyLoadReport = indexManager.CanLoadAssemblies();
                Assert.AreEqual(commandObject.AssemblyPaths.Count(), assemblyLoadReport.JobResults.Count);
                Assert.AreEqual(0, assemblyLoadReport.Successes);
                Assert.AreEqual(commandObject.AssemblyPaths.Count(), assemblyLoadReport.Failures);
                Assert.IsNotNull(assemblyLoadReport.JobResults.First().JobException);
                Assert.IsInstanceOf<InvalidOperationException>(assemblyLoadReport.JobResults.First().JobException);
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
