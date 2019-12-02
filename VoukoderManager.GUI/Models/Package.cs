namespace VoukoderManager.GUI.Models
{
    public class Package
    {
        public string PackageName { get; set; }
        //TODO: check if downloaded package (msi or exe) has a signature
        public bool Certified { get; set; }
        public string Path { get; set; }
        public PackageType Type { get; set; }

    }
}
