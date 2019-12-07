using Octokit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    internal class PackageManager : IPackageManager<IPackage>
    {
        private List<IPackage> _packages;
        private WebClient _webclient;
        private readonly string _downloadPath = Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
        public List<IPackage> Query { get => _packages; }

        public event AsyncCompletedEventHandler DownloadFinished;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public PackageManager()
        {
            _webclient = new WebClient();
        }

        public void StartDownloadPackage(Uri url)
        {
            _webclient.DownloadProgressChanged += DownloadProgressChanged;
            _webclient.DownloadFileCompleted += DownloadFinished;
            _webclient.DownloadFileAsync(url, _downloadPath);
        }

        public List<IVoukoderEntry> GetDownloadablePackages(VoukoderType type)
        {
            var lst = new List<IVoukoderEntry>();
            string repo;
            var client = new GitHubClient(new ProductHeaderValue("voukodermanager"));

            if (type == VoukoderType.VoukoderCore)
                repo = "voukoder";
            else if (type == VoukoderType.VoukoderConnectorPremiere)
                repo = "voukoder-connectors";
            else if (type == VoukoderType.VoukoderConnectorVegas)
                repo = "voukoder-connectors";
            else
                repo = "voukoder-connectors";

            var releases = client.Repository.Release.GetAll("Vouk", repo).Result;
            int i = 0;
            foreach (var f in releases)
            {
                if (i >= 5)
                    break;
                var latest = f;
                lst.Add(new VoukoderEntry
                {
                    Version = new Models.Version(latest.Name, latest.Prerelease),
                    DownloadUrl = new Uri(latest.Assets[0].BrowserDownloadUrl)
                });
                i++;
            }
            return lst;
        }

        public void StopDownloadPackage()
        {
            if (!_webclient.IsBusy)
                return;
            _webclient.CancelAsync();
            _webclient.DownloadProgressChanged -= DownloadProgressChanged;
            _webclient.DownloadFileCompleted -= DownloadFinished;
        }

        public void InstallPackage(IPackage package)
        {
        }

        public void UninstallPackage(IPackage package)
        {
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