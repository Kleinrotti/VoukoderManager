using System;
using System.Threading.Tasks;

namespace VoukoderManager.GUI.Models
{
    public interface IDownloadEntry
    {
        public Uri DownloadUrl { get; set; }

        Task<IPackage> StartPackageDownload();

        Task<IPackage> StartPackageDownloadWithDependencies();

        void StopPackageDownload();
    }
}