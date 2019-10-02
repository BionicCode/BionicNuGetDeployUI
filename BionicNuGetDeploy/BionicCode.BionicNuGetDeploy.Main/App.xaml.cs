using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BionicCode.BionicNuGetDeploy.Main
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void RunApplication(object sender, StartupEventArgs e)
    {
      var bootstrapper = new Bootstrapper();
      bootstrapper.ComposeDependencies();
      var mainWindow = bootstrapper.CompositionContainer.GetExportedValue<MainWindow>();
      mainWindow.Show();
    }
  }
}
