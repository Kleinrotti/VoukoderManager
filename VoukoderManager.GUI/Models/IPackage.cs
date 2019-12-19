namespace VoukoderManager.GUI.Models
{
    public interface IPackage : IEntry
    {
        public bool Certified { get; }

        public string Path { get; set; }

        public PackageType Type { get; }

        public IPackage Dependency { get; set; }
    }
}