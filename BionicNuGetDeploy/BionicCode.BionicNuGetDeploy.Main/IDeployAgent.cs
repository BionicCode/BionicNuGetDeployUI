#region Info
// //  
// BionicCode.BionicNuGetDeploy.Main
#endregion

using System;
using System.Net.Http;
using BionicUtilities.NetStandard.Generic;

namespace BionicCode.BionicNuGetDeploy.Main
{
  public interface IDeployAgent
  {
    void DeployPackage(IDeploymentArgs deploymentArgs);
    event EventHandler<ValueChangedEventArgs<(bool HasError, string Message, string Version)>> Completed;
  }
}