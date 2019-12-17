﻿using System;

namespace VoukoderManager.GUI.Models
{
    public interface IVoukoderEntry : IEntry
    {
        public Uri DownloadUrl { get; set; }
        public string Changelog { get; set; }
        public ProgramType Type { get; set; }
    }
}