using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace VoukoderManager.GUI
{
    public interface IPackageManager<T> : IDisposable
    {
        public event AsyncCompletedEventHandler DownloadFinished;

        public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        public List<T> Query { get; }

        public void AddToQuery(T package);

        public void RemoveFromQuery(T package);

        public void ProcessQuery();

        public void StartDownloadPackage(Uri url);

        public void StopDownloadPackage();

        public void InstallPackage(T package);

        public void UninstallPackage(T package);
    }
}