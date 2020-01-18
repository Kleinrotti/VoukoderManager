using System.Collections.Generic;

namespace VoukoderManager.Core.Models
{
    public interface IPackage : IEntry, IPackageInstall
    {
        bool Certified { get; }

        string Path { get; set; }

        PackageType Type { get; }

        ProgramType ComponentType { get; }

        List<IPackage> Dependencies { get; set; }
    }
}