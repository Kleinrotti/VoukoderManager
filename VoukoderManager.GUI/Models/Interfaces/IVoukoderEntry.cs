using System.Collections.Generic;

namespace VoukoderManager.GUI.Models
{
    public interface IVoukoderEntry : IEntry, IDownloadEntry
    {
        public string Changelog { get; set; }
        public ProgramType Type { get; set; }
        public List<IVoukoderEntry> Dependencies { get; set; }
    }
}