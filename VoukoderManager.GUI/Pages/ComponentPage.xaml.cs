using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private TextBlock _textBlockInfo;

        public ObservableCollection<VoukoderItemControl> VoukoderItemControls { get; set; }

        private bool _isInstalledPage;

        public ComponentPage(bool isInstalledPage)
        {
            InitializeComponent();
            Log.Information("Initializing Componentpage");
            DataContext = this;
            _isInstalledPage = isInstalledPage;
            VoukoderItemControls = new ObservableCollection<VoukoderItemControl>();
            VoukoderItemControls.CollectionChanged += VoukoderItemControls_CollectionChanged;
            _worker = new BackgroundWorker();
            this.Unloaded += ComponentPage_Unloaded;
            this.Loaded += ComponentPage_Loaded;
            VKProgramEntry.UninstallationFinished += ProgramEntry_UninstallationFinished;
            VKPackage.InstallationFinished += Package_InstallationFinished;
            _textBlockInfo = new TextBlock();
            if (!_isInstalledPage)
                _textBlockInfo.Text = "There is currently no NLE available where Voukoder can be installed for.";
            else
                _textBlockInfo.Text = "There is currently no Voukoder component installed.";
            _textBlockInfo.FontSize = 15;
            stackpanelPrograms.Children.Add(_textBlockInfo);
            LoadProgramLists();
        }

        private void VoukoderItemControls_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var v = (ObservableCollection<VoukoderItemControl>)sender;
            if (e.NewStartingIndex != -1)
            {
                _textBlockInfo.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (v.Count == 0)
            {
                _textBlockInfo.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Package_InstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            if (!e.Cancelled)
                if (e.Entry.ComponentType == ProgramType.VoukoderCore || ((VKPackage)e.Entry).Dependencies != null)
                    LoadProgramLists();
                else
                    UpdateProgramList(e.Entry, e.OperationType);
        }

        private void ProgramEntry_UninstallationFinished(object sender, OperationFinishedEventArgs e)
        {
            if (!e.Cancelled)
                UpdateProgramList(e.Entry, e.OperationType);
        }

        private void ComponentPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Log.Information("ComponentPage loaded");
        }

        public void RefreshVoukoderComponents()
        {
            LoadProgramLists();
        }

        private void ComponentPage_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Log.Information("ComponentPage unloaded");
        }

        ~ComponentPage()
        {
            Log.Information("ComponentPage destroyed");
        }

        private void UpdateProgramList(IEntry changedEntry, OperationType operationType)
        {
            Log.Debug("Updating program list", changedEntry);
            _detectedPrograms = ProgramDetector.GetInstalledPrograms(true, true);
            try
            {
                if (_isInstalledPage)
                {
                    if (operationType == OperationType.Uninstall)
                    {
                        var en = VoukoderItemControls.Single(x => x.VoukoderProgramData.SubComponent.Name == changedEntry.Name);
                        VoukoderItemControls.Remove(en);
                    }
                    else if (operationType == OperationType.Install)
                    {
                        var item = _detectedPrograms.Where(x => x.ComponentType == changedEntry.ComponentType).First();
                        //Check if entry already exists and delete it if true (appears when updating a component)
                        if (VoukoderItemControls.Any(x => x.Name == "item" + ((VKPackage)changedEntry).Nle.ToString()))
                        {
                            var rm = VoukoderItemControls.Single(x => x.Name == "item" + ((VKPackage)changedEntry).Nle.ToString());
                            VoukoderItemControls.Remove(rm);
                        }
                        AddItem(item);
                    }
                }
                else
                {
                    if (operationType == OperationType.Uninstall)
                    {
                        var item2 = _detectedPrograms.Where(x => x.ComponentType == changedEntry.ComponentType).First();
                        AddItem(item2);
                    }
                    else if (operationType == OperationType.Install)
                    {
                        var en = VoukoderItemControls.Single(x => x.VoukoderProgramData.ComponentType == changedEntry.ComponentType);
                        VoukoderItemControls.Remove(en);
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "Error updating Voukodercontrolitem", changedEntry);
            }
        }

        private void LoadProgramLists()
        {
            if (_worker.IsBusy)
                return;
            Log.Debug("Loading program lists");
            Mouse.OverrideCursor = Cursors.Wait;
            _worker.DoWork += GetEntries;
            _worker.RunWorkerCompleted += WorkCompleted;
            _worker.RunWorkerAsync();

            void GetEntries(object sender, DoWorkEventArgs args)
            {
                _detectedPrograms = ProgramDetector.GetInstalledPrograms(true, true);
            }

            void WorkCompleted(object sender, RunWorkerCompletedEventArgs args)
            {
                _worker.DoWork -= GetEntries;
                _worker.RunWorkerCompleted -= WorkCompleted;
                VoukoderItemControls.Clear();
                foreach (var v in _detectedPrograms)
                {
                    if (_isInstalledPage)
                    {
                        if (v.SubComponent != null && !v.Hide)
                        {
                            AddItem(v);
                        }
                    }
                    else
                    {
                        if (v.SubComponent == null && !v.Hide)
                        {
                            AddItem(v);
                        }
                    }
                }
                Mouse.OverrideCursor = null;
                Log.Debug("Completed program list initialization");
            }
        }

        private void AddItem(IProgramEntry entry)
        {
            var i = new VoukoderItemControl
            {
                Name = "item" + entry.ToString(),
                VoukoderProgramData = entry,
                Margin = new System.Windows.Thickness(0, 10, 0, 0)
            };
            VoukoderItemControls.Add(i);
        }
    }
}