using System.IO;
using System.Reflection;
using Hircine.Core.Runtime;
using NUnit.Framework;

namespace Hircine.Core.Tests.Runtime
{
    [TestFixture(Description = "Test for validating that our path-checking methods behave as expected")]
    public class AssemblyLoaderPathTests
    {
        private string validTestAssemblyPath = "Hircine.TestIndexes.dll"; //will be in the same directory as the test assembly
        private string verifiedAbsoluteTestAssemblyPath = "";

        #region Setup / Teardown

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            var binFolder = Path.GetFullPath(Assembly.GetExecutingAssembly().FullName).Replace(Assembly.GetExecutingAssembly().FullName, string.Empty);
            verifiedAbsoluteTestAssemblyPath = Path.Combine(binFolder, validTestAssemblyPath);
        }

        #endregion

        #region Tests

        [Test(Description = "Our GetFullAssemblyPath method should be able to translate a relative path into an absolute one")]
        public void Should_Extend_Relative_Path_to_Absolute_Path()
        {
            Assert.IsTrue(AssemblyRuntimeLoader.CanFindAssembly(validTestAssemblyPath));
            var absolutePath = AssemblyRuntimeLoader.GetFullAssemblyPath(validTestAssemblyPath);
            Assert.AreEqual(verifiedAbsoluteTestAssemblyPath, absolutePath);
            Assert.IsTrue(AssemblyRuntimeLoader.CanFindAssembly(absolutePath));
        }

        [Test(Description = "Should get a false on our can-find assembly method for an assembly that does not exist")]
        public void Should_Not_Find_Assembly_at_Bad_Path()
        {
            var fakeAssemblyPath = Path.Combine(Path.GetTempPath(), validTestAssemblyPath);
            Assert.IsFalse(AssemblyRuntimeLoader.CanFindAssembly(fakeAssemblyPath));
        }

        #endregion
    }
}
