using System;

namespace VoukoderManager.GUI.Models
{
    public class InstallEventArgs : EventArgs
    {
        public IVoukoderEntry PackageToInstall { get; set; }
        public IVoukoderEntry PackageDependency { get; set; }
    }
}
