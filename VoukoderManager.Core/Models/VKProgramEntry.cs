using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace VoukoderManager.Core.Models
{
    public class VKProgramEntry : VKEntry, IProgramEntry
    {
        public string InstallationPath { get; set; }
        public bool WindowsInstaller { get; set; }
        public string InstallationDate { get; set; }
        public string UninstallString { get; set; }
        public string ModifyPath { get; set; }
        public string Publisher { get; set; }
        public IProgramEntry SubComponent { get; set; }
        private bool _cancelled;
        protected bool _removeDependency;

        private static BackgroundWorker _worker = new BackgroundWorker();

        public VKProgramEntry(string name, IVersion version) : base(name, version)
        {
            Name = name;
            Version = version;
        }

        public VKProgramEntry(string name, IVersion version, string installationPath) : base(name, version)
        {
            Name = name;
            InstallationPath = installationPath;
            Version = version;
        }

        public static event EventHandler<OperationFinishedEventArgs> UninstallationFinished;

        public virtual BitmapImage Logo
        {
            get
            {
                Log.Debug("Getting program logo");
                if (ComponentType == ProgramType.MediaEncoder)
                    return ToImage(Properties.Resources.me_logo);
                else if (ComponentType == ProgramType.AfterEffects)
                    return ToImage(Properties.Resources.ae_logo);
                else if (ComponentType == ProgramType.Premiere)
                    return ToImage(Properties.Resources.premiere_logo);
                else if (ComponentType == ProgramType.VEGAS)
                    return ToImage(Properties.Resources.vegas_logo);
                else
                    return null;
            }
        }

        private BitmapImage ToImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.DecodePixelWidth = 70;
                image.DecodePixelHeight = 70;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                OnOperationStatusChanged(new ProcessStatusEventArgs($"Cancelled uninstallation of package {Name}", ComponentType));
            else
                UninstallationFinished?.Invoke(this, new OperationFinishedEventArgs(e.Error, _cancelled, this, OperationType.Uninstall));
            _worker.DoWork -= ExecuteProcess;
            _worker.RunWorkerCompleted -= WorkerCompleted;
        }

        public virtual void UninstallPackage(bool includeDendency)
        {
            _removeDependency = includeDendency;
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void ExecuteProcess(object sender, DoWorkEventArgs e)
        {
            OnOperationStatusChanged(new ProcessStatusEventArgs($"Starting uninstall of package {Name}", ComponentType));
            Process p = new Process();
            var startinfo = new ProcessStartInfo("msiexec.exe")
            {
                UseShellExecute = true,
                Arguments = UninstallString.Split(' ')[1],
                Verb = "runas"
            };
            p.StartInfo = startinfo;
            try
            {
                Log.Debug($"Uninstall package with arguments: {startinfo.Arguments}");
                p.Start();
                p.WaitForExit();
                if (!_removeDependency || SubComponent == null)
                {
                    OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished uninstall of package {Name}", ComponentType));
                }
                else
                {
                    OnOperationStatusChanged(new ProcessStatusEventArgs($"Starting uninstall of package {SubComponent.Name}", ComponentType));
                    startinfo.Arguments = SubComponent.UninstallString.Split(' ')[1];
                    p.StartInfo = startinfo;
                    Log.Debug($"Uninstall package with arguments: {startinfo.Arguments}");
                    p.Start();
                    p.WaitForExit();
                    OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished uninstall of package {SubComponent.Name}", ComponentType));
                }
                p.Dispose();
            }
            catch (Win32Exception ex)
            {
                Log.Error(ex, "Error while uninstalling", this);
                _cancelled = true;
                OnOperationStatusChanged(new ProcessStatusEventArgs(ex.Message, ComponentType));
                p.Dispose();
            }
        }

        public virtual void StopUninstallPackage()
        {
            _worker.CancelAsync();
        }
    }
}