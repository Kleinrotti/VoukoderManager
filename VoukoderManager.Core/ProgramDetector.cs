using System.Collections.Generic;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    /// <summary>
    /// Class for detecting installed programs on your operating system
    /// </summary>
    public class ProgramDetector
    {
        private static string _registryProgramPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        /// <summary>
        /// Returns a list which contains all installed programs where voukoder components are availible for
        /// </summary>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledPrograms()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<IProgramEntry> list = new List<IProgramEntry>();
            foreach (IProgramEntry e in programs)
            {
                if (e.Name.Contains("Adobe Premiere") || e.Name.Contains("Adobe Media Encoder")
                    || e.Name.Contains("VEGAS Pro")
                    || e.Name.Contains("Adobe After Effects"))
                {
                    if (e.Name.Contains("Premiere"))
                        e.Type = ProgramType.Premiere;
                    else if (e.Name.Contains("Encoder"))
                        e.Type = ProgramType.Premiere;
                    else if (e.Name.Contains("Effects"))
                        e.Type = ProgramType.AfterEffects;
                    else
                        e.Type = ProgramType.VEGAS;
                    list.Add(e);
                }
            }
            return list;
        }

        /// <summary>
        /// Returns a list which contains all installed Voukoder components
        /// </summary>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledVoukoderComponents()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<IProgramEntry> list = new List<IProgramEntry>();
            foreach (IProgramEntry e in programs)
            {
                if (e.Name.Contains("Voukoder"))
                {
                    if (e.Name.Contains("Premiere"))
                        e.Type = ProgramType.VoukoderConnectorPremiere;
                    else if (e.Name.Contains("VEGAS"))
                        e.Type = ProgramType.VoukoderConnectorVegas;
                    else if (e.Name.Contains("AfterEffects"))
                        e.Type = ProgramType.VoukoderConnectorAfterEffects;
                    else
                        e.Type = ProgramType.VoukoderCore;
                    list.Add(e);
                }
            }
            return list;
        }

        public static bool IsVoukoderComponentInstalled(IVoukoderEntry entry)
        {
            var cpn = GetInstalledVoukoderComponents();
            foreach (var v in cpn)
            {
                if (v.Type == entry.Type)
                {
                    return true;
                }
            }
            return false;
        }
    }
}