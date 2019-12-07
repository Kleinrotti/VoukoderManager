namespace VoukoderManager.GUI.Models
{
    public interface IEntry
    {
        public string Name { get; set; }
        public IVersion Version { get; }
    }
}