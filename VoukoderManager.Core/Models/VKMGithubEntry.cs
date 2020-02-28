using Serilog;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace VoukoderManager.Core.Models
{
    public class VKMGithubEntry : VKGithubEntry
    {
        private string _destination;

        public string DownloadDestination { get { return _destination; } set { _destination = value; } }

        public VKMGithubEntry(string name, IVersion version, string downloadDestination) : base(name, version)
        {
            Name = name;
            version = Version;
            _destination = downloadDestination;
        }

        public override async Task<IPackage> StartPackageDownload()
        {
            Log.Debug("Starting download of package", this);
            OnOperationStatusChanged(new ProcessStatusEventArgs("Downloading files...", ComponentType));
            string packagePath = _destination + DownloadUrl.Segments[DownloadUrl.Segments.Length - 1];
            await _webclient.DownloadFileTaskAsync(DownloadUrl, packagePath, new Progress<Tuple<long, int, long>>(t =>
            {
                ProgressChangedEventArgs a = new ProgressChangedEventArgs(t.Item2, this);
                OnDownloadProgressChanged(a);
            }));
            var pkg = new VKPackage(Name, Version, packagePath, ComponentType);
            OnOperationStatusChanged(new ProcessStatusEventArgs("Download finished", ComponentType));
            return pkg;
        }
    }
}