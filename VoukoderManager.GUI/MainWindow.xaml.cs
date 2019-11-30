using System.Collections.Generic;
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
        private List<ProgramEntry> _detectedPrograms;
        private List<ProgramEntry> _installedVoukoderComponents;
        private Lang _lang;

        public MainWindow()
        {
            InitializeComponent();
            Mouse.OverrideCursor = Cursors.Wait;
            _detector = new ProgramDetector();
            _detectedPrograms = new List<ProgramEntry>();
            _installedVoukoderComponents = new List<ProgramEntry>();
            _lang = new Lang();
            InitializeLanguage();
            _installedVoukoderComponents = _detector.GetInstalledVoukoderComponents();
            _detectedPrograms = _detector.GetInstalledPrograms();
            foreach (ProgramEntry s in _detectedPrograms)
            {
                listBoxPrograms.Items.Add(s.ProgramName);
            }
            foreach (ProgramEntry s in _installedVoukoderComponents)
            {
                listBoxVoukoderComponents.Items.Add(s.ProgramName);
            }
            Mouse.OverrideCursor = null;
        }

        private void InitializeLanguage()
        {
            labelPrograms.Content = Lang.GetText("ui_labelPrograms");
            labelVoukoderComponents.Content = Lang.GetText("ui_labelVoukoderComponents");
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            listBoxPrograms.Items.Clear();
            listBoxVoukoderComponents.Items.Clear();
            _installedVoukoderComponents = _detector.GetInstalledVoukoderComponents();
            _detectedPrograms = _detector.GetInstalledPrograms();
            foreach (ProgramEntry s in _detectedPrograms)
            {
                listBoxPrograms.Items.Add(s.ProgramName);
            }
            foreach (ProgramEntry s in _installedVoukoderComponents)
            {
                listBoxVoukoderComponents.Items.Add(s.ProgramName);
            }
            Mouse.OverrideCursor = null;
        }
    }
}