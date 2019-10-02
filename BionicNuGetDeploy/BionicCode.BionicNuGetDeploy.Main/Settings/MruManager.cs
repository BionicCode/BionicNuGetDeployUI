using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BionicCode.BionicNuGetDeploy.Main.Properties;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicCode.BionicNuGetDeploy.Main.Settings
{
  public class MruManager : BaseViewModel, IMruManager
  {
    public MruManager()
    {
      if (!(AppSettingsConnector.TryReadString(SettingsResources.MaxRecentlyUsedCount, out string mruCount) && int.TryParse(mruCount, out this.mostRecentlyUsedCount)))
      {
        this.mostRecentlyUsedCount = 1;
      }
      if (!(AppSettingsConnector.TryReadString(SettingsResources.LastRecentUsedFile, out string filePath) && File.Exists(filePath)))
      {
        this.LastUsedFilePath = string.Empty;
      }

      IEnumerable<MostRecentlyUsedFileItem> mru = new List<MostRecentlyUsedFileItem>();
      if (AppSettingsConnector.TryReadString(SettingsResources.MostRecentlyUsed, out string fileList))
      {
        mru = fileList.Split(new[] {SettingsResources.MostRecentlyUsedStringSeparator}, StringSplitOptions.RemoveEmptyEntries)
          .Where(File.Exists)
          .Select(validPath => new MostRecentlyUsedFileItem(new FileInfo(validPath)));
      }
      this.MostRecentlyUsedFiles = new ObservableCollection<MostRecentlyUsedFileItem>(mru);
    }

    public void AddMostRecentlyUsedFile(string filePath)
    {
      if (!File.Exists(filePath) || this.MostRecentlyUsedFiles.Any(mruItem => mruItem.FullName.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
      {
        return;
      }

      this.LastUsedFilePath = filePath;

      if (this.MostRecentlyUsedFiles.Count >= this.MostRecentlyUsedCount)
      {
        this.MostRecentlyUsedFiles.RemoveAt(0);
      }

      this.MostRecentlyUsedFiles.Add(new MostRecentlyUsedFileItem(new FileInfo(filePath)));
      string mruListString = string.Join(SettingsResources.MostRecentlyUsedStringSeparator, this.MostRecentlyUsedFiles.Select(mruItem => mruItem.FullName));
      AppSettingsConnector.WriteString(SettingsResources.MostRecentlyUsed, mruListString);
    }

    private ObservableCollection<MostRecentlyUsedFileItem> mostRecentlyUsedFiles;   
    public ObservableCollection<MostRecentlyUsedFileItem> MostRecentlyUsedFiles
    {
      get => this.mostRecentlyUsedFiles;
      private set => TrySetValue(value, ref this.mostRecentlyUsedFiles);
    }

    private string lastUsedFilePath;   
    public string LastUsedFilePath
    {
      get => this.lastUsedFilePath;
      private set => TrySetValue(value, ref this.lastUsedFilePath);
    }

    private int mostRecentlyUsedCount;   
    public int MostRecentlyUsedCount
    {
      get => this.mostRecentlyUsedCount;
      set
      {
        if (TrySetValue(value, IsMruCountValid, ref this.mostRecentlyUsedCount))
        {
          AppSettingsConnector.WriteString(SettingsResources.MaxRecentlyUsedCount, this.MostRecentlyUsedCount.ToString());
        }
      }
    }

    private (bool IsValid, IEnumerable<string> ErrorMessages) IsMruCountValid(int count)
    {
      bool isValid = count > 0 && count < 100;
      (bool IsValid, IEnumerable<string> ErrorMessages) result = (isValid,
        isValid ? new List<string>() : new List<string>() {"Value must be between 1 and 100"});
      return result;
    }
  }
}
