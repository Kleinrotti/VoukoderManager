using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoukoderManager.GUI.Models;
using VoukoderManager.Language;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IProgramEntry> _detectedPrograms;
        private List<IProgramEntry> _installedVoukoderComponents;
        private Lang _lang;
        private BackgroundWorker _worker;
        private PackageManager _packetmanager;
        private ProgressBar bar;
        private IVoukoderEntry _currentVoukoderEntry;
        private InstallationControl _InstallControl;

        public MainWindow()
        {
            _lang = new Lang();
            _lang.Initialize();
            InitializeComponent();
            _detectedPrograms = new List<IProgramEntry>();
            _installedVoukoderComponents = new List<IProgramEntry>();
            Lang.LanguageChanged += LanguageChanged;
            InitializeLanguage();
            _worker = new BackgroundWorker();
            _packetmanager = new PackageManager();
            Package.InstallationFinished += Package_InstallationFinished;
            ProgramEntry.UninstallationFinished += ProgramEntry_UninstallationFinished;
            LoadProgramLists();
        }

        private void ProgramEntry_UninstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            LoadProgramLists();
        }

        private void Package_InstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            LoadProgramLists();
        }

        private void MenuItemPropertiesPrograms(object sender, RoutedEventArgs e)
        {
            ShowInfos();
        }

        private void MenuItemInstallPrograms(object sender, RoutedEventArgs e)
        {
            if (_worker.IsBusy)
                return;
            _packetmanager = new PackageManager();
            _worker = new BackgroundWorker();
            List<IVoukoderEntry> lst = new List<IVoukoderEntry>();
            Mouse.OverrideCursor = Cursors.Wait;
            var clickedtype = ((IProgramEntry)((MenuItem)e.Source).DataContext).Type;
            _worker.DoWork += GetPackages;
            _worker.RunWorkerCompleted += WorkCompleted;
            _worker.RunWorkerAsync();

            void GetPackages(object sender, DoWorkEventArgs args)
            {
                if (clickedtype == ProgramType.VEGAS)
                {
                    lst = _packetmanager.GetDownloadablePackages(ProgramType.VoukoderConnectorVegas, 5);
                }
                else if (clickedtype == ProgramType.Premiere)
                {
                    lst = _packetmanager.GetDownloadablePackages(ProgramType.VoukoderConnectorPremiere, 5);
                }
                else if (clickedtype == ProgramType.VoukoderConnectorAfterEffects)
                {
                    lst = _packetmanager.GetDownloadablePackages(ProgramType.VoukoderConnectorAfterEffects, 5);
                }
            }

            void WorkCompleted(object sender, RunWorkerCompletedEventArgs args)
            {
                Mouse.OverrideCursor = null;
                var page = new PropertyWindow(lst);
                page.Owner = this;
                page.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                page.InstallEvent += BeginInstallation;
                page.ShowDialog();
            }
        }

        private void MenuItemUninstallPrograms(object sender, RoutedEventArgs e)
        {
            _InstallControl = new InstallationControl();
            _InstallControl.CloseControl += _InstallControl_CloseControl;
            mainGrid.Children.Add(_InstallControl);
            var clickedtype = ((IProgramEntry)((MenuItem)e.Source).DataContext).Type;
            if (clickedtype == ProgramType.Premiere)
            {
                _installedVoukoderComponents.Find(i => i.Type == ProgramType.VoukoderConnectorPremiere).UninstallPackage();
            }
            else if (clickedtype == ProgramType.AfterEffects)
            {
                _installedVoukoderComponents.Find(i => i.Type == ProgramType.VoukoderConnectorAfterEffects).UninstallPackage();
            }
            else if (clickedtype == ProgramType.VEGAS)
            {
                _installedVoukoderComponents.Find(i => i.Type == ProgramType.VoukoderConnectorVegas).UninstallPackage();
            }
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

        private void ShowInfos()
        {
            _packetmanager = new PackageManager();
            var test = listBoxPrograms.SelectedItem;
            var page = new PropertyWindow((ProgramEntry)test);
            page.ShowDialog();
        }

        private void LanguageChanged(object sender, LanguageChangeEventArgs e)
        {
            InitializeLanguage();
        }

        private void LoadProgramLists()
        {
            if (_worker.IsBusy)
                return;
            _worker = new BackgroundWorker();
            Mouse.OverrideCursor = Cursors.Wait;
            _worker.DoWork += GetEntries;
            _worker.RunWorkerCompleted += WorkCompleted;
            _worker.RunWorkerAsync();

            void GetEntries(object sender, DoWorkEventArgs args)
            {
                _installedVoukoderComponents = ProgramDetector.GetInstalledVoukoderComponents();
                _detectedPrograms = ProgramDetector.GetInstalledPrograms();
            }

            void WorkCompleted(object sender, RunWorkerCompletedEventArgs args)
            {
                listBoxVoukoderComponents.ItemsSource = _installedVoukoderComponents;
                listBoxPrograms.ItemsSource = _detectedPrograms;
                Mouse.OverrideCursor = null;
            }
        }

        private void InitializeLanguage()
        {
            labelPrograms.Content = Lang.GetText("ui_labelPrograms");
            labelVoukoderComponents.Content = Lang.GetText("ui_labelVoukoderComponents");
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProgramLists();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            _currentVoukoderEntry.StopPackageDownload();
        }

        private void listBoxPrograms_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var type = ((IProgramEntry)((ListBox)e.Source).SelectedItem).Type;
            bool installed = false;
            foreach (var v in _installedVoukoderComponents)
            {
                if (type == ProgramType.Premiere && v.Type == ProgramType.VoukoderConnectorPremiere)
                {
                    installed = true;
                }
                else if (type == ProgramType.VEGAS && v.Type == ProgramType.VoukoderConnectorVegas)
                {
                    installed = true;
                }
                else if (type == ProgramType.AfterEffects && v.Type == ProgramType.VoukoderConnectorAfterEffects)
                {
                    installed = true;
                }
                else
                {
                    installed = false;
                }
            }
            ContextMenu c = listBoxPrograms.FindResource("ItemContextMenu") as ContextMenu;
            MenuItem m = c.Items[0] as MenuItem;
            if (installed)
                m.Visibility = Visibility.Collapsed;
            else
                m.Visibility = Visibility.Visible;
            m = c.Items[1] as MenuItem;
            if (!installed)
                m.Visibility = Visibility.Collapsed;
            else
                m.Visibility = Visibility.Visible;
        }

        private void itemVoukoderUninstall_Click(object sender, RoutedEventArgs e)
        {
            ((IProgramEntry)((MenuItem)e.Source).DataContext).UninstallPackage();
            _InstallControl = new InstallationControl();
            _InstallControl.CloseControl += _InstallControl_CloseControl;
            mainGrid.Children.Add(_InstallControl);
        }
    }
}