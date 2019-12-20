using System;
using System.Threading.Tasks;

namespace VoukoderManager.GUI.Models
{
    public interface IDownloadEntry
    {
        public Uri DownloadUrl { get; set; }

        Task StartPackageDownload();

        Task StartPackageDownloadWithDependencies();

        void StopPackageDownload();
    }
}