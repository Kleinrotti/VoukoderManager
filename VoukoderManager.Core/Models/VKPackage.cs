using Microsoft.Deployment.WindowsInstaller;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace VoukoderManager.Core.Models
{
    public class VKPackage : VKEntry, IPackage
    {
        public virtual List<IPackage> Dependencies { get; set; }
        public virtual string Path { get; set; }
        protected static BackgroundWorker _worker = new BackgroundWorker();
        protected bool _installDependencies;
        public IEntry Nle { get; set; }

        protected virtual string InstallArguments
        {
            get
            {
                if (ComponentType == ProgramType.VEGAS || ComponentType == ProgramType.MovieStudio)
                    return @" /i " + Path + @" VEGASDIR=""" + PluginsPath + @""" /qn";
                else if (ComponentType == ProgramType.VoukoderCore)
                    return @" /i " + Path + @" /qn";
                else
                    return @" /i " + Path + @" TGTDIR=""" + PluginsPath + @""" /qn";
            }
        }

        protected string msiExec = "msiexec.exe";

        protected virtual string PluginsPath
        {
            get
            {
                return ProgramDetector.GetPluginsDir(Nle);
            }
        }

        public VKPackage(string name, IVersion version) : base(name, version)
        {
            Name = name;
            Version = version;
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
        }

        public VKPackage(string name, IVersion version, string path, ProgramType type) : base(name, version)
        {
            Path = path;
            Name = name;
            ComponentType = type;
        }

        public virtual bool Certified
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual PackageType Type
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

        public override IVersion Version
        {
            get
            {
                using (Database db = new Database(Path))
                {
                    Log.Debug("Reading version from file", this);
                    return new Version(db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", "ProductVersion") as string);
                }
            }
        }

        public static event EventHandler<OperationFinishedEventArgs> InstallationFinished;

        public virtual void InstallPackage(IEntry nle)
        {
            _installDependencies = false;
            Nle = nle;
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                OnOperationStatusChanged(new ProcessStatusEventArgs($"Cancelled installation of package {Name}", ComponentType));
            else
            {
                Log.Debug($"Installation finished of package: {Name}");
                InstallationFinished?.Invoke(this, new OperationFinishedEventArgs(e.Error, e.Cancelled, this, OperationType.Install));
            }
            _worker.DoWork -= ExecuteProcess;
            _worker.RunWorkerCompleted -= WorkerCompleted;
        }

        private void ExecuteProcess(object sender, DoWorkEventArgs e)
        {
            try
            {
                Process p = new Process();
                OnOperationStatusChanged(new ProcessStatusEventArgs($"Starting installation of package {Name}", ComponentType));
                var startinfo = new ProcessStartInfo(msiExec)
                {
                    UseShellExecute = true,
                    Arguments = InstallArguments,
                    Verb = "runas"
                };
                p.StartInfo = startinfo;
                Log.Debug($"Install package with arguments: {startinfo.Arguments}");
                p.Start();
                p.WaitForExit();
                if (!_installDependencies || (Dependencies == null))
                {
                    OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished installation of package {Name}", ComponentType));
                }
                else
                {
                    foreach (var v in Dependencies)
                    {
                        OnOperationStatusChanged(new ProcessStatusEventArgs($"Starting installation of package dependency {v.Name}", ComponentType));
                        startinfo.Arguments = @" /i " + v.Path + @" /qn";
                        p.StartInfo = startinfo;
                        Log.Debug($"Install package with arguments: {startinfo.Arguments}");
                        p.Start();
                        p.WaitForExit();
                        OnOperationStatusChanged(new ProcessStatusEventArgs($"Finished installation of package dependency {v.Name}", ComponentType));
                    }
                }
                p.Dispose();
            }
            catch (Win32Exception ex)
            {
                Log.Error(ex, "Error while installing package", this);
                OnOperationStatusChanged(new ProcessStatusEventArgs(ex.Message, ComponentType));
            }
        }

        public virtual void InstallPackageWithDepenencies(IEntry nle)
        {
            _installDependencies = true;
            Nle = nle;
            _worker.DoWork += ExecuteProcess;
            _worker.RunWorkerCompleted += WorkerCompleted;
            _worker.RunWorkerAsync();
        }

        public virtual void StopInstallation()
        {
            _worker.CancelAsync();
        }
    }
}