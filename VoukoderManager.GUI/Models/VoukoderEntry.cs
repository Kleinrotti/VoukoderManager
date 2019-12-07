using System;

namespace VoukoderManager.GUI.Models
{
    public class VoukoderEntry : IVoukoderEntry
    {
        public Uri DownloadUrl { get; set; }
        public string DisplayName { get; set; }
        public string Changelog { get; set; }
        public IVersion Version { get; set; }
        public VoukoderType Type { get; set; }
    }
}