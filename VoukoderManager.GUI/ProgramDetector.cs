using System.Collections.Generic;

namespace VoukoderManager.GUI
{
    public class ProgramDetector
    {
        private string _registryProgramPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public Entry GetInstalledProgram(string name)
        {
            return null;
        }

        public List<Entry> GetInstalledPrograms()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<Entry> list = new List<Entry>();
            foreach (Entry e in programs)
            {
                if (e.ProgramName.Contains("Adobe Premiere") || e.ProgramName.Contains("Adobe Media Encoder") || e.ProgramName.Contains("VEGAS Pro"))
                {
                    list.Add(e);
                }
            }
            return list;
        }

        public List<Entry> GetInstalledVoukoderComponents()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<Entry> list = new List<Entry>();
            foreach (Entry e in programs)
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