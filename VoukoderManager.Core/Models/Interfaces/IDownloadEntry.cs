using System;
using System.Threading.Tasks;

namespace VoukoderManager.Core.Models
{
    public interface IDownloadEntry
    {
        Uri DownloadUrl { get; set; }

        Task<IPackage> StartPackageDownload();

        Task<IPackage> StartPackageDownloadWithDependencies();

        void StopPackageDownload();
    }
}