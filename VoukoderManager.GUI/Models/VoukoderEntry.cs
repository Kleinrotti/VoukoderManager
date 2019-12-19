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
        private WebClient _webclient;
        private string packagePath = "";
        public List<IVoukoderEntry> Dependencies { get; }

        public static event EventHandler<ProgressChangedEventArgs> DownloadProgressChanged;

        public ProgramType Dependency
        {
            get
            {
                if (Type == ProgramType.VoukoderConnectorAfterEffects || Type == ProgramType.VoukoderConnectorPremiere || Type == ProgramType.VoukoderConnectorVegas)
                {
                    return ProgramType.VoukoderCore;
                }
                else
                {
                    return ProgramType.None;
                }
            }
        }

        public async Task StartPackageDownload()
        {
            _webclient = new WebClient();
            OnInstallProgress(new ProcessStatusEventArgs("Downloading files..."));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
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