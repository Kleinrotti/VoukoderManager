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
        private List<ProgramEntry> _detectedPrograms;
        private List<ProgramEntry> _installedVoukoderComponents;
        private Lang _lang;
        private BackgroundWorker _worker;

        public MainWindow()
        {
            InitializeComponent();
            _detector = new ProgramDetector();
            _detectedPrograms = new List<ProgramEntry>();
            _installedVoukoderComponents = new List<ProgramEntry>();
            _lang = new Lang();
            Lang.LanguageChanged += LanguageChanged;
            InitializeLanguage();
            _worker = new BackgroundWorker();
            LoadProgramLists();
        }

        private void LanguageChanged(object sender, LanguageChangeEventArgs e)
        {
            InitializeLanguage();
        }

        private void LoadProgramLists()
        {
            if (_worker.IsBusy)
            {
                return;
            }
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
                listBoxPrograms.Items.Clear();
                listBoxVoukoderComponents.Items.Clear();
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

        private void InitializeLanguage()
        {
            labelPrograms.Content = Lang.GetText("ui_labelPrograms");
            labelVoukoderComponents.Content = Lang.GetText("ui_labelVoukoderComponents");
        }

        private void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadProgramLists();
        }
    }
}