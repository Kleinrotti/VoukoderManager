using System.Collections.Generic;

namespace VoukoderManager.Core.Models
{
    public interface IVoukoderEntry : IEntry, IDownloadEntry
    {
        string Changelog { get; set; }
        ProgramType Type { get; set; }
        List<IVoukoderEntry> Dependencies { get; set; }
    }
}