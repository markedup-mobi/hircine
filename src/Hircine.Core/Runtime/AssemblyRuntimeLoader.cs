using System.IO;
using System.Reflection;

namespace Hircine.Core.Runtime
{
    /// <summary>
    /// Utility class responsible for loading assemblies which contain RavenDB indexes (at run-time!)
    /// </summary>
    public static class AssemblyRuntimeLoader
    {
        /// <summary>
        /// Gets the full path of an assembly
        /// </summary>
        /// <param name="assemblyPath">The short / relative path to an assembly</param>
        /// <returns>the full path of an assembly</returns>
        public static string GetFullAssemblyPath(string assemblyPath)
        {
            return Path.GetFullPath(assemblyPath);
        }

        /// <summary>
        /// Verifies if we can actually find an assembly at the specified location
        /// </summary>
        /// <param name="assemblyPath">The path to an assembly</param>
        /// <returns>true if we were able to find the assembly; false otherwise.</returns>
        public static bool CanFindAssembly(string assemblyPath)
        {
            //Can we determine that an assembly exists in this location?
            if (File.Exists(assemblyPath))
                return true;
            return false;
        }

        /// <summary>
        /// Loads an assembly into the current AppDomain (to check for the presence of RavenDb indexes)
        /// </summary>
        /// <param name="assemblyPath">The path to an assembly</param>
        /// <returns>The assembly at the specified location</returns>
        public static Assembly LoadAssembly(string assemblyPath)
        {
            var targetAssembly = Assembly.LoadFrom(assemblyPath);
            return targetAssembly;
        }
    }
}
