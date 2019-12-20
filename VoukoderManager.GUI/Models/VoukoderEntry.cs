using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace VoukoderManager.GUI.Models
{
    /// <summary>
    /// Describes a downloadable Voukoder component
    /// </summary>
    public class VoukoderEntry : Operation, IVoukoderEntry
    {
        public Uri DownloadUrl { get; set; }
        public string Changelog { get; set; }
        public IVersion Version { get; set; }
        public ProgramType Type { get; set; }
        public string Name { get; set; }
        private WebClient _webclient = new WebClient();
        private string packagePath = "";
        public List<IVoukoderEntry> Dependencies { get; set; }

        public static event EventHandler<ProgressChangedEventArgs> DownloadProgressChanged;

        public async Task StartPackageDownload()
        {
            OnInstallProgress(new ProcessStatusEventArgs("Downloading files..."));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            OnInstallProgress(new ProcessStatusEventArgs("Download finished"));
        }

        public async Task StartPackageDownloadWithDependencies()
        {
            OnInstallProgress(new ProcessStatusEventArgs("Downloading files..."));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            //Download all depencencies
            foreach (var v in Dependencies)
            {
                packagePath = Path.GetTempPath() + v.DownloadUrl.Segments[v.DownloadUrl.Segments.Length - 1];
                await _webclient.DownloadFileTaskAsync(v.DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
                {
                    ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                    OnDownloadProgressChanged(a);
                }));
            }
            OnInstallProgress(new ProcessStatusEventArgs("Download finished"));
        }

        private void OnDownloadProgressChanged(ProgressChangedEventArgs e)
        {
            DownloadProgressChanged?.Invoke(this, e);
        }

        public void StopPackageDownload()
        {
            OnInstallProgress(new ProcessStatusEventArgs("Stopping download..."));
            if (!_webclient.IsBusy)
            {
                OnInstallProgress(new ProcessStatusEventArgs("Download already finished"));
                return;
            }
            _webclient.CancelAsync();
            OnInstallProgress(new ProcessStatusEventArgs("Download stopped"));
        }
    }
}