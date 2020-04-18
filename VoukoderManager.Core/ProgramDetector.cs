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
        private static readonly string _premierePluginsDir = @"SOFTWARE\Adobe\Premiere Pro\";
        private static readonly string _afterEffectsPluginsDir = @"SOFTWARE\Adobe\After Effects\";
        private static readonly string _vegasPluginsDir = @"SOFTWARE\Sony Creative Software\VEGAS Pro\";
        private static readonly string _movieStudioPluginsDir = @"SOFTWARE\Sony Creative Software\Movie Studio\";
        public static readonly List<string> ProgramSearchValues = new List<string>() { "Premiere", "Media Encoder", "AfterEffects", "VEGAS" };
        public static readonly List<string> VoukoderSearchValues = new List<string>() { "Premiere", "AfterEffects", "VEGAS" };

        /// <summary>
        /// Returns a list which contains all installed programs where voukoder components are available for
        /// </summary>
        /// <param name="includeConnector">include the connector if installed</param>
        /// <param name="onlyNewestVersion">only the newest version of each program</param>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledPrograms(bool includeConnector, bool onlyNewestVersion)
        {
            var programs = RegistryHelper.GetPrograms();
            Log.Debug("Received registry program list for GetInstalledPrograms");
            List<IProgramEntry> list = new List<IProgramEntry>();
            IProgramEntry pro;
            foreach (RegistryEntry pr in programs)
            {
                var displayname = pr.DisplayName;
                foreach (var v in ProgramSearchValues)
                {
                    if (displayname.Contains(v) && !displayname.Contains("connector"))
                    {
                        var parsed = Enum.TryParse(v.Replace(" ", ""), out ProgramType result);
                        Log.Debug($"Enum parsing successed: {parsed}");
                        ConvertFromRegistryEntry(out pro, pr, result);
                        if (includeConnector)
                            FillComponent(ref pro);
                        list.Add(pro);
                        Log.Debug($"Adding: {pro.Name} to list");
                    }
                }
            }

            void FillComponent(ref IProgramEntry entry)
            {
                entry.SubComponent = GetVoukoderComponent(entry.ComponentType);
                if (entry.SubComponent != null)
                {
                    Log.Debug($"Adding subcomponent: {entry.SubComponent.Name} to entry");
                    entry.SubComponent.SubComponent = GetVoukoderComponent(ProgramType.VoukoderCore);
                }
            }
            if (onlyNewestVersion)
            {
                var enumlengh = Enum.GetNames(typeof(ProgramType)).Length;
                //Check for multiple installed versions of each program
                for (int i = 0; i < enumlengh; i++)
                {
                    var items = list.Where(x => (int)x.ComponentType == i);
                    //if more than one version of the same program is installed
                    if (items.Count() > 2)
                    {
                        Log.Debug("Removing older versions of same program from list");
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
            var programs = RegistryHelper.GetPrograms();
            Log.Debug("Received registry program list for GetVoukoderComponent");
            IProgramEntry entry = null;
            List<RegistryEntry> vComponents = null;
            foreach (RegistryEntry e in programs)
            {
                var displayname = e.DisplayName;
                if (displayname.Contains("Voukoder"))
                {
                    if (vComponents == null)
                        vComponents = new List<RegistryEntry>();
                    vComponents.Add(e);
                }
            }
            if (vComponents == null)
                return null;
            try
            {
                var ad = vComponents.Single(x => x.DisplayName.Any(char.IsDigit));
                ConvertFromRegistryEntry(out entry, ad, ProgramType.VoukoderCore);
            }
            catch (InvalidOperationException ex) { }
            try
            {
                var re = vComponents.Single(x => x.DisplayName.Contains(connectorType.ToString()));
                ConvertFromRegistryEntry(out entry, re, connectorType);
            }
            catch (InvalidOperationException ex) { }
            if (entry != null && connectorType == entry.ComponentType)
                return entry;

            Log.Debug("No match for searching voukoder component");
            return null;
        }

        /// <summary>
        /// Returns a list which contains all installed Voukoder components
        /// </summary>
        /// <returns></returns>
        public static List<IProgramEntry> GetInstalledVoukoderComponents()
        {
            var programs = RegistryHelper.GetPrograms();
            Log.Debug("Received registry program list for GetInstalledVoukoderComponents");
            List<IProgramEntry> list = new List<IProgramEntry>();
            IProgramEntry entry;
            foreach (RegistryEntry e in programs)
            {
                var displayname = e.DisplayName;
                if (displayname.Contains("Voukoder"))
                {
                    foreach (var v in VoukoderSearchValues)
                    {
                        if (displayname.Contains(v))
                        {
                            Enum.TryParse(v, out ProgramType result);
                            ConvertFromRegistryEntry(out entry, e, result);
                        }
                        else
                            ConvertFromRegistryEntry(out entry, e, ProgramType.VoukoderCore);
                        list.Add(entry);
                    }
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
            if ((program.ComponentType == ProgramType.Premiere || program.ComponentType == ProgramType.MediaEncoder) && program.Version.Major > 6)
                return RegistryHelper.GetHKEYLocalValue(_premierePluginsDir + program.Version.Major + ".0", "CommonPluginInstallPath");
            //If CS6 is installed we have to use a different registry path
            else if ((program.ComponentType == ProgramType.Premiere) && program.Version.Major <= 6)
                return RegistryHelper.GetHKEYLocalValue(_premierePluginsDir + "CurrentVersion", "Plug-InsDir");
            else if (program.ComponentType == ProgramType.AfterEffects)
                return RegistryHelper.GetHKEYLocalValue(_afterEffectsPluginsDir + program.Version.Major + ".0", "CommonPluginInstallPath");
            else if (program.ComponentType == ProgramType.MovieStudio)
                return RegistryHelper.GetHKEYLocalValue(_movieStudioPluginsDir + program.Version.Major + ".0", "InstallPath");
            else
                return RegistryHelper.GetHKEYLocalValue(_vegasPluginsDir + program.Version.Major + ".0", "InstallPath");
        }
    }
}