using System.Collections.Generic;
using System.Windows;

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

        public MainWindow()
        {
            InitializeComponent();
            _detector = new ProgramDetector();
            _detectedPrograms = new List<ProgramEntry>();
            _installedVoukoderComponents = new List<ProgramEntry>();
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
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
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
        }
    }
}