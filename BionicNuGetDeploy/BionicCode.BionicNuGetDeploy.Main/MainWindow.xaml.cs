using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BionicUtilities.NetStandard.ViewModel;
using Microsoft.Win32;

namespace BionicCode.BionicNuGetDeploy.Main
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
      "ViewModel",
      typeof(IMainPageViewModel),
      typeof(MainWindow),
      new PropertyMetadata(default(IMainPageViewModel)));

    public IMainPageViewModel ViewModel { get { return (IMainPageViewModel) GetValue(MainWindow.ViewModelProperty); } set { SetValue(MainWindow.ViewModelProperty, value); } }

    public MainWindow()
    {
      InitializeComponent();
    }

    public MainWindow(IMainPageViewModel viewModel) : this()
    {
      this.ViewModel = viewModel;
    }

    private void OpenFileDialog_OnClick(object sender, RoutedEventArgs e)
    {
      var fileDialog = new OpenFileDialog()
        {DefaultExt = ".csproj", Filter = "C# Project Source File (.csproj)|*.csproj"};
      if (fileDialog.ShowDialog().Value)
      {
        var selectedFileInfo = new FileInfo(fileDialog.FileName);
        this.ViewModel.ProjectFile = selectedFileInfo;
      }
    }
  }
}
