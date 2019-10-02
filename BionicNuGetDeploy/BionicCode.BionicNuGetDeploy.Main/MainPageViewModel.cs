using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BionicCode.BionicNuGetDeploy.Main.Properties;
using BionicCode.BionicNuGetDeploy.Main.Settings;
using BionicUtilities.Net.Utility;
using BionicUtilities.NetStandard.Generic;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicCode.BionicNuGetDeploy.Main
{
  public class MainPageViewModel : BaseViewModel, IMainPageViewModel
  {
    public MainPageViewModel(IDeployAgent deployAgent, IMruManager mostRecentlyUsedFilesManager)
    {
      this.DeployAgent = deployAgent;
      this.MostRecentlyUsedFilesManager = mostRecentlyUsedFilesManager;
    }
    private bool CanDeploy() => !this.HasErrors;

    private Task<(bool HasError, string Message, string Version)> DeployAsync()
    {
      this.TaskCompletionSource = new TaskCompletionSource<(bool HasError, string Message, string Version)>();
      this.DeployAgent.Completed += OnDeploymentCompleted;
      this.DeployAgent.DeployPackage(this);
      return this.TaskCompletionSource.Task;
    }

    private void OnDeploymentCompleted(object sender, ValueChangedEventArgs<(bool HasError, string Message, string Version)> e)
    {
      (bool HasError, string Message, string Version) newValue = e.NewValue;
      if (newValue.HasError)
      {
        this.TaskCompletionSource.TrySetException(new InvalidOperationException(newValue.Message));
      }

      this.TaskCompletionSource.TrySetResult(newValue);
    }

    private string projectFilePathString;

    /// <inheritdoc />
    public string ProjectFilePathString
    {
      get => this.ProjectFile.FullName;
      set
      {
        TrySetValue(value, IsPathValid, ref this.projectFilePathString);
      }
    }

    private (bool IsValid, IEnumerable<string> ErrorMessages) IsPathValid(string path)
    {
      var errors = new List<string>();
      (bool IsValid, IEnumerable<string> ErrorMessages) result = (IsValid: true, ErrorMessages: errors);
      if (File.Exists(path))
      {
        FileInfo fileInfo = new FileInfo(path);
        if (!TrySetValue(fileInfo, IsFileProjectFile, ref this.projectFile, nameof(this.ProjectFile)) && PropertyHasError(nameof(this.ProjectFile)))
        {
          errors.AddRange(GetErrors(nameof(this.ProjectFile)).Cast<string>());
          result.IsValid = false;
        }

        return result;
      }

      result.IsValid = false;
      errors.Add("File doesn't exist");
      return result;
    }

    private (bool IsValid, IEnumerable<string> ErrorMessages) IsFileProjectFile(FileInfo fileInfo)
    {
      var errors = new List<string>();
      (bool IsValid, IEnumerable<string> ErrorMessages) result = (IsValid: true, ErrorMessages: errors);
      if (!fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase))
      {
        result.IsValid = false;
        errors.Add("File is not a C# project source file (.csproj).");
      }

      return result;
    }

    public IAsyncRelayCommand DeployCommand => new AsyncRelayCommand(DeployAsync, CanDeploy);


    private FileInfo projectFile;
    public FileInfo ProjectFile
    {
      get => this.projectFile;
      set
      {
        if (TrySetValue(value, IsFileProjectFile, ref this.projectFile))
        {
          this.MostRecentlyUsedFilesManager.AddMostRecentlyUsedFile(this.ProjectFile.FullName);
        }
      }
    }

    private string versionNumber;   
    public string VersionNumber
    {
      get => this.versionNumber;
      set => TrySetValue(value, ref this.versionNumber);
    }

    private bool isIncludeReferencedProjectsEnabled;   
    public bool IsIncludeReferencedProjectsEnabled
    {
      get => this.isIncludeReferencedProjectsEnabled;
      set => TrySetValue(value, ref this.isIncludeReferencedProjectsEnabled);
    }

    private bool isIncludeDebugSymbolsEnabled;   
    public bool IsIncludeDebugSymbolsEnabled
    {
      get => this.isIncludeDebugSymbolsEnabled;
      set => TrySetValue(value, ref this.isIncludeDebugSymbolsEnabled);
    }

    private string url;   
    public string Url
    {
      get => this.url;
      set => TrySetValue(value, ref this.url);
    }

    private string applicationKey;   
    public string ApplicationKey
    {
      get => this.applicationKey;
      set => TrySetValue(value, ref this.applicationKey);
    }
    private IDeployAgent DeployAgent { get; }
    private IMruManager MostRecentlyUsedFilesManager { get; }
    private TaskCompletionSource<(bool HasError, string Message, string Version)> TaskCompletionSource { get; set; }
  }
}
