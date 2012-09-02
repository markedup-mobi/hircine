using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG
    [assembly: AssemblyProduct("Hircine.Core (Debug)")]
    [assembly: AssemblyConfiguration("Debug")]
#else
    [assembly: AssemblyProduct("Hircine.Core (Release)")]
    [assembly: AssemblyConfiguration("Release")]
#endif


// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Hircine.Core")]
[assembly: AssemblyDescription("Core engine for Hircine, the stand-alone RavenDB index builder, used in CI systems and automated deployments")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("0acff44e-c20d-4d2b-a4e1-4bdc48fa7f44")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]