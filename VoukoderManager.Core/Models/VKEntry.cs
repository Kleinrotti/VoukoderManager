﻿using Serilog;
using System;

namespace VoukoderManager.Core.Models
{
    public abstract class VKEntry : IEntry
    {
        public virtual string Name { get; protected set; }
        public virtual IVersion Version { get; protected set; }
        public virtual ProgramType ComponentType { get; set; }
        public virtual bool Hide { get; set; }

        public VKEntry(string name, IVersion version)
        {
            Log.Verbose($"Creating VKEntry {name} Version: {version.PackageVersion}");
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Current status message of a installation process
        /// </summary>
        public static event EventHandler<ProcessStatusEventArgs> OperationStatus;

        protected void OnOperationStatusChanged(ProcessStatusEventArgs e)
        {
            Log.Debug($"OperationStatusChanged of {e.ComponentType} with message: {e.StatusMessage}");
            OperationStatus?.Invoke(this, e);
        }

        /// <summary>
        /// Returns the name with replaced space and dots
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name.Replace(' ', '_').Replace('.', '_');
        }
    }
}