namespace VoukoderManager.GUI
{
    internal class VoukoderEntry : ProgramEntry
    {
        public string UninstallString { get; set; }
        public VoukoderEntry(string programName, string installationPath, string version) : base(programName, installationPath, version)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
        }
        public VoukoderEntry(string programName, string installationPath, string version, bool windowsInstaller) : base(programName, installationPath, version, windowsInstaller)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
            WindowsInstaller = windowsInstaller;
        }
        public VoukoderEntry(string programName, string installationPath, string version, string uninstallString) : base(programName, installationPath, version)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
            UninstallString = uninstallString;
        }
        public VoukoderEntry(string programName, string installationPath, string version, string uninstallString, bool windowsInstaller) : base(programName, installationPath, version, windowsInstaller)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
            UninstallString = uninstallString;
            WindowsInstaller = windowsInstaller;
        }
    }
}