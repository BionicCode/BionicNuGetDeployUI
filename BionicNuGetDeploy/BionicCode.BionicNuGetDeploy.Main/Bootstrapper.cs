using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BionicCode.BionicNuGetDeploy.Main.Settings;
using BionicUtilities.NetStandard.ViewModel;

namespace BionicCode.BionicNuGetDeploy.Main
{
  class Bootstrapper
  {
    public void ComposeDependencies()
    {
      var dependencyBuilder = new RegistrationBuilder();

      dependencyBuilder.ForTypesDerivedFrom<IDeployAgent>()
        .Export<IDeployAgent>();
      dependencyBuilder.ForTypesDerivedFrom<IMruManager>()
        .Export<IMruManager>();
      dependencyBuilder.ForTypesDerivedFrom<IMainPageViewModel>()
        .Export<IMainPageViewModel>();
      dependencyBuilder.ForType<MainWindow>()
        .Export();

      var catalog = new AssemblyCatalog(Assembly.GetExecutingAssembly(), dependencyBuilder);
      this.CompositionContainer = new CompositionContainer(catalog, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
      this.CompositionContainer.ComposeParts();
    }

    public CompositionContainer CompositionContainer { get; private set; }
  }
}
