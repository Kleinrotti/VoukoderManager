namespace VoukoderManager.GUI.Models
{
    public class ProgramEntry : IProgramEntry
    {
        public string DisplayName { get; set; }
        public string InstallationPath { get; set; }
        public IVersion DisplayVersion { get; set; }
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