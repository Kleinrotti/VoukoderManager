namespace VoukoderManager.Core.Models
{
    public interface IEntry
    {
        string Name { get; }
        IVersion Version { get; }
        ProgramType ComponentType { get; set; }
    }
}