using System;
using System.ComponentModel;
using System.Diagnostics;

namespace VoukoderManager.Core.Models
{
    public class ProgramEntry : Operation, IProgramEntry
    {
        public string Name { get; set; }
        public string InstallationPath { get; set; }
        public IVersion Version { get; set; }
        public bool WindowsInstaller { get; set; }
        public string InstallationDate { get; set; }
        public string UninstallString { get; set; }
        public string ModifyPath { get; set; }
        public string Publisher { get; set; }
        public ProgramType Type { get; set; }
        private static BackgroundWorker _worker = new BackgroundWorker();

        public static event EventHandler<OperationFinishedEventArgs> UninstallationFinished;

        public ProgramEntry(string programName, string installationPath, string version)
        {
            Name = programName;
            InstallationPath = installationPath;
            Version = new Version(version, CheckPreRelease(version));
        }

        public ProgramEntry(string programName, string installationPath, string version, bool windowsInstaller)
        {
            Name = programName;
            InstallationPath = installationPath;
            Version = new Version(version, CheckPreRelease(version));
            WindowsInstaller = windowsInstaller;
        }

        public ProgramEntry()
        {
        }

        private bool CheckPreRelease(string version)
        {
            if (version.Contains("rc") || version.Contains("beta"))
                return true;
            else
                return false;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                OnInstallProgress(new ProcessStatusEventArgs($"Cancelled installation of package {Name}"));
            else
                UninstallationFinished?.Invoke(this, new OperationFinishedEventArgs(e.Error, e.Cancelled, this));
            _worker.DoWork -= ExecuteProcess;
            _worker.RunWorkerCompleted -= WorkerCompleted;
        }

        public void UninstallPackage()
        {
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void ExecuteProcess(object sender, DoWorkEventArgs e)
        {
            OnInstallProgress(new ProcessStatusEventArgs($"Starting uninstall of package {Name}"));
            Process p = new Process();
            var startinfo = new ProcessStartInfo("msiexec.exe")
            {
                UseShellExecute = true,
                Arguments = UninstallString.Split(' ')[1],
                Verb = "runas"
            };
            p.StartInfo = startinfo;
            p.Start();
            p.WaitForExit();
            p.Dispose();
            OnInstallProgress(new ProcessStatusEventArgs($"Finished uninstall of package {Name}"));
        }

        public void StopUninstallPackage()
        {
            _worker.CancelAsync();
        }
    }
}