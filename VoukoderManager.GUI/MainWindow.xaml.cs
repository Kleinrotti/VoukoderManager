﻿using Microsoft.Win32;
using Serilog;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VoukoderManager.Core;
using VoukoderManager.Core.Models;
using VoukoderManager.Language;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Lang _lang;
        private ComponentPage _installedPage;
        private ComponentPage _availiblePage;
        private int _requests = 0;
        private IGitHubEntry _selfUpdate;

        public int RemainingRequests
        {
            get { return _requests; }
            set
            {
                _requests = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            _lang = new Lang();
            _lang.Initialize();
            InitializeComponent();
            menuItem_beta.IsChecked = PackageManager.AllowPreReleaseVersion;
            menuItem_debug.IsChecked = RegistryHelper.GetLogging();
            if (menuItem_debug.IsChecked)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            DataContext = this;
            Lang.LanguageChanged += LanguageChanged;
            PackageManager.ApiRequestUsed += PackageManager_ApiRequestUsed;
            VKPackage.InstallationFinished += VKPackage_InstallationFinished;
            _installedPage = new ComponentPage(true);
            _availiblePage = new ComponentPage(false);
            framePages.Navigate(_installedPage);
            CheckSelfUpdate();
        }

        private void VKPackage_InstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            if (e.OperationType == OperationType.Install)
                buttonInstalled_Click(this, new RoutedEventArgs());
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion INotifyPropertyChanged Members

        private void PackageManager_ApiRequestUsed(object sender, ApiRequestEventArgs e)
        {
            RemainingRequests = e.Remaining;
        }

        private void CheckSelfUpdate()
        {
            PackageManager m = new PackageManager();
            _selfUpdate = m.GetManagerUpdate(new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            if (_selfUpdate != null)
                menuItem_update.Visibility = Visibility.Visible;
        }

        private void LanguageChanged(object sender, LanguageChangeEventArgs e)
        {
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

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_beta_Click(object sender, RoutedEventArgs e)
        {
            var src = e.Source as MenuItem;
            RegistryHelper.SetUseBetaVersion(src.IsChecked);
        }

        private void menuItem_notifications_Click(object sender, RoutedEventArgs e)
        {
        }

        private void menuItem_debug_Click(object sender, RoutedEventArgs e)
        {
            RegistryHelper.SetLogging(menuItem_debug.IsChecked);
            if (menuItem_debug.IsChecked)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();
            }
            else
            {
                Log.CloseAndFlush();
            }
        }

        private void menuItem_about_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.Owner = this;
            about.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            about.Show();
        }

        private void menuItem_update_Click(object sender, RoutedEventArgs e)
        {
            if (_selfUpdate == null)
                return;
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "VoukoderManager.exe",
                Filter = "Executable file (*.exe)|*.exe",
                Title = "Download to..."
            };
            if (sfd.ShowDialog(this) == true)
            {
                ((VKMGithubEntry)_selfUpdate).DownloadDestination = sfd.FileName.TrimEnd('\\') + @"\";
                var pkg = _selfUpdate.StartPackageDownload().Result;
            }
        }
    }
}