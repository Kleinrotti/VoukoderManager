using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    internal class PackageManager : IPackageManager<Package>
    {
        private List<Package> _packages;
        private WebClient _webclient;
        private const string _downloadPath = "test.iso";
        public List<Package> Query { get => _packages; }

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

        public void StopDownloadPackage()
        {
            if (!_webclient.IsBusy)
                return;
            _webclient.CancelAsync();
            _webclient.DownloadProgressChanged -= DownloadProgressChanged;
            _webclient.DownloadFileCompleted -= DownloadFinished;
        }

        public void InstallPackage(Package package)
        {
        }

        public void UninstallPackage(Package package)
        {
        }

        public void AddToQuery(Package package)
        {
            throw new NotImplementedException();
        }

        public void RemoveFromQuery(Package package)
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