using System;

namespace VoukoderManager.Core.Models
{
    public interface IVersion : IComparable<IVersion>
    {
        string PackageVersion { get; set; }

        int Major { get; }

        int Minor { get; }

        int Patch { get; }

        bool PreRelease { get; }
    }
}