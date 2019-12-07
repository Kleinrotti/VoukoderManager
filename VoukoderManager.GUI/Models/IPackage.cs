namespace VoukoderManager.GUI.Models
{
    public interface IPackage
    {
        public string PackageName { get; set; }

        public bool Certified { get; }

        public string Path { get; set; }

        public PackageType Type { get; }

        public IVersion Version { get; }
    }
}