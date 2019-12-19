using System;

namespace VoukoderManager.GUI.Models
{
    public abstract class Operation
    {
        public static event EventHandler<ProcessStatusEventArgs> InstallProgressChanged;

        protected void OnInstallProgress(ProcessStatusEventArgs e)
        {
            InstallProgressChanged?.Invoke(this, e);
        }
    }
}