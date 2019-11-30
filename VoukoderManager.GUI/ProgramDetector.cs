using System.Collections.Generic;
using VoukoderManager.GUI.Models;

namespace VoukoderManager.GUI
{
    /// <summary>
    /// Class for detecting installed programs on your operating system
    /// </summary>
    public class ProgramDetector
    {
        private string _registryProgramPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        public ProgramEntry GetInstalledProgram(string name)
        {
            return null;
        }

        /// <summary>
        /// Returns a list which contains all installed programs where voukoder components are availible for
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a list which contains all installed Voukoder components
        /// </summary>
        /// <returns></returns>
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