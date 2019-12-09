using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
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
        private ProgramDetector _detector;
        private List<IProgramEntry> _detectedPrograms;
        private List<IProgramEntry> _installedVoukoderComponents;
        private Lang _lang;
        private BackgroundWorker _worker;
        private IPackageManager<IPackage> _packetmanager;

        public MainWindow()
        {
            InitializeComponent();
            _detector = new ProgramDetector();
            _detectedPrograms = new List<IProgramEntry>();
            _installedVoukoderComponents = new List<IProgramEntry>();
            _lang = new Lang();
            Lang.LanguageChanged += LanguageChanged;
            InitializeLanguage();
            _worker = new BackgroundWorker();
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
            _worker.DoWork += GetPackages;
            _worker.RunWorkerCompleted += WorkCompleted;
            _worker.RunWorkerAsync();
            
            void GetPackages(object sender, DoWorkEventArgs args)
            {
                lst = _packetmanager.GetDownloadablePackages(VoukoderType.VoukoderCore);
            }

            void WorkCompleted(object sender, RunWorkerCompletedEventArgs args)
            {
                Mouse.OverrideCursor = null;
                var page = new PropertyWindow(lst);
                page.InstallEvent += testevent;
                page.ShowDialog();
            }
        }
        private void testevent(object sender, InstallEventArgs e)
        {
            Console.WriteLine(e.PackageToInstall.Name);
            _packetmanager.StartDownloadPackage(e.PackageToInstall.DownloadUrl);
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
                _installedVoukoderComponents = _detector.GetInstalledVoukoderComponents();
                _detectedPrograms = _detector.GetInstalledPrograms();
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
        }
    }
}