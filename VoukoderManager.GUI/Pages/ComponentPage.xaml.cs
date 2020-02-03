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

        public ObservableCollection<VoukoderItemControl> VoukoderItemControls { get; set; }

        private bool _isInstalledPage;

        public ComponentPage(bool isInstalledPage)
        {
            InitializeComponent();
            DataContext = this;
            _isInstalledPage = isInstalledPage;
            VoukoderItemControls = new ObservableCollection<VoukoderItemControl>();
            VoukoderItemControls = new ObservableCollection<VoukoderItemControl>();
            _worker = new BackgroundWorker();
            this.Unloaded += ComponentPage_Unloaded;
            this.Loaded += ComponentPage_Loaded;
            VKProgramEntry.UninstallationFinished += ProgramEntry_UninstallationFinished;
            VKPackage.InstallationFinished += Package_InstallationFinished;
            LoadProgramLists();
        }

        private void Package_InstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            if (!e.Cancelled)
                UpdateProgramList(e.Entry, e.OperationType);
        }

        private void ProgramEntry_UninstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            if (!e.Cancelled)
                UpdateProgramList(e.Entry, e.OperationType);
        }

        private void ComponentPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Trace.WriteLine("ComponentPage loaded");
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

        private void UpdateProgramList(IEntry changedEntry, OperationType operationType)
        {
            _detectedPrograms = ProgramDetector.GetInstalledPrograms(true);
            if (_isInstalledPage)
            {
                if (operationType == OperationType.Uninstall)
                {
                    var en = VoukoderItemControls.Single(x => x.VoukoderProgramData.VoukoderComponent.ComponentType == changedEntry.ComponentType);
                    VoukoderItemControls.Remove(en);
                }
                else if (operationType == OperationType.Install)
                {
                    var item = _detectedPrograms.Single(x => x.ComponentType == changedEntry.ComponentType);
                    AddItem(item);
                }
            }
            else
            {
                if (operationType == OperationType.Uninstall)
                {
                    var item2 = _detectedPrograms.Single(x => x.ComponentType == VKEnumHelper.GetMatchingType(changedEntry.ComponentType));
                    AddItem(item2);
                }
                else if (operationType == OperationType.Install)
                {
                    var en = VoukoderItemControls.Single(x => x.VoukoderProgramData.ComponentType == changedEntry.ComponentType);
                    VoukoderItemControls.Remove(en);
                }
            }
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
                    if (_isInstalledPage)
                    {
                        if (v.VoukoderComponent != null && !v.Hide && !VoukoderItemControls.Any(x => x.Name == "item" + v.ComponentType.ToString()))
                        {
                            AddItem(v);
                        }
                    }
                    else
                    {
                        if (v.VoukoderComponent == null && !v.Hide && !VoukoderItemControls.Any(x => x.Name == "item" + v.ComponentType.ToString()))
                        {
                            AddItem(v);
                        }
                    }
                }

                //if (VoukoderItemControls.Count < 1)
                //{
                //    Label l = new Label();
                //    if (_isInstalledPage)
                //    {
                //        l.Content = "There is currently no Voukoder component installed";
                //    }
                //    else
                //    {
                //        l.Content = "There is currently no program availible where Voukoder can be installed for";
                //    }
                //    l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                //    l.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                //    l.FontSize = 22;
                //    stackpanelPrograms.Children.Add(l);
                //}
                Mouse.OverrideCursor = null;
            }
        }

        private void AddItem(IProgramEntry entry)
        {
            var i = new VoukoderItemControl
            {
                Name = "item" + entry.ComponentType.ToString(),
                VoukoderProgramData = entry,
                Margin = new System.Windows.Thickness(0, 10, 0, 0)
            };
            VoukoderItemControls.Add(i);
        }
    }
}