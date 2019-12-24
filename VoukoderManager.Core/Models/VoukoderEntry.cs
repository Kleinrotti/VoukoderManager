﻿using System;
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

        public IPackage DownloadedPackage { get; private set; }

        public static event EventHandler<ProgressChangedEventArgs> DownloadProgressChanged;

        public async Task<IPackage> StartPackageDownload()
        {
            OnInstallProgress(new ProcessStatusEventArgs("Downloading files..."));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            var pkg = new Package(Name, packagePath);
            OnInstallProgress(new ProcessStatusEventArgs("Download finished"));
            return pkg;
        }

        public async Task<IPackage> StartPackageDownloadWithDependencies()
        {
            OnInstallProgress(new ProcessStatusEventArgs($"Downloading package {Name}..."));
            packagePath = Path.GetTempPath() + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            var p = new Package(Name, packagePath);
            //Download all depencencies
            List<IPackage> dep = new List<IPackage>();
            foreach (var v in Dependencies)
            {
                if (!ProgramDetector.IsVoukoderComponentInstalled(v))
                {
                    OnInstallProgress(new ProcessStatusEventArgs($"Downloading package dependency  {v.Type} {v.Name}..."));
                    packagePath = Path.GetTempPath() + v.DownloadUrl.Segments[v.DownloadUrl.Segments.Length - 1];
                    await _webclient.DownloadFileTaskAsync(v.DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
                    {
                        ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                        OnDownloadProgressChanged(a);
                    }));
                    dep.Add(new Package(v.Name, packagePath));
                }
            }
            p.Dependencies = dep;
            DownloadedPackage = p;
            OnInstallProgress(new ProcessStatusEventArgs("Download finished"));
            return p;
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