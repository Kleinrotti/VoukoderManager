using System.Collections.Generic;

namespace VoukoderManager.Core.Models
{
    public interface IGitHubEntry : IEntry, IDownloadEntry
    {
        string Changelog { get; set; }
        List<IGitHubEntry> Dependencies { get; set; }
    }
}