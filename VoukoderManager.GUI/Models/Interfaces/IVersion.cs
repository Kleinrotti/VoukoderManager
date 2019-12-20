namespace VoukoderManager.GUI.Models
{
    public interface IVersion
    {
        public string PackageVersion { get; set; }

        public int Major { get; }

        public int Minor { get; }

        public bool PreRelease { get; set; }
    }
}