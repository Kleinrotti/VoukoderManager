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
        public List<IProgramEntry> GetInstalledPrograms()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<IProgramEntry> list = new List<IProgramEntry>();
            foreach (IProgramEntry e in programs)
            {
                if (e.Name.Contains("Adobe Premiere") || e.Name.Contains("Adobe Media Encoder") || e.Name.Contains("VEGAS Pro"))
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
        public List<IProgramEntry> GetInstalledVoukoderComponents()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<IProgramEntry> list = new List<IProgramEntry>();
            foreach (IProgramEntry e in programs)
            {
                if (e.Name.Contains("Voukoder") || e.Name.Contains("connector"))
                {
                    list.Add(e);
                }
            }
            return list;
        }
    }
}