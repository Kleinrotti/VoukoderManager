﻿using System;
using System.Windows;
using System.Windows.Controls;
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

        private ProgressBar _barProgress;
        private Button _buttonProperties;
        private Button _buttonUpdate;
        private Button _buttonUninstall;
        private Button _buttonInstall;
        private Image _programLogo;
        private TextBlock _programName;
        private TextBlock _textBlockStatus;
        private IProgramEntry _entry { get; set; }
        private IGitHubEntry _connectorUpdate;
        private static IGitHubEntry _coreUpdate;
        private bool _updateSearchDone;
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
            if (_entry.VoukoderComponent != null)
            {
                if (e.ComponentType == _entry.VoukoderComponent.ComponentType)
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
            _programName.Text = _entry.Name;
            _programLogo.Source = _entry.Logo;

            if (_entry.VoukoderComponent == null)
            {
                _buttonInstall.Visibility = Visibility.Visible;
            }
            else
            {
                _buttonProperties.Visibility = Visibility.Visible;
                _buttonUninstall.Visibility = Visibility.Visible;
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
            _entry.VoukoderComponent.UninstallPackage();
        }

        private void _buttonProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertyWindow w = new PropertyWindow(_entry.VoukoderComponent)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.ShowDialog();
        }

        private void _buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (_connectorUpdate != null)
            {
                var msb = MessageBox.Show("Are you sure to update: " + _connectorUpdate.ComponentType.ToString() +
                    " to version: " + _connectorUpdate.Version.PackageVersion +
                    "?", "Update", MessageBoxButton.YesNo);
                if (msb == MessageBoxResult.Yes)
                    DownloadPackage(_connectorUpdate);
            }
            if (_coreUpdate != null)
            {
                var msb = MessageBox.Show("Are you sure to update: " + _coreUpdate.ComponentType.ToString() +
                    " to version: " + _coreUpdate.Version.PackageVersion +
                    "?", "Update Core", MessageBoxButton.YesNo);
                if (msb == MessageBoxResult.Yes)
                    DownloadPackage(_coreUpdate);
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
            DownloadPackage(e.PackageToInstall);
        }

        private async void DownloadPackage(IGitHubEntry entry)
        {
            VKGithubEntry.DownloadProgressChanged += VoukoderEntry_DownloadProgressChanged;
            var t = await entry.StartPackageDownloadWithDependencies();
            t.InstallPackageWithDepenencies();
            VKGithubEntry.DownloadProgressChanged -= VoukoderEntry_DownloadProgressChanged;
        }

        private void VoukoderEntry_DownloadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            UpdateStatusTextBlock($"Downloading: {e.ProgressPercentage}%");
        }

        private void CheckForUpdate()
        {
            if (_entry.VoukoderComponent != null)
            {
                Console.WriteLine("Update Called");
                var p = new PackageManager();
                var update = p.GetUpdate(_entry.VoukoderComponent);
                if (update != null)
                {
                    _connectorUpdate = update;
                    _buttonUpdate.Content = "Update";
                    _buttonUpdate.Visibility = Visibility.Visible;
                }
                //Check for voukoder core update
                if (!_updateSearchCoreDone)
                {
                    var update2 = p.GetUpdate(_entry.VoukoderComponent.VoukoderComponent);
                    _updateSearchCoreDone = true;
                    if (update2 != null)
                    {
                        _coreUpdate = update2;
                        _buttonUpdate.Visibility = Visibility.Visible;
                        _buttonUpdate.Content = "Update Core";
                    }
                }
            }
        }
    }
}