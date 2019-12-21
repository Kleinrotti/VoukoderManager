using System;

namespace VoukoderManager.GUI.Models
{
    /// <summary>
    /// Provides events for derived classes to update the progress
    /// </summary>
    public abstract class Operation
    {
        public static event EventHandler<ProcessStatusEventArgs> InstallProgressChanged;

        protected void OnInstallProgress(ProcessStatusEventArgs e)
        {
            InstallProgressChanged?.Invoke(this, e);
        }
    }
}