using System.Collections.Generic;

namespace VoukoderManager.GUI
{
    public class ProgramDetector
    {
        private string _registryProgramPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public ProgramEntry GetInstalledProgram(string name)
        {
            return null;
        }

        public List<ProgramEntry> GetInstalledPrograms()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<ProgramEntry> list = new List<ProgramEntry>();
            foreach (ProgramEntry e in programs)
            {
                if (e.ProgramName.Contains("Adobe Premiere") || e.ProgramName.Contains("Adobe Media Encoder") || e.ProgramName.Contains("VEGAS Pro"))
                {
                    list.Add(e);
                }
            }
            return list;
        }

        public List<ProgramEntry> GetInstalledVoukoderComponents()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<ProgramEntry> list = new List<ProgramEntry>();
            foreach (ProgramEntry e in programs)
            {
                if (e.ProgramName.Contains("Voukoder") || e.ProgramName.Contains("connector"))
                {
                    list.Add(e);
                }
            }
            return list;
        }
    }
}