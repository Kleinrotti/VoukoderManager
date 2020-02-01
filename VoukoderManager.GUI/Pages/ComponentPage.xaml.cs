using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
        private ObservableCollection<VoukoderItemControl> _voukoderItemControls;

        public ObservableCollection<VoukoderItemControl> VoukoderItemControls
        {
            get
            {
                return _voukoderItemControls;
            }
            set
            {
                _voukoderItemControls = value;
            }
        }

        private bool _isInstalledPage;

        public ComponentPage(bool isInstalledPage)
        {
            InitializeComponent();
            DataContext = this;
            _isInstalledPage = isInstalledPage;
            _voukoderItemControls = new ObservableCollection<VoukoderItemControl>();
            _voukoderItemControls = new ObservableCollection<VoukoderItemControl>();
            _worker = new BackgroundWorker();
            this.Unloaded += ComponentPage_Unloaded;
            this.Loaded += ComponentPage_Loaded;
            VKProgramEntry.UninstallationFinished += ProgramEntry_UninstallationFinished;
            VKPackage.InstallationFinished += Package_InstallationFinished;
            LoadProgramLists();
        }

        private void Package_InstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            LoadProgramLists();
        }

        private void ProgramEntry_UninstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            if (!e.Cancelled)
                LoadProgramLists();
        }

        private void ComponentPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Trace.WriteLine("ComponentPage loaded");
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
                _worker.DoWork -= GetEntries;
                _worker.RunWorkerCompleted -= WorkCompleted;
                foreach (var v in _detectedPrograms)
                {
                    var vv = v as VKProgramEntry;
                    if (_isInstalledPage)
                    {
                        if (v.VoukoderComponent != null && !vv.Hide)
                        {
                            AddItem(v);
                        }
                    }
                    else
                    {
                        if (v.VoukoderComponent == null && !vv.Hide)
                        {
                            AddItem(v);
                        }
                    }
                }

                if (VoukoderItemControls.Count < 1)
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
                    l.FontSize = 22;
                    stackpanelPrograms.Children.Add(l);
                }
                Mouse.OverrideCursor = null;

                void AddItem(IProgramEntry entry)
                {
                    var i = new VoukoderItemControl
                    {
                        Name = "item" + entry.ComponentType.ToString(),
                        VoukoderProgramData = entry,
                        Margin = new System.Windows.Thickness(0, 10, 0, 0)
                    };
                    //only add to collection if not existing
                    if (!VoukoderItemControls.Any(x => x.Name == "item" + entry.ComponentType.ToString()))
                        VoukoderItemControls.Add(i);
                }
            }
        }
    }
}