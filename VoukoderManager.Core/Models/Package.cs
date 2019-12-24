using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace VoukoderManager.Core.Models
{
    public class Package : Operation, IPackage
    {
        public string Name { get; set; }
        public List<IPackage> Dependencies { get; set; }
        private static BackgroundWorker _worker = new BackgroundWorker();
        private bool _installDependencies;

        public static event EventHandler<OperationFinishedEventArgs> InstallationFinished;

        public Package()
        {
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
        }

        public bool Certified
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public string Path { get; set; }

        public PackageType Type
        {
            get
            {
                var info = new FileInfo(Path).Extension;
                if (info == ".msi")
                    return PackageType.MSI;
                else if (info == ".exe")
                    return PackageType.EXE;
                else
                    return PackageType.NONE;
            }
        }

        public IVersion Version
        {
            get
            {
                using (Database db = new Database(Path))
                {
                    return new Version(db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", "ProductVersion") as string);
                }
            }
        }

        public Package(string packageName, string path)
        {
            Path = path;
            Name = packageName;
        }

        public void InstallPackage()
        {
            _installDependencies = false;
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                OnInstallProgress(new ProcessStatusEventArgs($"Cancelled installation of package {Name}"));
            else
                InstallationFinished?.Invoke(this, new OperationFinishedEventArgs(e.Error, e.Cancelled, this));
            _worker.DoWork -= ExecuteProcess;
            _worker.RunWorkerCompleted -= WorkerCompleted;
        }

        private void ExecuteProcess(object sender, DoWorkEventArgs e)
        {
            Process p = new Process();
            OnInstallProgress(new ProcessStatusEventArgs($"Starting installation of package {Name}"));
            var startinfo = new ProcessStartInfo("msiexec.exe")
            {
                UseShellExecute = true,
                Arguments = @" /i " + Path + @" /qn /log install.log",
                Verb = "runas"
            };
            p.StartInfo = startinfo;
            p.Start();
            p.WaitForExit();
            if (!_installDependencies)
            {
                p.Dispose();
                OnInstallProgress(new ProcessStatusEventArgs($"Finished installation of package {Name}"));
            }
            else
            {
                foreach (var v in Dependencies)
                {
                    OnInstallProgress(new ProcessStatusEventArgs($"Starting installation of package dependency{v.Name}"));
                    startinfo.Arguments = @" /i " + v.Path + @" /qn /log install.log";
                    p.StartInfo = startinfo;
                    p.Start();
                    p.WaitForExit();
                    OnInstallProgress(new ProcessStatusEventArgs($"Finished installation of package dependency {v.Name}"));
                }
                p.Dispose();
            }
        }

        public void InstallPackageWithDepenencies()
        {
            _installDependencies = true;
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.RunWorkerAsync();
        }

        public void StopInstallation()
        {
            _worker.CancelAsync();
        }
    }
}