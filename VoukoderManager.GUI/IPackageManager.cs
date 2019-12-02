using System;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    public interface IPackageManager : IDisposable
    {
        public void DownloadPackage(Uri url);
        public void InstallPackage(Package package);
        public void UninstallPackage(Package package);
    }
}
