namespace VoukoderManager.GUI.Models
{
    public interface IProgramEntry : IEntry
    {
        /// <summary>
        /// Location where the program is installed to
        /// </summary>
        public string InstallationPath { get; set; }

        /// <summary>
        /// Determines whether it's a windows installer or not
        /// </summary>
        public bool WindowsInstaller { get; set; }

        /// <summary>
        /// Installation date of the program
        /// </summary>
        public string InstallationDate { get; set; }

        /// <summary>
        /// Command to uninstall the program
        /// </summary>
        public string UninstallString { get; set; }

        /// <summary>
        /// Command to modify the program
        /// </summary>
        public string ModifyPath { get; set; }

        /// <summary>
        /// Publisher of the program
        /// </summary>
        public string Publisher { get; set; }
    }
}