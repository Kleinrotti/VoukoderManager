using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using VoukoderManager.Controls;
using VoukoderManager.Core;
using VoukoderManager.Core.Models;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Interaction logic for ComponentPage.xaml
    /// </summary>
    public partial class ComponentPage : Page
    {
        private BackgroundWorker _worker;
        private List<IProgramEntry> _detectedPrograms;
        private List<VoukoderItemControl> _voukoderItemControls;
        private bool _isInstalledPage;

        public ComponentPage(bool isInstalledPage)
        {
            InitializeComponent();
            _isInstalledPage = isInstalledPage;
            _worker = new BackgroundWorker();
            this.Unloaded += ComponentPage_Unloaded;
            LoadProgramLists();
        }

        public void RefreshVoukoderComponents()
        {
            LoadProgramLists();
        }

        private void ComponentPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Trace.WriteLine("ComponentPage unloaded");
        }

        ~ComponentPage()
        {
            Trace.WriteLine("ComponentPage destroyed");
        }

        private void LoadProgramLists()
        {
            if (_worker.IsBusy)
                return;
            _worker = new BackgroundWorker();
            Mouse.OverrideCursor = Cursors.Wait;
            _worker.DoWork += GetEntries;
            _worker.RunWorkerCompleted += WorkCompleted;
            _worker.RunWorkerAsync();

            void GetEntries(object sender, DoWorkEventArgs args)
            {
                _detectedPrograms = ProgramDetector.GetInstalledPrograms(true);
            }

            void WorkCompleted(object sender, RunWorkerCompletedEventArgs args)
            {
                stackpanelPrograms.Children.Clear();
                _voukoderItemControls = new List<VoukoderItemControl>();
                foreach (var v in _detectedPrograms)
                {
                    if (_isInstalledPage)
                    {
                        if (v.VoukoderConnector != null)
                        {
                            AddItem(v);
                        }
                    }
                    else
                    {
                        if (v.VoukoderConnector == null)
                        {
                            AddItem(v);
                        }
                    }
                }

                if (_voukoderItemControls.Count < 1)
                {
                    Label l = new Label();
                    if (_isInstalledPage)
                    {
                        l.Content = "There is currently no Voukoder component installed";
                    }
                    else
                    {
                        l.Content = "There is currently no program availible where Voukoder can be installed for";
                    }
                    l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                    l.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    l.FontSize = 25;
                    stackpanelPrograms.Children.Add(l);
                }
                Mouse.OverrideCursor = null;

                void AddItem(IProgramEntry entry)
                {
                    var i = new VoukoderItemControl
                    {
                        Name = "item" + entry.Type.ToString(),
                        VoukoderProgramData = entry,
                        Margin = new System.Windows.Thickness(0, 10, 0, 0)
                    };
                    _voukoderItemControls.Add(i);
                    stackpanelPrograms.Children.Add(i);
                }
            }
        }
    }
}