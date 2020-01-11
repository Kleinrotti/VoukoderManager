using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using VoukoderManager.Core;
using VoukoderManager.Core.Models;
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
        private IVoukoderEntry _currentVoukoderEntry;
        private InstallationControl _InstallControl;

        private readonly Border _gridBorder = new Border
        {
            BorderBrush = new SolidColorBrush(Colors.Black),
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(0),
            Effect = new DropShadowEffect
            {
                Color = Colors.Black,
                ShadowDepth = 0,
            },
        };

        private readonly Image _voukoderLogo = new Image
        {
            Source = new BitmapImage(new Uri(@"/Resources/logo.png", UriKind.Relative)),
            Height = 30,
            Width = 30,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10)
        };

        private readonly Label _logoLabel = new Label
        {
            Content = "oukoder for",
            Width = 80,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(38, 8, 0, 0)
        };

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
            ShowInfos((IProgramEntry)listBoxPrograms.SelectedItem);
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
                else if (clickedtype == ProgramType.AfterEffects)
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

        private void ShowInfos(IEntry entry)
        {
            var page = new PropertyWindow(entry);
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
                itemVoukoderTest.VoukoderData = _detectedPrograms[0];
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
                switch (type)
                {
                    case ProgramType.Premiere when v.Type == ProgramType.VoukoderConnectorPremiere:
                        installed = true;
                        break;

                    case ProgramType.VEGAS when v.Type == ProgramType.VoukoderConnectorVegas:
                        installed = true;
                        break;

                    case ProgramType.AfterEffects when v.Type == ProgramType.VoukoderConnectorAfterEffects:
                        installed = true;
                        break;

                    default:
                        installed = false;
                        break;
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

        private void itemVoukoderProperties_Click(object sender, RoutedEventArgs e)
        {
            ShowInfos((IProgramEntry)listBoxVoukoderComponents.SelectedItem);
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
    }
}