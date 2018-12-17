using System.Reflection;
using System.Runtime.InteropServices;
using TcHmiSrv.Management;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("EngraveItServer")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("EngraveItServer")]
[assembly: AssemblyCopyright("Copyright ©  2018")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3c62b829-dcda-45c8-8b85-f4bbab0fb63b")]

[assembly: // keep this newline
    AssemblyVersion("1.0.0.0")]
[assembly: // keep this newline
    AssemblyFileVersion("1.0.0.0")]

// Declare the default type of the server extension
[assembly: ExtensionType(typeof(EngraveItServer.EngraveItServer))]
