using System;

namespace VoukoderManager.GUI.Models
{
    /// <summary>
    /// Describes a downloadable Voukoder component
    /// </summary>
    public class VoukoderEntry : IVoukoderEntry
    {
        public Uri DownloadUrl { get; set; }
        public string Changelog { get; set; }
        public IVersion Version { get; set; }
        public VoukoderType Type { get; set; }
        public string Name { get; set; }
    }
}