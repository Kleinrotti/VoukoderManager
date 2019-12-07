namespace VoukoderManager.GUI.Models
{
    public class ProgramEntry : IProgramEntry
    {
        public string Name { get; set; }
        public string InstallationPath { get; set; }
        public IVersion Version { get; set; }
        public bool WindowsInstaller { get; set; }
        public string InstallationDate { get; set; }
        public string UninstallString { get; set; }
        public string ModifyPath { get; set; }
        public string Publisher { get; set; }

        public ProgramEntry(string programName, string installationPath, string version)
        {
            Name = programName;
            InstallationPath = installationPath;
            Version = new Version(version, CheckPreRelease(version));
        }

        public ProgramEntry(string programName, string installationPath, string version, bool windowsInstaller)
        {
            Name = programName;
            InstallationPath = installationPath;
            Version = new Version(version, CheckPreRelease(version));
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