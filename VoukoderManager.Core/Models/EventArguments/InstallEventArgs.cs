using System;

namespace VoukoderManager.Core.Models
{
    public class InstallEventArgs : EventArgs
    {
        public IGitHubEntry PackageToInstall { get; set; }
        public IGitHubEntry PackageDependency { get; set; }
    }
}