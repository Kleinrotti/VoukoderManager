namespace VoukoderManager.Core.Models
{
    public interface IPackageInstall
    {
        void InstallPackage();

        void InstallPackageWithDepenencies();

        void StopInstallation();
    }
}