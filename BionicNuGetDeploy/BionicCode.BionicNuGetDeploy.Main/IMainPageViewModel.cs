#region Info
// //  
// BionicCode.BionicNuGetDeploy.Main
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using BionicUtilities.Net.Utility;

namespace BionicCode.BionicNuGetDeploy.Main
{
  public interface IMainPageViewModel : IDeploymentArgs
  {
    string ProjectFilePathString { get; set; }
    IAsyncRelayCommand DeployCommand { get; }
    bool HasErrors { get; }
    IEnumerable GetErrors(string propertyName);
    event PropertyChangedEventHandler PropertyChanged;
    event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
  }
}