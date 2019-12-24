using System;

namespace VoukoderManager.Core.Models
{
    public class InstallEventArgs : EventArgs
    {
        public IVoukoderEntry PackageToInstall { get; set; }
        public IVoukoderEntry PackageDependency { get; set; }
    }
}