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
        public IProgramEntry VoukoderComponent { get; set; }
        private bool _cancelled;

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
                if (ComponentType == ProgramType.MediaEncoder)
                    return ToImage(VoukoderManager.Core.Properties.Resources.me_logo);
                else if (ComponentType == ProgramType.AfterEffects)
                    return ToImage(VoukoderManager.Core.Properties.Resources.ae_logo);
                else if (ComponentType == ProgramType.Premiere)
                    return ToImage(VoukoderManager.Core.Properties.Resources.premiere_logo);
                else if (ComponentType == ProgramType.VEGAS)
                    return ToImage(VoukoderManager.Core.Properties.Resources.vegas_logo);
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
                OnOperationStatusChanged(new ProcessStatusEventArgs($"Cancelled installation of package {Name}", ComponentType));
            else
                UninstallationFinished?.Invoke(this, new OperationFinishedEventArgs(e.Error, _cancelled, this));
            _worker.DoWork -= ExecuteProcess;
            _worker.RunWorkerCompleted -= WorkerCompleted;
        }

        public virtual void UninstallPackage()
        {
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
                p.Start();
                p.WaitForExit();
                p.Dispose();
                OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished uninstall of package {Name}", ComponentType));
            }
            catch (Win32Exception ex)
            {
                _cancelled = true;
                OnOperationStatusChanged(new ProcessStatusEventArgs(ex.Message, ComponentType));
            }
        }

        public virtual void StopUninstallPackage()
        {
            _worker.CancelAsync();
        }
    }
}