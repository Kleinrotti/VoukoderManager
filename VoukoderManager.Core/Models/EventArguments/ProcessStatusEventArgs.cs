using System;

namespace VoukoderManager.Core.Models
{
    public class ProcessStatusEventArgs : EventArgs
    {
        public string StatusMessage { get; set; }
        public ProgramType ComponentType { get; set; }

        public ProcessStatusEventArgs()
        {
        }

        public ProcessStatusEventArgs(string message)
        {
            StatusMessage = message;
        }

        public ProcessStatusEventArgs(string message, ProgramType componentType)
        {
            StatusMessage = message;
            ComponentType = componentType;
        }
    }
}