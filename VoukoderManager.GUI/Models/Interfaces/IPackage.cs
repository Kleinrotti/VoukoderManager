using System.Collections.Generic;

namespace VoukoderManager.GUI.Models
{
    public interface IPackage : IEntry, IPackageInstall
    {
        public bool Certified { get; }

        public string Path { get; set; }

        public PackageType Type { get; }

        public List<IPackage> Dependencies { get; set; }
    }
}