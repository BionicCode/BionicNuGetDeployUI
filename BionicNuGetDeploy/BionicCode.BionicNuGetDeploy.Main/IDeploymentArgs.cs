#region Info
// //  
// BionicCode.BionicNuGetDeploy.Main
#endregion

using System.IO;

namespace BionicCode.BionicNuGetDeploy.Main
{
  public interface IDeploymentArgs
  {
    FileInfo ProjectFile { get; set; }
    string VersionNumber { get; set; }
    bool IsIncludeReferencedProjectsEnabled { get; set; }
    bool IsIncludeDebugSymbolsEnabled { get; set; }
    string Url { get; set; }
    string ApplicationKey { get; set; }
  }
}