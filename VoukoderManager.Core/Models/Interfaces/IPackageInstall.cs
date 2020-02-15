namespace VoukoderManager.Core.Models
{
    public interface IPackageInstall
    {
        void InstallPackage(IEntry nle);

        void InstallPackageWithDepenencies(IEntry nle);

        void StopInstallation();
    }
}