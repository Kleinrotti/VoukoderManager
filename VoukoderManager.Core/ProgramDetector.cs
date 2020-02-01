using System.Collections.Generic;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    /// <summary>
    /// Class for detecting installed programs on your operating system
    /// </summary>
    public static class ProgramDetector
    {
        private static string _registryProgramPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private static string _premierePluginsDir = @"SOFTWARE\Adobe\Premiere Pro\CurrentVersion";
        private static string _afterEffectsPluginsDir = @"SOFTWARE\Adobe\After Effects\CurrentVersion";
        private static string _vegasPluginsDir = @"SOFTWARE\Sony Creative Software\VEGAS Pro\17.0";

        /// <summary>
        /// Returns a list which contains all installed programs where voukoder components are availible for
        /// </summary>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledPrograms(bool includeConnector)
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            List<IProgramEntry> list = new List<IProgramEntry>();
            IProgramEntry pro;
            foreach (RegistryEntry pr in programs)
            {
                if (pr.DisplayName.Contains("Adobe Premiere") || pr.DisplayName.Contains("Adobe Media Encoder")
                    || pr.DisplayName.Contains("VEGAS Pro")
                    || pr.DisplayName.Contains("Adobe After Effects"))
                {
                    if (pr.DisplayName.Contains("Premiere"))
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.Premiere);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.VoukoderConnectorPremiere);
                    }
                    else if (pr.DisplayName.Contains("Media Encoder"))
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.MediaEncoder);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.VoukoderConnectorPremiere);
                    }
                    else if (pr.DisplayName.Contains("Effects"))
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.AfterEffects);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.VoukoderConnectorAfterEffects);
                    }
                    else
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.VEGAS);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.VoukoderConnectorVegas);
                    }
                    list.Add(pro);
                }
            }

            void FillComponent(ref IProgramEntry entry, ProgramType componentType)
            {
                entry.VoukoderComponent = GetVoukoderComponent(componentType);
                if (entry.VoukoderComponent != null)
                    entry.VoukoderComponent.VoukoderComponent = GetVoukoderComponent(ProgramType.VoukoderCore);
            }
            return list;
        }

        private static void ConvertFromRegistryEntry(out IProgramEntry entry, RegistryEntry regEntry, ProgramType type)
        {
            entry = new VKProgramEntry(regEntry.DisplayName, new Version(regEntry.DisplayVersion, regEntry.PreRelease));
            entry.ComponentType = type;
            entry.InstallationDate = regEntry.InstallationDate;
            entry.InstallationPath = regEntry.InstallationPath;
            entry.ModifyPath = regEntry.ModifyPath;
            entry.Publisher = regEntry.Publisher;
            entry.UninstallString = regEntry.UninstallString;
            entry.WindowsInstaller = regEntry.WindowsInstaller;
        }

        /// <summary>
        /// Returns the connector for the given type if installed on the system
        /// </summary>
        /// <param name="connectorType"></param>
        /// <returns></returns>
        public static IProgramEntry GetVoukoderComponent(ProgramType connectorType)
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            IProgramEntry entry;
            foreach (RegistryEntry e in programs)
            {
                if (e.DisplayName.Contains("Voukoder"))
                {
                    if (e.DisplayName.Contains("Premiere"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderConnectorPremiere);
                    else if (e.DisplayName.Contains("VEGAS"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderConnectorVegas);
                    else if (e.DisplayName.Contains("AfterEffects"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderConnectorAfterEffects);
                    else
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderCore);

                    if (entry.ComponentType == connectorType)
                        return entry;
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
            IProgramEntry entry;
            foreach (RegistryEntry e in programs)
            {
                if (e.DisplayName.Contains("Voukoder"))
                {
                    if (e.DisplayName.Contains("Premiere"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderConnectorPremiere);
                    else if (e.DisplayName.Contains("VEGAS"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderConnectorVegas);
                    else if (e.DisplayName.Contains("AfterEffects"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderConnectorAfterEffects);
                    else
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderCore);
                    list.Add(entry);
                }
            }
            return list;
        }

        public static bool IsVoukoderComponentInstalled(IGitHubEntry entry)
        {
            var cpn = GetInstalledVoukoderComponents();
            foreach (var v in cpn)
            {
                if (v.ComponentType == entry.ComponentType)
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetPluginsDir(ProgramType programType)
        {
            if (programType == ProgramType.Premiere || programType == ProgramType.MediaEncoder
                || programType == ProgramType.VoukoderConnectorPremiere)
                return RegistryHelper.GetHKEYLocalValue(_premierePluginsDir, "Plug-InsDir");
            else if (programType == ProgramType.AfterEffects || programType == ProgramType.VoukoderConnectorAfterEffects)
                return RegistryHelper.GetHKEYLocalValue(_afterEffectsPluginsDir, "Plug-InsDir");
            else
                return RegistryHelper.GetHKEYLocalValue(_vegasPluginsDir, "InstallPath");
        }
    }
}