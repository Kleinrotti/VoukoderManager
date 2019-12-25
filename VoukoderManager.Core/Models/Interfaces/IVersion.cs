namespace VoukoderManager.Core.Models
{
    public interface IVersion
    {
        string PackageVersion { get; set; }

        int Major { get; }

        int Minor { get; }

        bool PreRelease { get; set; }
    }
}