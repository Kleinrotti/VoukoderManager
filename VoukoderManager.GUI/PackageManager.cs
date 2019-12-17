using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    internal class PackageManager
    {
        private List<IPackage> _packages;
        private WebClient _webclient;
        public List<IPackage> Query { get => _packages; }

        public event AsyncCompletedEventHandler DownloadFinished;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public static event EventHandler<ProcessStatusEventArgs> InstallProgressChanged;

        public PackageManager()
        {
            _webclient = new WebClient();
        }

        /// <summary>
        /// Start download of the file. Returns the download path.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IPackage StartDownloadPackage(Uri url)
        {
            OnInstallProgress(new ProcessStatusEventArgs("Downloading files"));
            var path = Path.GetTempPath() + url.Segments[url.Segments.Length - 1];
            _webclient.DownloadProgressChanged += DownloadProgressChanged;
            _webclient.DownloadFileCompleted += DownloadFinished;
            _webclient.DownloadFileAsync(url, path);
            return new Package(url.Segments[url.Segments.Length - 1], path);
        }

        public IPackage StartDownloadPackage(IVoukoderEntry entry)
        {
            OnInstallProgress(new ProcessStatusEventArgs("Downloading files"));
            var path = Path.GetTempPath() + entry.DownloadUrl.Segments[entry.DownloadUrl.Segments.Length - 1];
            _webclient.DownloadProgressChanged += DownloadProgressChanged;
            _webclient.DownloadFileCompleted += DownloadFinished;
            _webclient.DownloadFileAsync(entry.DownloadUrl, path);
            return new Package(entry.DownloadUrl.Segments[entry.DownloadUrl.Segments.Length - 1], path);
        }

        private void OnInstallProgress(ProcessStatusEventArgs e)
        {
            InstallProgressChanged?.Invoke(this, e);
        }

        public List<IVoukoderEntry> GetDownloadablePackages(ProgramType type)
        {
            var lst = new List<IVoukoderEntry>();
            string repo;
            string repopath;
            var client = new GitHubClient(new ProductHeaderValue("voukodermanager"));

            if (type == ProgramType.VoukoderCore)
            {
                repo = "voukoder";
                GetReleases();
            }
            else
            {
                repo = "voukoder-connectors";
                if (type == ProgramType.VoukoderConnectorVegas)
                {
                    repopath = "vegas";
                }
                else if (type == ProgramType.VoukoderConnectorAfterEffects)
                {
                    repopath = "aftereffects";
                }
                else
                {
                    repopath = "premiere";
                }
                GetContent();
            }

            void GetContent()
            {
                var test = client.Repository.Content.GetAllContents("Vouk", repo, repopath).Result;

                int i = 0;
                foreach (var v in test)
                {
                    if (v.Name.Contains("connector"))
                    {
                        if (i >= 5)
                            break;
                        var version = v.Name.Split('.');
                        lst.Add(new VoukoderEntry
                        {
                            Name = v.Name,
                            DownloadUrl = new Uri(v.DownloadUrl),
                            Version = new Models.Version(version[version.Length - 1])
                        });
                        i++;
                    }
                }
            }

            void GetReleases()
            {
                var releases = client.Repository.Release.GetAll("Vouk", repo).Result;
                int i = 0;
                foreach (var f in releases)
                {
                    if (i >= 5)
                        break;
                    lst.Add(new VoukoderEntry
                    {
                        Name = f.Name,
                        Version = new Models.Version(f.Name, f.Prerelease),
                        DownloadUrl = new Uri(f.Assets[0].BrowserDownloadUrl)
                    });
                    i++;
                }
            }
            return lst;
        }

        public void StopDownloadPackage()
        {
            OnInstallProgress(new ProcessStatusEventArgs("Stopping download..."));
            if (!_webclient.IsBusy)
                return;
            _webclient.CancelAsync();
            _webclient.DownloadProgressChanged -= DownloadProgressChanged;
            _webclient.DownloadFileCompleted -= DownloadFinished;
            OnInstallProgress(new ProcessStatusEventArgs("Download stopped"));
        }

        public void InstallPackage(IPackage package)
        {
            OnInstallProgress(new ProcessStatusEventArgs("Installing: " + package.Name));
            Process p = new Process();
            var startinfo = new ProcessStartInfo("msiexec.exe")
            {
                UseShellExecute = true,
                Arguments = @" /i " + package.Path + @" /qn /log install.log",
                Verb = "runas"
            };
            p.StartInfo = startinfo;
            p.Start();
            p.WaitForExit();
            p.Dispose();
            OnInstallProgress(new ProcessStatusEventArgs("Finished installation of package: " + package.Name));
        }

        public void UninstallPackage(IProgramEntry package)
        {
            OnInstallProgress(new ProcessStatusEventArgs("Unistalling: " + package.Name));
            Process p = new Process();
            var startinfo = new ProcessStartInfo("msiexec.exe")
            {
                UseShellExecute = true,
                Arguments = package.UninstallString.Split(' ')[1],
                Verb = "runas"
            };
            p.StartInfo = startinfo;
            p.Start();
            p.WaitForExit();
            p.Dispose();
            OnInstallProgress(new ProcessStatusEventArgs("Finished uninstall of package: " + package.Name));
        }

        public void AddToQuery(IPackage package)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromQuery(IPackage package)
        {
            throw new NotImplementedException();
        }

        public void ProcessQuery()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _webclient.Dispose();
                    _packages = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PackageManager()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}