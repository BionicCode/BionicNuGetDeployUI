#region Info
// //  
// BionicCode.BionicNuGetDeploy.Main
#endregion

using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BionicCode.BionicNuGetDeploy.Main.Properties;
using BionicCode.BionicNuGetDeploy.Main.Settings;
using BionicUtilities.NetStandard.Generic;

namespace BionicCode.BionicNuGetDeploy.Main
{
  public class DeployAgent : IDeployAgent
  {
    public DeployAgent()
    {
    }
    private void SetupEnvironment()
    {
      if (!File.Exists("nuget.exe"))
      {
        throw new FileNotFoundException("Missing executable: nuget.exe.");
      }
    }

    #region Implementation of IDeployAgent

    /// <inheritdoc />
    private void RegisterPackage(string applicationKey, FileInfo assemblyInfo)
    {
      byte[] hashedApplicationKey = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(applicationKey));
      var hashString = Encoding.UTF8.GetString(hashedApplicationKey);
      if (!AppSettingsConnector.TryReadString(hashString, out string filePath) 
          || !filePath.Equals(
            assemblyInfo.FullName,
            StringComparison.OrdinalIgnoreCase))
      {
        RegisterUser(applicationKey, assemblyInfo);
      }


      // Create .nuspec file
      if (!File.Exists(Path.Combine(assemblyInfo.DirectoryName, $"{assemblyInfo.Name}.nuspec")))
      {
        DeployAgent.CreateNuSpecFile(assemblyInfo);
      }
    }

    private static void CreateNuSpecFile(FileInfo assemblyInfo)
    {
      var processInfo = new ProcessStartInfo("nuget.exe")
      {
//        CreateNoWindow = true,
        WorkingDirectory = assemblyInfo.DirectoryName,
        Arguments = $"spec {assemblyInfo.Name}",
        ErrorDialog = true
      };
      Process.Start(processInfo);
    }

    private void RegisterUser(string applicationKey, FileInfo assemblyInfo)
    {
      var processInfo = new ProcessStartInfo("nuget.exe")
      {
//        CreateNoWindow = true, 
        WorkingDirectory = assemblyInfo.DirectoryName,
        Arguments = $"setApiKey {applicationKey}",
        ErrorDialog = true
      };

      // Register app key
      Process.Start(processInfo);

      byte[] hashedApplicationKey = SHA512.Create().ComputeHash(Encoding.UTF8.GetBytes(applicationKey));
      var hashString = Encoding.UTF8.GetString(hashedApplicationKey);
      AppSettingsConnector.WriteString(hashString, string.Empty);
    }

    /// <inheritdoc />
    public void DeployPackage(IDeploymentArgs deploymentArgs)
    {
      RegisterPackage(deploymentArgs.ApplicationKey, deploymentArgs.ProjectFile);
      string assemblyVersion = AssemblyName.GetAssemblyName(deploymentArgs.ProjectFile.FullName).Version.ToString();
      CreatePackageFile(deploymentArgs);
      PushPackage(deploymentArgs, assemblyVersion);

      (bool HasErrors, string Message, string Version) resultArgs = (false, string.Empty, assemblyVersion);
      OnCompleted(resultArgs, resultArgs);
    }

    private void PushPackage(IDeploymentArgs deploymentArgs, string assemblyVersion)
    {
      string command = DeployAgent.BuildPushPackageCommand(deploymentArgs, assemblyVersion);
      var processInfo = new ProcessStartInfo("nuget.exe")
      {
//        CreateNoWindow = true,
        WorkingDirectory = deploymentArgs.ProjectFile.DirectoryName,
        Arguments = command,
        ErrorDialog = true
      };
      Process.Start(processInfo);
    }

    private void CreatePackageFile(IDeploymentArgs deploymentArgs)
    {
      string command = DeployAgent.BuildCreatePackageCommand(deploymentArgs);
      var processInfo = new ProcessStartInfo("nuget.exe")
      {
//        CreateNoWindow = true,
        WorkingDirectory = deploymentArgs.ProjectFile.DirectoryName,
        Arguments = command,
        ErrorDialog = true
      };
      Process.Start(processInfo);
    }

    private static string BuildPushPackageCommand(IDeploymentArgs deploymentArgs, string assemblyVersion)
    {
      var command = $"push {deploymentArgs.ProjectFile.Name.Substring(0, deploymentArgs.ProjectFile.Name.IndexOf(deploymentArgs.ProjectFile.Extension))}" + ".";

      command += assemblyVersion + ".nupk ";
      command += CommandlineArguments.Source + " " + CommandlineArguments.SourceUrl;
      return command;
    }

    private static string BuildCreatePackageCommand(IDeploymentArgs deploymentArgs)
    {
      var command = $"pack {deploymentArgs.ProjectFile.Name} ";
      if (AppSettingsConnector.TryReadBool(
            SettingsResources.IsIncludingDebugSymbols,
            out bool isIncludingReferencedProjects) && isIncludingReferencedProjects)
      {
        command += CommandlineArguments.IncludeReferencedProjects + " ";
      }

      if (AppSettingsConnector.TryReadBool(SettingsResources.IsIncludingDebugSymbols, out bool isIncludingDebugSymbols) &&
          isIncludingDebugSymbols)
      {
        command += CommandlineArguments.Symbols;
      }

      command += CommandlineArguments.Source + " " + CommandlineArguments.SourceUrl;
      return command;
    }

    /// <inheritdoc />
    public event EventHandler<ValueChangedEventArgs<(bool HasError, string Message, string Version)>> Completed;

    #endregion

    protected virtual void OnCompleted((bool HasError, string Message, string Version) oldValue, (bool HasError, string Message, string Version) newValue)
    {
      this.Completed?.Invoke(this, new ValueChangedEventArgs<(bool HasError, string Message, string Version)>(oldValue, newValue));
    }
  }
}