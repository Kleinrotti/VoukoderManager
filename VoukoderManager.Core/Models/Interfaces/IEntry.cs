namespace VoukoderManager.Core.Models
{
    public interface IEntry
    {
        string Name { get; set; }
        IVersion Version { get; }
    }
}