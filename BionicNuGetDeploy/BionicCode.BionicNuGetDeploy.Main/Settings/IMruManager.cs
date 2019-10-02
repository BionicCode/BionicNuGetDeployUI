#region Info
// //  
// BionicCode.BionicNuGetDeploy.Main
#endregion

using System.Collections.ObjectModel;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicCode.BionicNuGetDeploy.Main.Settings
{
  public interface IMruManager : IViewModel
  {
    void AddMostRecentlyUsedFile(string filePath);
    ObservableCollection<MostRecentlyUsedFileItem> MostRecentlyUsedFiles { get; }
    string LastUsedFilePath { get; }
    int MostRecentlyUsedCount { get; set; }
  }
}