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
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Current status message of a installation process
        /// </summary>
        public static event EventHandler<ProcessStatusEventArgs> OperationStatus;

        protected void OnOperationStatusChanged(ProcessStatusEventArgs e)
        {
            OperationStatus?.Invoke(this, e);
        }
    }
}