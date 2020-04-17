using Microsoft.Win32;
using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VoukoderManager.Core;
using VoukoderManager.Core.Models;
using VoukoderManager.Language;
using VoukoderManager.Notify;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Lang _lang;
        private ComponentPage _installedPage;
        private ComponentPage _availablePage;
        private int _requests = 0;
        private IGitHubEntry _selfUpdate;
        private NotifyService _notifyService;

        public int RemainingRequests
        {
            get { return _requests; }
            set
            {
                _requests = value;
                OnPropertyChanged();
            }
        }

        private string GetOS
        {
            get
            {
                return (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                        select x.GetPropertyValue("Caption")).FirstOrDefault().ToString();
            }
        }

        public MainWindow()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            var logging = RegistryHelper.GetValue("Logging");
            if (logging)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Hour)
                    .CreateLogger();
                Log.Information("----Logging started----");
            }
            _lang = new Lang();
            _lang.Initialize();
            InitializeComponent();
            this.IsVisibleChanged += MainWindow_IsVisibleChanged;
            _notifyService = new NotifyService(this);
            this.Closed += MainWindow_Closed;
            menuItem_debug.IsChecked = logging;
            menuItem_notifications.IsChecked = RegistryHelper.GetValue("Notifications");
            menuItem_tray.IsChecked = RegistryHelper.GetValue("MinimizeToTray");
            menuItem_start_tray.IsChecked = RegistryHelper.GetValue("StartMinimized");
            menuItem_beta.IsChecked = PackageManager.AllowPreReleaseVersion;
            DataContext = this;
            Lang.LanguageChanged += LanguageChanged;
            PackageManager.ApiRequestUsed += PackageManager_ApiRequestUsed;
            VKPackage.InstallationFinished += VKPackage_InstallationFinished;
            _installedPage = new ComponentPage(true);
            _availablePage = new ComponentPage(false);
            _installedPage.InitializeComponent();
            framePages.Navigate(_installedPage);
            CheckSelfUpdate();
            w.Stop();
            Log.Debug($"VoukoderManager running on {GetOS} 64Bit: {Environment.Is64BitOperatingSystem}");
            Log.Debug($"Initialization finished in {w.ElapsedMilliseconds}ms");
        }

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            _notifyService.HideIcon();
        }

        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!((bool)e.NewValue))
                _notifyService.ShowIcon();
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
            _selfUpdate = m.GetManagerUpdate(new Core.Models.Version(Assembly.GetExecutingAssembly().GetName().Version.ToString()));
            if (_selfUpdate != null)
            {
                menuItem_update.Visibility = Visibility.Visible;
                NotifyService.Notify(new Notification("Update", "There is a update for VoukoderManager. Click to install it"), DoSelfUpdate);
            }
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
            if (!menuItem_tray.IsChecked)
                this.WindowState = WindowState.Minimized;
            else
            {
                _notifyService.ShowIcon();
                this.Hide();
            }
        }

        public void MinimizeToTray()
        {
            this.WindowState = WindowState.Minimized;
            Show();
            _notifyService.ShowIcon();
            this.Hide();
        }

        private void gridMove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void buttonInstalled_Click(object sender, RoutedEventArgs e)
        {
            buttonavailable.Foreground = new SolidColorBrush(Colors.Black);
            buttonInstalled.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3399FF"));
            framePages.Navigate(_installedPage);
        }

        private void buttonavailable_Click(object sender, RoutedEventArgs e)
        {
            buttonavailable.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3399FF"));
            buttonInstalled.Foreground = new SolidColorBrush(Colors.Black);
            framePages.Navigate(_availablePage);
        }

        private void MenuItem_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MenuItem_beta_Click(object sender, RoutedEventArgs e)
        {
            var src = e.Source as MenuItem;
            RegistryHelper.SetValue("UseBetaVersions", src.IsChecked);
            MessageBox.Show("Program restart required");
        }

        private void menuItem_notifications_Click(object sender, RoutedEventArgs e)
        {
        }

        private void menuItem_debug_Click(object sender, RoutedEventArgs e)
        {
            RegistryHelper.SetValue("Logging", menuItem_debug.IsChecked);
            if (menuItem_debug.IsChecked)
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Hour)
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
            DoSelfUpdate();
        }

        private async void DoSelfUpdate()
        {
            if (_selfUpdate == null)
                return;
            if (MessageBox.Show("Are you sure to update VoukoderManager?", "Update", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "VoukoderManager.exe",
                Filter = "Executable file (*.exe)|*.exe",
                Title = "Download to..."
            };
            if (sfd.ShowDialog(this) == true)
            {
                ((VKMGithubEntry)_selfUpdate).DownloadDestination = sfd.FileName;
                var pkg = await _selfUpdate.StartPackageDownload();
                Process.Start(((VKMGithubEntry)_selfUpdate).DownloadDestination);
                Application.Current.Shutdown();
            }
        }

        private void menuItem_tray_Click(object sender, RoutedEventArgs e)
        {
            var value = ((MenuItem)e.Source).IsChecked;
            RegistryHelper.SetValue("MinimizeToTray", value);
        }

        private void menuItem_start_tray_Click(object sender, RoutedEventArgs e)
        {
            var value = ((MenuItem)e.Source).IsChecked;
            RegistryHelper.SetValue("StartMinimized", value);
            if (value)
            {
                string fullPath = System.Reflection.Assembly.GetAssembly(typeof(VoukoderManager.GUI.App)).Location;
                RegistryHelper.SetValue(@"Software\Microsoft\Windows\CurrentVersion\Run", "VoukoderManager", fullPath + " --minimized");
            }
            else
            {
                RegistryHelper.DeleteValue(@"Software\Microsoft\Windows\CurrentVersion\Run", "VoukoderManager");
            }
        }
    }
}