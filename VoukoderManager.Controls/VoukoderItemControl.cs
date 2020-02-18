using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VoukoderManager.Core;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Controls
{
    [TemplatePart(Name = VoukoderItemControl.ElementProgramLabel, Type = typeof(TextBlock))]
    [TemplatePart(Name = VoukoderItemControl.ElementProgramLogo, Type = typeof(Image))]
    [TemplatePart(Name = VoukoderItemControl.ElementPropertiesButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementUninstallButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementUpdateButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementInstallButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementProgressBar, Type = typeof(ProgressBar))]
    [TemplatePart(Name = VoukoderItemControl.ElementTextBlockStatus, Type = typeof(TextBlock))]
    [TemplatePart(Name = VoukoderItemControl.ElementLabelVersion, Type = typeof(Label))]
    [TemplatePart(Name = VoukoderItemControl.ElementGridOuter, Type = typeof(Grid))]
    public class VoukoderItemControl : Control
    {
        private const string ElementProgramLabel = "PART_ProgramLabel";
        private const string ElementProgramLogo = "PART_ProgramLogo";
        private const string ElementPropertiesButton = "PART_PropertiesButton";
        private const string ElementUninstallButton = "PART_UninstallButton";
        private const string ElementUpdateButton = "PART_UpdateButton";
        private const string ElementInstallButton = "PART_InstallButton";
        private const string ElementProgressBar = "PART_BarProgress";
        private const string ElementTextBlockStatus = "PART_TextBlockStatus";
        private const string ElementLabelVersion = "PART_labelVersion";
        private const string ElementGridOuter = "PART_gridOuter";

        private ProgressBar _barProgress;
        private Button _buttonProperties;
        private Button _buttonUpdate;
        private Button _buttonUninstall;
        private Button _buttonInstall;
        private Image _programLogo;
        private TextBlock _programName;
        private TextBlock _textBlockStatus;
        private Label _labelVersion;
        private Grid _gridOuter;
        private IProgramEntry _entry { get; set; }
        private IGitHubEntry _packageUpdate;
        private static IGitHubEntry _coreUpdate;
        private static bool _updateSearchCoreDone;

        static VoukoderItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VoukoderItemControl), new FrameworkPropertyMetadata(typeof(VoukoderItemControl)));
        }

        public VoukoderItemControl()
        {
            VKEntry.OperationStatus += Operation_InstallProgressChanged;
        }

        private void Operation_InstallProgressChanged(object sender, ProcessStatusEventArgs e)
        {
            if (_entry.SubComponent != null)
            {
                if (e.ComponentType == _entry.SubComponent.ComponentType)
                    UpdateStatusTextBlock(e.StatusMessage);
            }
            else
            {
                if (e.ComponentType == _entry.ComponentType)
                    UpdateStatusTextBlock(e.StatusMessage);
            }
        }

        private void UpdateStatusTextBlock(string statusMessage)
        {
            _textBlockStatus.Dispatcher.Invoke(() =>
            {
                _textBlockStatus.Visibility = Visibility.Visible;
                _textBlockStatus.Text = statusMessage;
            });
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _buttonInstall = GetTemplateChild(ElementInstallButton) as Button;
            if (_buttonInstall != null)
                _buttonInstall.Click += _buttonInstall_Click;

            _buttonUpdate = GetTemplateChild(ElementUpdateButton) as Button;
            if (_buttonUpdate != null)
                _buttonUpdate.Click += _buttonUpdate_Click;

            _buttonProperties = GetTemplateChild(ElementPropertiesButton) as Button;
            if (_buttonProperties != null)
                _buttonProperties.Click += _buttonProperties_Click;

            _buttonUninstall = GetTemplateChild(ElementUninstallButton) as Button;
            if (_buttonUninstall != null)
                _buttonUninstall.Click += _buttonUninstall_Click;
            _programLogo = GetTemplateChild(ElementProgramLogo) as Image;
            _programName = GetTemplateChild(ElementProgramLabel) as TextBlock;
            _textBlockStatus = GetTemplateChild(ElementTextBlockStatus) as TextBlock;
            _barProgress = GetTemplateChild(ElementProgressBar) as ProgressBar;
            _labelVersion = GetTemplateChild(ElementLabelVersion) as Label;
            _gridOuter = GetTemplateChild(ElementGridOuter) as Grid;
            _programName.Text = _entry.Name;
            _programLogo.Source = _entry.Logo;

            if (_entry.SubComponent == null)
            {
                _buttonInstall.Visibility = Visibility.Visible;
            }
            else
            {
                _buttonProperties.Visibility = Visibility.Visible;
                _buttonUninstall.Visibility = Visibility.Visible;
                _labelVersion.Content = _entry.SubComponent.Version.PackageVersion;
                _labelVersion.Visibility = Visibility.Visible;
                CheckForUpdate();
            }
        }

        public IProgramEntry VoukoderProgramData
        {
            get { return (IProgramEntry)GetValue(VoukoderProgramDataProperty); }
            set { SetValue(VoukoderProgramDataProperty, value); }
        }

        public static readonly DependencyProperty VoukoderProgramDataProperty =
            DependencyProperty.Register(
            "VoukoderProgramData",
            typeof(IProgramEntry),
            typeof(VoukoderItemControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(VoukoderItemControl.OnProgramEntryChanged)));

        protected virtual void OnProgramEntryChanged(IProgramEntry oldEntry, IProgramEntry newEntry)
        {
            _entry = newEntry;
            if (this.IsInitialized)
            {
                _programName.Text = newEntry.Name;
                _programLogo.Source = newEntry.Logo;
            }
        }

        private static void OnProgramEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (VoukoderItemControl)d;
            v.OnProgramEntryChanged((IProgramEntry)e.OldValue, (IProgramEntry)e.NewValue);
        }

        private void _buttonUninstall_Click(object sender, RoutedEventArgs e)
        {
            var cp = ProgramDetector.GetInstalledVoukoderComponents();
            if (_entry.SubComponent.SubComponent != null)
            {
                foreach (var v in cp)
                {
                    //remove voukoder core too if it's the last connector installed
                    if (v.ComponentType != _entry.SubComponent.ComponentType && v.ComponentType != _entry.SubComponent.SubComponent.ComponentType)
                    {
                        _entry.SubComponent.UninstallPackage(false);
                        return;
                    }
                }
            }
            _entry.SubComponent.UninstallPackage(true);
        }

        private void _buttonProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertyWindow w = new PropertyWindow(_entry.SubComponent)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.ShowDialog();
        }

        private async void _buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_packageUpdate != null)
            {
                var updatemsg = "Update: Voukoder for " + _packageUpdate.ComponentType.ToString() +
                    " to version: " + _packageUpdate.Version.PackageVersion;
                if (_packageUpdate.Dependencies != null)
                {
                    foreach (var v in _packageUpdate.Dependencies)
                    {
                        updatemsg += "\nUpdate: Voukoder Core " + v.ComponentType.ToString() +
                        " to version: " + v.Version.PackageVersion;
                    }
                }
                var msb = MessageBox.Show(updatemsg, "Update", MessageBoxButton.YesNo);
                if (msb == MessageBoxResult.Yes)
                    await DownloadPackage(_packageUpdate, true);
            }
        }

        private void _buttonInstall_Click(object sender, RoutedEventArgs e)
        {
            PackageManager p = new PackageManager();
            var lst = p.GetDownloadablePackages(_entry.ComponentType, 5);
            if (lst == null)
            {
                MessageBox.Show("Couldn't receive list, maybe too many API requests");
                return;
            }
            PropertyWindow w = new PropertyWindow(lst)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };
            w.InstallEvent += W_InstallEvent;
            w.ShowDialog();
            w.InstallEvent -= W_InstallEvent;
        }

        private void W_InstallEvent(object sender, InstallEventArgs e)
        {
            DownloadPackage(e.PackageToInstall, false);
        }

        private async Task DownloadPackage(IGitHubEntry entry, bool forceDepDownload)
        {
            VKGithubEntry.DownloadProgressChanged += VoukoderEntry_DownloadProgressChanged;
            var t = await entry.StartPackageDownloadWithDependencies(forceDepDownload);
            _coreUpdate = null;
            t.InstallPackageWithDepenencies(_entry);
            VKGithubEntry.DownloadProgressChanged -= VoukoderEntry_DownloadProgressChanged;
        }

        private void VoukoderEntry_DownloadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            UpdateStatusTextBlock($"Downloading: {e.ProgressPercentage}%");
        }

        private void CheckForUpdate()
        {
            if (_entry.SubComponent != null)
            {
                var p = new PackageManager();
                if (!_updateSearchCoreDone)
                {
                    _coreUpdate = p.GetUpdate(_entry.SubComponent.SubComponent);
                    _updateSearchCoreDone = true;
                }

                var update = p.GetUpdate(_entry.SubComponent);
                if (update == null)
                {
                    if (_coreUpdate != null)
                    {
                        _packageUpdate = _coreUpdate;
                        _buttonUpdate.Content = "Update";
                        _buttonUpdate.Visibility = Visibility.Visible;
                        CreateUpdateInfo();
                    }
                }
                else
                {
                    if (_coreUpdate != null)
                        update.Dependencies = new List<IGitHubEntry>() { _coreUpdate };
                    _packageUpdate = update;
                    _buttonUpdate.Content = "Update";
                    _buttonUpdate.Visibility = Visibility.Visible;
                    CreateUpdateInfo();
                }
            }
        }

        private void CreateUpdateInfo()
        {
            var dockPanelUpdate = new DockPanel();
            dockPanelUpdate.SetValue(Grid.RowProperty, 1);
            dockPanelUpdate.Margin = new Thickness(30, -5, 0, 0);
            dockPanelUpdate.SetValue(VerticalAlignmentProperty, VerticalAlignment.Bottom);
            dockPanelUpdate.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);

            var icon = new PackIcon { Kind = PackIconKind.Information };
            icon.Foreground = new SolidColorBrush(Colors.CornflowerBlue);

            var labelBlockHeadline = new Label();
            labelBlockHeadline.Content = "Update availible";
            DockPanel.SetDock(labelBlockHeadline, Dock.Bottom);

            var labelUpdateVersion = new Label();
            labelUpdateVersion.Content = _packageUpdate.Version.PackageVersion;
            DockPanel.SetDock(labelUpdateVersion, Dock.Bottom);

            var buttonChangelog = new Button();
            buttonChangelog.Content = "View Details";
            buttonChangelog.Foreground = new SolidColorBrush(Colors.CornflowerBlue);
            buttonChangelog.Background = new SolidColorBrush(Colors.Transparent);
            buttonChangelog.BorderThickness = new Thickness(0);
            buttonChangelog.Click += ButtonChangelog_Click;
            buttonChangelog.Width = double.NaN;
            buttonChangelog.HorizontalAlignment = HorizontalAlignment.Left;
            DockPanel.SetDock(buttonChangelog, Dock.Bottom);

            dockPanelUpdate.Children.Add(icon);
            dockPanelUpdate.Children.Add(buttonChangelog);
            dockPanelUpdate.Children.Add(labelUpdateVersion);

            dockPanelUpdate.Children.Add(labelBlockHeadline);
            _gridOuter.Children.Add(dockPanelUpdate);
        }

        private void ButtonChangelog_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Not implemented yet");
        }
    }
}