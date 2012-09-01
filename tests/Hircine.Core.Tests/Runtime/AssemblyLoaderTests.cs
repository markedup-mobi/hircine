using System.Collections;
using System.IO;
using System.Reflection;
using Hircine.Core.Runtime;
using NUnit.Framework;

namespace Hircine.Core.Tests.Runtime
{
    [TestFixture(Description = "Test for validating that our path-checking methods behave as expected")]
    public class AssemblyLoaderTests
    {
        #region Setup / Teardown
        #endregion

        #region Tests

        [Test(Description = "Our GetFullAssemblyPath method should be able to translate a relative path into an absolute one")]
        public void Should_Extend_Relative_Path_to_Absolute_Path()
        {
            Assert.IsTrue(AssemblyRuntimeLoader.CanFindAssembly(TestHelper.ValidTestAssemblyPath));
            var absolutePath = AssemblyRuntimeLoader.GetFullAssemblyPath(TestHelper.ValidTestAssemblyPath);
            Assert.AreEqual(TestHelper.VerifiedFullTestAssemblyPath, absolutePath);
            Assert.IsTrue(AssemblyRuntimeLoader.CanFindAssembly(absolutePath));
        }

        [Test(Description = "Should get a false on our can-find assembly method for an assembly that does not exist")]
        public void Should_Not_Find_Assembly_at_Bad_Path()
        {
            var fakeAssemblyPath = Path.Combine(Path.GetTempPath(), TestHelper.ValidTestAssemblyPath);
            Assert.IsFalse(AssemblyRuntimeLoader.CanFindAssembly(fakeAssemblyPath));
        }

        [Test(Description = "Should be able to load a valid assembly into memory at run-time")]
        public void Should_Load_Valid_Assembly()
        {
            Assert.IsTrue(AssemblyRuntimeLoader.CanFindAssembly(TestHelper.ValidTestAssemblyPath));

            var assembly = AssemblyRuntimeLoader.LoadAssembly(TestHelper.ValidTestAssemblyPath);

            Assert.IsNotNull(assembly);
            Assert.AreNotEqual(0, ((ICollection) assembly.GetTypes()).Count);
        }

        [Test(Description = "Should be able to find RavenDb indexes on an assembly that is defined with them")]
        public void Should_Find_RavenDbIndexes_On_Assembly_with_Index_Definitions()
        {
            Assert.IsTrue(AssemblyRuntimeLoader.CanFindAssembly(TestHelper.ValidTestAssemblyPath));

            var assembly = AssemblyRuntimeLoader.LoadAssembly(TestHelper.ValidTestAssemblyPath);

            Assert.IsNotNull(assembly);

            var indexTypes = AssemblyRuntimeLoader.GetRavenDbIndexes(assembly);
            Assert.IsNotNull(indexTypes);
            Assert.IsTrue(indexTypes.Count > 0);
            Assert.IsTrue(AssemblyRuntimeLoader.HasRavenDbIndexes(assembly));
        }

        #endregion
    }
}
