using System;

namespace VoukoderManager.Core.Models
{
    public class ProcessStatusEventArgs : EventArgs
    {
        public string StatusMessage { get; set; }

        public ProcessStatusEventArgs()
        {
        }

        public ProcessStatusEventArgs(string message)
        {
            StatusMessage = message;
        }
    }
}