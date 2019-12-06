using System;

namespace VoukoderManager.GUI.Models
{
    public class VoukoderEntry
    {
        public Uri DownloadUrl { get; set; }
        public string Changelog { get; set; }
        public Version Version { get; set; }
        public VoukoderType Type { get; set; }
    }
}