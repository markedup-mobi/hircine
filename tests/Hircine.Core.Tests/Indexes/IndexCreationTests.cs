using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Hircine.Core.Connectivity;
using Hircine.Core.Indexes;
using Hircine.Core.Runtime;
using NUnit.Framework;

namespace Hircine.Core.Tests.Indexes
{
    [TestFixture(Description = "Test fixture used for verifying that we can successfully create indexes using IndexBuilder")]
    public class IndexCreationTests
    {
        private IRavenInstanceFactory _ravenInstanceFactory;
        private Assembly _indexAssembly;

        #region Setup / Teardown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _ravenInstanceFactory = new DefaultRavenInstanceFactory();
            _indexAssembly = AssemblyRuntimeLoader.LoadAssembly(TestHelper.ValidTestAssemblyPath);
        }

        #endregion

        #region Tests

        [Test(Description = "Should be able to build our valid RavenDB indexes against an in-memory test database")]
        public void Should_Synchronously_Create_Indexes_Against_Embedded_Database()
        {
            //Assert one pre-condition: must have n > 0 indexes in the assembly before we begin
            var numberOfTargetIndexes = AssemblyRuntimeLoader.GetRavenDbIndexes(_indexAssembly).Count;
            Assert.IsTrue(numberOfTargetIndexes > 0, "Pre-condition failed: must have at least 1 index in the defined assembly");
            
            var embeddedDb = _ravenInstanceFactory.GetEmbeddedInstance(runInMemory: true);
            var indexBuilder = new IndexBuilder(embeddedDb, _indexAssembly);
            try
            {
                var indexBuildResults = indexBuilder.Run(null);
                Assert.IsNotNull(indexBuildResults);
                Assert.IsTrue(indexBuildResults.Completed > 0, "Should have been able to successfully build at least 1 index");
                Assert.AreEqual(numberOfTargetIndexes, indexBuildResults.Completed, "Expected the number of built indexes to match the number of indexes defined in the assembly");
                Assert.IsTrue(indexBuildResults.Cancelled == 0, "Should not have had any index building jobs cancelled");
                Assert.IsTrue(indexBuildResults.Failed == 0 , "Should not have had any index building jobs fail");
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                indexBuilder.Dispose();
            }
        }

        #endregion
    }
}
