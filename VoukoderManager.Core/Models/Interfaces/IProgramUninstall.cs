namespace VoukoderManager.Core.Models
{
    public interface IProgramUninstall
    {
        void UninstallPackage();

        void StopUninstallPackage();
    }
}