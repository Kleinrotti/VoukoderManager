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
        public string Path { get; set; }
        public ProgramType ComponentType { get; set; }
        private static BackgroundWorker _worker = new BackgroundWorker();
        private bool _installDependencies;

        public bool Certified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        public static event EventHandler<OperationFinishedEventArgs> InstallationFinished;

        public Package()
        {
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
        }

        public Package(string packageName, string path, ProgramType type)
        {
            Path = path;
            Name = packageName;
            ComponentType = type;
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
                OnOperationStatusChanged(new ProcessStatusEventArgs($"Cancelled installation of package {Name}", ComponentType));
            else
                InstallationFinished?.Invoke(this, new OperationFinishedEventArgs(e.Error, e.Cancelled, this));
            _worker.DoWork -= ExecuteProcess;
            _worker.RunWorkerCompleted -= WorkerCompleted;
        }

        private void ExecuteProcess(object sender, DoWorkEventArgs e)
        {
            Process p = new Process();
            OnOperationStatusChanged(new ProcessStatusEventArgs($"Starting installation of package {Name}", ComponentType));
            var startinfo = new ProcessStartInfo("msiexec.exe")
            {
                UseShellExecute = true,
                Arguments = @" /i " + Path + @" /qn /log install.log",
                Verb = "runas"
            };
            p.StartInfo = startinfo;
            try
            {
                p.Start();
                p.WaitForExit();
                if (!_installDependencies || Dependencies.Count == 0)
                {
                    OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished installation of package {Name}", ComponentType));
                    p.Dispose();
                }
                else
                {
                    foreach (var v in Dependencies)
                    {
                        OnOperationStatusChanged(new ProcessStatusEventArgs($"Starting installation of package dependency {v.Name}", ComponentType));
                        startinfo.Arguments = @" /i " + v.Path + @" /qn /log install.log";
                        p.StartInfo = startinfo;
                        p.Start();
                        p.WaitForExit();
                        OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished installation of package dependency {v.Name}", ComponentType));
                    }
                    p.Dispose();
                }
            }
            catch (Win32Exception ex)
            {
                OnOperationStatusChanged(new ProcessStatusEventArgs(ex.Message, ComponentType));
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