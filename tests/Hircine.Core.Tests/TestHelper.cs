using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hircine.Core.Tests
{
    /// <summary>
    /// Helpers for some of our tests - mostly used for exposing paths to our test assemblies
    /// </summary>
    public static class TestHelper
    {
        /// <summary>
        /// The name of our test assembly containing valid indexes
        /// </summary>
        public const string ValidTestAssemblyPath = "Hircine.TestIndexes.dll"; //will be in the same directory as the test assembly

        /// <summary>
        /// The absolute path to the assembly, based on the current file location of the test assembly
        /// </summary>
        public static string VerifiedFullTestAssemblyPath
        {
            get
            {
                var binFolder = Path.GetFullPath(Assembly.GetExecutingAssembly().FullName).Replace(Assembly.GetExecutingAssembly().FullName, string.Empty);
                return Path.Combine(binFolder, TestHelper.ValidTestAssemblyPath);
            }
        }

        /// <summary>
        /// Absolute path to an assembly that does not exist
        /// </summary>
        public static string InvalidAssemblyPath
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), TestHelper.ValidTestAssemblyPath);
            }
        }
    }
}
