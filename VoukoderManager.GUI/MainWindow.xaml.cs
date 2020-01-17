using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VoukoderManager.Core.Models;
using VoukoderManager.Language;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Lang _lang;
        private IVoukoderEntry _currentVoukoderEntry;
        private InstallationControl _InstallControl;
        private ComponentPage _installedPage;
        private ComponentPage _availiblePage;

        public MainWindow()
        {
            _lang = new Lang();
            _lang.Initialize();
            InitializeComponent();
            Lang.LanguageChanged += LanguageChanged;
            _installedPage = new ComponentPage(true);
            _availiblePage = new ComponentPage(false);
            framePages.Navigate(_installedPage);
        }

        private void BeginInstallation(object sender, InstallEventArgs e)
        {
            StartDownload(e.PackageToInstall);
        }

        private async void StartDownload(IVoukoderEntry package)
        {
            _InstallControl = new InstallationControl();
            _InstallControl.CloseControl += _InstallControl_CloseControl;
            mainGrid.Children.Add(_InstallControl);
            _currentVoukoderEntry = package;
            try
            {
                var pkg = await package.StartPackageDownloadWithDependencies() as Package;
                pkg.InstallPackageWithDepenencies();
            }
            catch (WebException ex) { }
        }

        private void _InstallControl_CloseControl(object sender, System.EventArgs e)
        {
            mainGrid.Children.Remove(_InstallControl);
            _InstallControl.CloseControl -= _InstallControl_CloseControl;
        }

        private void LanguageChanged(object sender, LanguageChangeEventArgs e)
        {
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            _currentVoukoderEntry.StopPackageDownload();
        }

        private void itemVoukoderUninstall_Click(object sender, RoutedEventArgs e)
        {
            ((IProgramEntry)((MenuItem)e.Source).DataContext).UninstallPackage();
            _InstallControl = new InstallationControl();
            _InstallControl.CloseControl += _InstallControl_CloseControl;
            mainGrid.Children.Add(_InstallControl);
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void gridMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void buttonInstalled_Click(object sender, RoutedEventArgs e)
        {
            buttonAvailible.Foreground = new SolidColorBrush(Colors.Black);
            buttonInstalled.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3399FF"));
            framePages.Navigate(_installedPage);
        }

        private void buttonAvailible_Click(object sender, RoutedEventArgs e)
        {
            buttonAvailible.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3399FF"));
            buttonInstalled.Foreground = new SolidColorBrush(Colors.Black);
            framePages.Navigate(_availiblePage);
        }
    }
}