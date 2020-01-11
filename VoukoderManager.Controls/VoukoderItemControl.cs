using System.Windows;
using System.Windows.Controls;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Controls
{
    [TemplatePart(Name = VoukoderItemControl.ElementProgramLabel, Type = typeof(TextBlock))]
    [TemplatePart(Name = VoukoderItemControl.ElementProgramLogo, Type = typeof(Image))]
    [TemplatePart(Name = VoukoderItemControl.ElementPropertiesButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementUninstallButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementInstallButton, Type = typeof(Button))]
    [TemplatePart(Name = VoukoderItemControl.ElementProgressBar, Type = typeof(ProgressBar))]
    public class VoukoderItemControl : Control
    {
        #region Constants

        private const string ElementProgramLabel = "PART_ProgramLabel";
        private const string ElementProgramLogo = "PART_ProgramLogo";
        private const string ElementPropertiesButton = "PART_PropertiesButton";
        private const string ElementUninstallButton = "PART_UninstallButton";
        private const string ElementInstallButton = "PART_InstallButton";
        private const string ElementProgressBar = "PART_ProgressBar";

        #endregion Constants

        #region Data

        private ProgressBar _barProgress;
        private Button _buttonProperties;
        private Button _buttonInstall;
        private Button _buttonUninstall;
        private Image _programLogo;
        private TextBlock _programName;

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

            _buttonProperties = GetTemplateChild(ElementPropertiesButton) as Button;
            if (_buttonProperties != null)
                _buttonProperties.Click += _buttonProperties_Click;

            _buttonUninstall = GetTemplateChild(ElementUninstallButton) as Button;
            if (_buttonUninstall != null)
                _buttonUninstall.Click += _buttonUninstall_Click;
            _programLogo = GetTemplateChild(ElementProgramLogo) as Image;
            _programName = GetTemplateChild(ElementProgramLabel) as TextBlock;
        }

        public IProgramEntry VoukoderData
        {
            get { return (IProgramEntry)GetValue(VoukoderDataProperty); }
            set { SetValue(VoukoderDataProperty, value); }
        }

        public static readonly DependencyProperty VoukoderDataProperty =
            DependencyProperty.Register(
            "VoukoderData",
            typeof(IProgramEntry),
            typeof(VoukoderItemControl),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(VoukoderItemControl.OnEntryChanged)));

        protected virtual void OnEntryChanged(IProgramEntry oldEntry, IProgramEntry newEntry)
        {
            _programLogo.Source = newEntry.Logo;
            _programName.Text = newEntry.Name;
        }

        private static void OnEntryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (VoukoderItemControl)d;
            v.OnEntryChanged((IProgramEntry)e.OldValue, (IProgramEntry)e.NewValue);
        }

        private void _buttonUninstall_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _buttonProperties_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _buttonInstall_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}