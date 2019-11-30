namespace VoukoderManager.GUI.Models
{
    public class ProgramEntry
    {
        /// <summary>
        /// Displayname of the program
        /// </summary>
        public string ProgramName { get; set; }
        /// <summary>
        /// Location where the program is installed to
        /// </summary>
        public string InstallationPath { get; set; }
        /// <summary>
        /// Version of the installed program
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Determines whether it's a windows installer or not
        /// </summary>
        public bool WindowsInstaller { get; protected set; }

        public ProgramEntry(string programName, string installationPath, string version)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
        }
        public ProgramEntry(string programName, string installationPath, string version, bool windowsInstaller)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
            WindowsInstaller = windowsInstaller;
        }
    }
}