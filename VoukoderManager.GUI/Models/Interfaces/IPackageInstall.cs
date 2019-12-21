namespace VoukoderManager.GUI.Models
{
    public interface IPackageInstall
    {
        public void InstallPackage();

        public void InstallPackageWithDepenencies();

        public void StopInstallation();
    }
}