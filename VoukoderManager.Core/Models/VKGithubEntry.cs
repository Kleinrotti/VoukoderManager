using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VoukoderManager.Core.Models
{
    /// <summary>
    /// Describes a downloadable Voukoder component
    /// </summary>
    public class VKGithubEntry : VKEntry, IGitHubEntry
    {
        public Uri DownloadUrl { get; set; }
        public string Changelog { get; set; }
        private WebClient _webclient = new WebClient();
        private string packagePath = String.Empty;

        public VKGithubEntry(string name, IVersion version) : base(name, version)
        {
        }

        public List<IGitHubEntry> Dependencies { get; set; }

        public static event EventHandler<ProgressChangedEventArgs> DownloadProgressChanged;

        public virtual async Task<IPackage> StartPackageDownload()
        {
            OnOperationStatusChanged(new ProcessStatusEventArgs("Downloading files...", ComponentType));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            var pkg = new VKPackage(Name, Version, packagePath, ComponentType);
            OnOperationStatusChanged(new ProcessStatusEventArgs("Download finished", ComponentType));
            return pkg;
        }

        public virtual async Task<IPackage> StartPackageDownloadWithDependencies()
        {
            OnOperationStatusChanged(new ProcessStatusEventArgs($"Downloading package {Name}...", ComponentType));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            var p = new VKPackage(Name, Version, packagePath, ComponentType);

            //Download all depencencies
            if (Dependencies != null)
            {
                List<IPackage> dep = new List<IPackage>();
                foreach (var v in Dependencies)
                {
                    if (!ProgramDetector.IsVoukoderComponentInstalled(v))
                    {
                        OnOperationStatusChanged(new ProcessStatusEventArgs($"Downloading package dependency  {v.ComponentType} {v.Name}...", ComponentType));
                        packagePath = Path.GetTempPath() + v.DownloadUrl.Segments[v.DownloadUrl.Segments.Length - 1];
                        await _webclient.DownloadFileTaskAsync(v.DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
                        {
                            ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                            OnDownloadProgressChanged(a);
                        }));
                        dep.Add(new VKPackage(v.Name, v.Version, packagePath, v.ComponentType));
                    }
                }
                p.Dependencies = dep;
            }
            OnOperationStatusChanged(new ProcessStatusEventArgs("Download finished", ComponentType));
            return p;
        }

        private void OnDownloadProgressChanged(ProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        public virtual void StopPackageDownload()
        {
            OnOperationStatusChanged(new ProcessStatusEventArgs("Stopping download...", ComponentType));
            if (!_webclient.IsBusy)
            {
                OnOperationStatusChanged(new ProcessStatusEventArgs("Download already finished", ComponentType));
                return;
            }
            _webclient.CancelAsync();
            OnOperationStatusChanged(new ProcessStatusEventArgs("Download stopped", ComponentType));
        }
    }
}