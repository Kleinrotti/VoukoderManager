using System;

namespace VoukoderManager.Core.Models
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

        public override string ToString()
        {
            return $"Operation finished of package {Entry.Name} Cancelled: {Cancelled.ToString()}";
        }
    }
}