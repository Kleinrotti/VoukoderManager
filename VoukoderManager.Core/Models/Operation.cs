using System;

namespace VoukoderManager.Core.Models
{
    /// <summary>
    /// Provides events for derived classes to update the progress
    /// </summary>
    public abstract class Operation
    {
        public static event EventHandler<ProcessStatusEventArgs> OperationStatus;

        protected void OnOperationStatusChanged(ProcessStatusEventArgs e)
        {
            OperationStatus?.Invoke(this, e);
        }
    }
}