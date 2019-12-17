using System;

namespace VoukoderManager.GUI.Models
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