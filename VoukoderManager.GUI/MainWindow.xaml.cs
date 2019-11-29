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
        private List<Entry> _detectedPrograms;
        private List<Entry> _installedVoukoderComponents;

        public MainWindow()
        {
            InitializeComponent();
            _detector = new ProgramDetector();
            _detectedPrograms = new List<Entry>();
            _installedVoukoderComponents = new List<Entry>();
            _installedVoukoderComponents = _detector.GetInstalledVoukoderComponents();
            _detectedPrograms = _detector.GetInstalledPrograms();
            foreach (Entry s in _detectedPrograms)
            {
                listBoxPrograms.Items.Add(s.ProgramName);
            }
            foreach (Entry s in _installedVoukoderComponents)
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
            foreach (Entry s in _detectedPrograms)
            {
                listBoxPrograms.Items.Add(s.ProgramName);
            }
            foreach (Entry s in _installedVoukoderComponents)
            {
                listBoxVoukoderComponents.Items.Add(s.ProgramName);
            }
        }
    }
}