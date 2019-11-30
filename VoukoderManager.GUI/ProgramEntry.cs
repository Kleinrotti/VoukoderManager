namespace VoukoderManager.GUI
{
    public class ProgramEntry
    {
        public string ProgramName { get; set; }
        public string InstallationPath { get; set; }
        public string Version { get; set; }
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