using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicCode.BionicNuGetDeploy.Main
{
  public class MostRecentlyUsedFileItem : BaseViewModel
  {
    public MostRecentlyUsedFileItem(FileInfo fileInfo)
    {
      this.FileInfo = fileInfo;
    }

    public FileInfo FileInfo { get; }
    public string Name => this.FileInfo.Name;
    public string FullName => this.FileInfo.FullName;
  }
}
