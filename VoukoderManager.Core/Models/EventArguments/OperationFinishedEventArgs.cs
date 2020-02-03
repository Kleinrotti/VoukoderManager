using System;

namespace VoukoderManager.Core.Models
{
    public class OperationFinishedEventArgs
    {
        public bool Cancelled { get; }
        public Exception Error { get; }
        public IEntry Entry { get; }
        public OperationType OperationType { get; }

        public OperationFinishedEventArgs(Exception error, bool cancelled, IEntry entry, OperationType operationType)
        {
            Cancelled = cancelled;
            Error = error;
            Entry = entry;
            OperationType = operationType;
        }

        public OperationFinishedEventArgs(IEntry entry, OperationType operationType)
        {
            Entry = entry;
            OperationType = operationType;
        }

        public override string ToString()
        {
            return "Operation: " + OperationType.ToString() + $" finished of package {Entry.Name} Cancelled: {Cancelled.ToString()}";
        }
    }
}