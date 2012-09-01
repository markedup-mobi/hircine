using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Raven.Client.Indexes;

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

        /// <summary>
        /// Checks a loaded assembly for any defined RavenDB indexes
        /// </summary>
        /// <param name="targetAssembly">The assembly containing RavenDb indexes</param>
        /// <returns>true if indexes are detected in the assembly, false otherwise</returns>
        public static bool HasRavenDbIndexes(Assembly targetAssembly)
        {
            return GetRavenDbIndexes(targetAssembly).Count > 0;
        }

        /// <summary>
        /// Lists the types of all defined RavenDB indexes from the assembly
        /// </summary>
        /// <param name="targetAssembly">The assembly containing RavenDb indexes</param>
        /// <returns>A list of types derived from AbstractIndexCreationTask (base class for defining RavenDb indexes)</returns>
        public static IList<Type> GetRavenDbIndexes(Assembly targetAssembly)
        {
            return targetAssembly.GetTypes()
                .Where(x => typeof (AbstractIndexCreationTask).IsAssignableFrom(x) && !x.IsAbstract).ToList();
        }
    }
}
