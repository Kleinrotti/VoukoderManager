using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace VoukoderManager.GUI
{
    public static class RegistryHelper
    {
        private static List<RegistryView> _views;

        public static string GetValue(string registryPath, string name)
        {
            var value = GetSystemTypeKey().OpenSubKey("Software\\Adobe\\Premiere Pro\\CurrentVersion").GetValue("Plug-InsDir").ToString();
            //localKey.Close();
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
        public static List<Entry> GetPrograms(string registryPath)
        {
            List<Entry> values = new List<Entry>();
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
                                var entry = new Entry(sk.GetValue("DisplayName").ToString(),
                                    sk.GetValue("InstallLocation").ToString(),
                                    sk.GetValue("DisplayVersion").ToString());
                                if (!values.Exists(x => x.ProgramName.Contains(entry.ProgramName)))
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

        private static RegistryKey GetSystemTypeKey()
        {
            if (Environment.Is64BitOperatingSystem)
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            else
                return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
        }
    }
}