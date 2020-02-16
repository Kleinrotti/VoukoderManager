namespace VoukoderManager.Core.Models
{
    public interface IProgramUninstall
    {
        void UninstallPackage(bool includeDependency);

        void StopUninstallPackage();
    }
}