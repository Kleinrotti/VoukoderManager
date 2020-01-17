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
    public class VoukoderItemControl : Control
    {
        #region Constants

        private const string ElementProgramLabel = "PART_ProgramLabel";
        private const string ElementProgramLogo = "PART_ProgramLogo";
        private const string ElementPropertiesButton = "PART_PropertiesButton";
        private const string ElementUninstallButton = "PART_UninstallButton";
        private const string ElementUpdateButton = "PART_UpdateButton";
        private const string ElementInstallButton = "PART_InstallButton";
        private const string ElementProgressBar = "PART_BarProgress";

        #endregion Constants

        #region Data

        private ProgressBar _barProgress;
        private Button _buttonProperties;
        private Button _buttonUpdate;
        private Button _buttonUninstall;
        private Button _buttonInstall;
        private Image _programLogo;
        private TextBlock _programName;
        private IProgramEntry _entry { get; set; }
        private IProgramEntry _voukoderEntry { get; set; }

        #endregion Data

        #region Ctor

        static VoukoderItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VoukoderItemControl), new FrameworkPropertyMetadata(typeof(VoukoderItemControl)));
        }

        public VoukoderItemControl()
        {
        }

        #endregion Ctor

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
            _barProgress = GetTemplateChild(ElementProgressBar) as ProgressBar;
            _programName.Text = _entry.Name;
            _programLogo.Source = _entry.Logo;

            if (_entry.VoukoderConnector == null)
            {
                _buttonInstall.Visibility = Visibility.Visible;
            }
            else
            {
                _buttonProperties.Visibility = Visibility.Visible;
                _buttonUninstall.Visibility = Visibility.Visible;
                _buttonUpdate.Visibility = Visibility.Visible;
            }
        }

        public IProgramEntry VoukoderProgramData
        {
            get { return (IProgramEntry)GetValue(VoukoderProgramDataProperty); }
            set { SetValue(VoukoderProgramDataProperty, value); }
        }

        public IProgramEntry VoukoderComponentData
        {
            get { return (IProgramEntry)GetValue(VoukoderComponentDataProperty); }
            set { SetValue(VoukoderComponentDataProperty, value); }
        }

        public static readonly DependencyProperty VoukoderProgramDataProperty =
            DependencyProperty.Register(
            "VoukoderProgramData",
            typeof(IProgramEntry),
            typeof(VoukoderItemControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(VoukoderItemControl.OnProgramEntryChanged)));

        public static readonly DependencyProperty VoukoderComponentDataProperty =
            DependencyProperty.Register(
            "VoukoderComponentData",
            typeof(IProgramEntry),
            typeof(VoukoderItemControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(VoukoderItemControl.OnVoukoderEntryChanged)));

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

        protected virtual void OnVoukoderEntryChanged(IProgramEntry oldEntry, IProgramEntry newEntry)
        {
            _voukoderEntry = newEntry;
        }

        private static void OnVoukoderEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (VoukoderItemControl)d;
            v.OnVoukoderEntryChanged((IProgramEntry)e.OldValue, (IProgramEntry)e.NewValue);
        }

        private void _buttonUninstall_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _buttonProperties_Click(object sender, RoutedEventArgs e)
        {
            PropertyWindow w = new PropertyWindow(_entry.VoukoderConnector);
            w.ShowDialog();
        }

        private void _buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _buttonInstall_Click(object sender, RoutedEventArgs e)
        {
            PackageManager p = new PackageManager();
            var lst = p.GetDownloadablePackages(_entry.Type, 5);
            PropertyWindow w = new PropertyWindow(lst)
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.ShowDialog();
        }
    }
}