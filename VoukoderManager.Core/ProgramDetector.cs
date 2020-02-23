using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    /// <summary>
    /// Class for detecting installed programs on your operating system
    /// </summary>
    public static class ProgramDetector
    {
        private static string _registryProgramPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private static string _premierePluginsDir = @"SOFTWARE\Adobe\Premiere Pro\";
        private static string _afterEffectsPluginsDir = @"SOFTWARE\Adobe\After Effects\";
        private static string _vegasPluginsDir = @"SOFTWARE\Sony Creative Software\VEGAS Pro\";

        /// <summary>
        /// Returns a list which contains all installed programs where voukoder components are availible for
        /// </summary>
        /// <param name="includeConnector">include the connector if installed</param>
        /// <param name="onlyNewestVersion">only the newest version of each program</param>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledPrograms(bool includeConnector, bool onlyNewestVersion)
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            Log.Debug("Received registry program list");
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
                            FillComponent(ref pro, ProgramType.Premiere);
                    }
                    else if (pr.DisplayName.Contains("Media Encoder"))
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.MediaEncoder);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.Premiere);
                    }
                    else if (pr.DisplayName.Contains("Effects"))
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.AfterEffects);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.AfterEffects);
                    }
                    else
                    {
                        ConvertFromRegistryEntry(out pro, pr, ProgramType.VEGAS);
                        if (includeConnector)
                            FillComponent(ref pro, ProgramType.VEGAS);
                    }
                    list.Add(pro);
                }
            }

            void FillComponent(ref IProgramEntry entry, ProgramType componentType)
            {
                entry.SubComponent = GetVoukoderComponent(componentType);
                if (entry.SubComponent != null)
                    entry.SubComponent.SubComponent = GetVoukoderComponent(ProgramType.VoukoderCore);
            }
            if (onlyNewestVersion)
            {
                Log.Debug("Removing older versions of same program from list");
                var enumlengh = Enum.GetNames(typeof(ProgramType)).Length;
                //Check for multiple installed versions of each program
                for (int i = 0; i < enumlengh; i++)
                {
                    var items = list.Where(x => (int)x.ComponentType == i);
                    //if more than one version of the same program is installed
                    if (items.Count() > 2)
                    {
                        //search for the newest version of the same program
                        var ii = items.Aggregate((i1, i2) => i1.Version.Major > i2.Version.Major ? i1 : i2);
                        //remove older versions from the list
                        list.RemoveAll(x => x.Version.Major != ii.Version.Major && x.ComponentType == ii.ComponentType);
                    }
                }
            }
            return list;
        }

        private static void ConvertFromRegistryEntry(out IProgramEntry entry, RegistryEntry regEntry, ProgramType type)
        {
            entry = new VKProgramEntry(regEntry.DisplayName, new Models.Version(regEntry.DisplayVersion, regEntry.PreRelease));
            entry.ComponentType = type;
            entry.InstallationDate = regEntry.InstallationDate;
            entry.InstallationPath = regEntry.InstallationPath;
            entry.ModifyPath = regEntry.ModifyPath;
            entry.Publisher = regEntry.Publisher;
            entry.UninstallString = regEntry.UninstallString;
            entry.WindowsInstaller = regEntry.WindowsInstaller;
            if (entry.ComponentType == ProgramType.MediaEncoder)
                entry.Hide = true;
        }

        /// <summary>
        /// Returns the connector for the given type if installed on the system
        /// </summary>
        /// <param name="connectorType"></param>
        /// <returns></returns>
        public static IProgramEntry GetVoukoderComponent(ProgramType connectorType)
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            Log.Debug("Received registry program list");
            IProgramEntry entry;
            foreach (RegistryEntry e in programs)
            {
                if (e.DisplayName.Contains("Voukoder"))
                {
                    if (e.DisplayName.Contains("Premiere"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.Premiere);
                    else if (e.DisplayName.Contains("VEGAS"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VEGAS);
                    else if (e.DisplayName.Contains("AfterEffects"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.AfterEffects);
                    else
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderCore);

                    if (entry.ComponentType == connectorType)
                        return entry;
                }
            }
            Log.Debug("No match for searching voukoder component");
            return null;
        }

        /// <summary>
        /// Returns a list which contains all installed Voukoder components
        /// </summary>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledVoukoderComponents()
        {
            var programs = RegistryHelper.GetPrograms(_registryProgramPath);
            Log.Debug("Received registry program list");
            List<IProgramEntry> list = new List<IProgramEntry>();
            IProgramEntry entry;
            foreach (RegistryEntry e in programs)
            {
                if (e.DisplayName.Contains("Voukoder"))
                {
                    if (e.DisplayName.Contains("Premiere"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.Premiere);
                    else if (e.DisplayName.Contains("VEGAS"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VEGAS);
                    else if (e.DisplayName.Contains("AfterEffects"))
                        ConvertFromRegistryEntry(out entry, e, ProgramType.AfterEffects);
                    else
                        ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderCore);
                    list.Add(entry);
                }
            }
            return list;
        }

        public static bool IsVoukoderComponentInstalled(IEntry entry)
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

        public static string GetPluginsDir(IEntry program)
        {
            if (program.ComponentType == ProgramType.Premiere || program.ComponentType == ProgramType.MediaEncoder)
                return RegistryHelper.GetHKEYLocalValue(_premierePluginsDir + program.Version.Major + ".0", "CommonPluginInstallPath");
            else if (program.ComponentType == ProgramType.AfterEffects || program.ComponentType == ProgramType.AfterEffects)
                return RegistryHelper.GetHKEYLocalValue(_afterEffectsPluginsDir + program.Version.Major + ".0", "CommonPluginInstallPath");
            else
                return RegistryHelper.GetHKEYLocalValue(_vegasPluginsDir + program.Version.Major + ".0", "InstallPath");
        }
    }
}