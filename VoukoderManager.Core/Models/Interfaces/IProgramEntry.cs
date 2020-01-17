using System.Windows.Media.Imaging;

namespace VoukoderManager.Core.Models
{
    public interface IProgramEntry : IEntry, IProgramUninstall
    {
        /// <summary>
        /// Location where the program is installed to
        /// </summary>
        string InstallationPath { get; set; }

        /// <summary>
        /// Determines whether it's a windows installer or not
        /// </summary>
        bool WindowsInstaller { get; set; }

        /// <summary>
        /// Installation date of the program
        /// </summary>
        string InstallationDate { get; set; }

        /// <summary>
        /// Command to uninstall the program
        /// </summary>
        string UninstallString { get; set; }

        /// <summary>
        /// Command to modify the program
        /// </summary>
        string ModifyPath { get; set; }

        /// <summary>
        /// Publisher of the program
        /// </summary>
        string Publisher { get; set; }

        /// <summary>
        /// Type of the component
        /// </summary>
        ProgramType Type { get; set; }

        /// <summary>
        /// Programlogo
        /// </summary>
        BitmapImage Logo { get; }

        IProgramEntry VoukoderConnector { get; set; }
    }
}