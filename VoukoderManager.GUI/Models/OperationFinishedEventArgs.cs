using System;

namespace VoukoderManager.GUI.Models
{
    public class OperationFinishedEventArgs
    {
        public bool Cancelled { get; }
        public Exception Error { get; }
        public IEntry Entry { get; }

        public OperationFinishedEventArgs(Exception error, bool cancelled, IEntry entry)
        {
            Cancelled = cancelled;
            Error = error;
            Entry = entry;
        }

        public OperationFinishedEventArgs(IEntry entry)
        {
            Entry = entry;
        }
    }
}