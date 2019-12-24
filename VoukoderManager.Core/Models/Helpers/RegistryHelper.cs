using Microsoft.Win32;
using System;
using System.Collections.Generic;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    public static class RegistryHelper
    {
        private static List<RegistryView> _views;

        public static string GetValue(string registryPath, string name)
        {
            var value = GetSystemTypeKey().OpenSubKey("Software\\Adobe\\Premiere Pro\\CurrentVersion").GetValue("Plug-InsDir").ToString();
            return value;
        }

        static RegistryHelper()
        {
            _views = new List<RegistryView>() { RegistryView.Registry32, RegistryView.Registry64 };
        }

        /// <summary>
        /// Returns all values in that registy path with the given name
        /// </summary>
        /// <param name="registryPath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<ProgramEntry> GetPrograms(string registryPath)
        {
            List<ProgramEntry> values = new List<ProgramEntry>();
            ProgramEntry entry = new ProgramEntry();
            foreach (RegistryView v in _views)
            {
                using (RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, v).OpenSubKey(registryPath))
                {
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            try
                            {
                                entry = new ProgramEntry()
                                {
                                    Name = sk.GetValue("DisplayName")?.ToString() ?? "",
                                    InstallationPath = sk.GetValue("InstallLocation")?.ToString() ?? "",
                                    Version = new Models.Version(sk.GetValue("DisplayVersion")?.ToString() ?? ""),
                                    UninstallString = sk.GetValue("UninstallString")?.ToString() ?? "",
                                    Publisher = sk.GetValue("Publisher")?.ToString() ?? "",
                                    InstallationDate = sk.GetValue("InstallDate")?.ToString() ?? "",
                                    ModifyPath = sk.GetValue("ModifyPath")?.ToString() ?? "",
                                    WindowsInstaller = Convert.ToBoolean(sk.GetValue("WindowsInstaller") ?? false)
                                };
                                if (!values.Exists(x => x.Name.Contains(entry.Name)))
                                {
                                    values.Add(entry);
                                }
                            }
                            catch (NullReferenceException ex)
                            { }
                        }
                    }
                }
            }
            return values;
        }

        /// <summary>
        /// Returns the registry base key for 32bit or 64bit
        /// </summary>
        /// <returns></returns>
        private static RegistryKey GetSystemTypeKey()
        {
            if (Environment.Is64BitOperatingSystem)
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
        }
    }
}