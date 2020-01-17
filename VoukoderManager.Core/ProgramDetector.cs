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
        public static List<IProgramEntry> GetInstalledPrograms(bool includeConnector)
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
                    {
                        e.Type = ProgramType.Premiere;
                        if (includeConnector)
                            e.VoukoderConnector = GetVoukoderConnector(ProgramType.VoukoderConnectorPremiere);
                    }
                    else if (e.Name.Contains("Media Encoder"))
                    {
                        e.Type = ProgramType.MediaEncoder;
                        if (includeConnector)
                            e.VoukoderConnector = GetVoukoderConnector(ProgramType.VoukoderConnectorPremiere);
                    }
                    else if (e.Name.Contains("Effects"))
                    {
                        e.Type = ProgramType.AfterEffects;
                        if (includeConnector)
                            e.VoukoderConnector = GetVoukoderConnector(ProgramType.VoukoderConnectorAfterEffects);
                    }
                    else
                    {
                        e.Type = ProgramType.VEGAS;
                        if (includeConnector)
                            e.VoukoderConnector = GetVoukoderConnector(ProgramType.VoukoderConnectorVegas);
                    }
                    list.Add(e);
                }
            }
            return list;
        }

        public static IProgramEntry GetVoukoderConnector(ProgramType connectorType)
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
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

                    if (e.Type == connectorType)
                        return e;
                }
            }
            return null;
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