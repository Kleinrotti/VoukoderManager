namespace VoukoderManager.GUI
{
    public class Entry
    {
        public string ProgramName { get; set; }
        public string InstallationPath { get; set; }
        public string Version { get; set; }

        public Entry(string programName, string installationPath, string version)
        {
            ProgramName = programName;
            InstallationPath = installationPath;
            Version = version;
        }
    }
}