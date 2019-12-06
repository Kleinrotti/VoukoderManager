namespace VoukoderManager.GUI.Models
{
    public class ProgramEntry
    {
        /// <summary>
        /// Displayname of the program
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Location where the program is installed to
        /// </summary>
        public string InstallationPath { get; set; }

        /// <summary>
        /// Version of the installed program
        /// </summary>
        public Version DisplayVersion { get; set; }

        /// <summary>
        /// Determines whether it's a windows installer or not
        /// </summary>
        public bool WindowsInstaller { get; set; }

        public string InstallationDate { get; set; }
        public string UninstallString { get; set; }
        public string ModifyPath { get; set; }
        public string Publisher { get; set; }

        public ProgramEntry(string programName, string installationPath, string version)
        {
            DisplayName = programName;
            InstallationPath = installationPath;
            DisplayVersion = new Version(version, CheckPreRelease(version));
        }

        public ProgramEntry(string programName, string installationPath, string version, bool windowsInstaller)
        {
            DisplayName = programName;
            InstallationPath = installationPath;
            DisplayVersion = new Version(version, CheckPreRelease(version));
            WindowsInstaller = windowsInstaller;
        }

        public ProgramEntry()
        {
        }

        private bool CheckPreRelease(string version)
        {
            if (version.Contains("rc") || version.Contains("beta"))
                return true;
            else
                return false;
        }
    }
}