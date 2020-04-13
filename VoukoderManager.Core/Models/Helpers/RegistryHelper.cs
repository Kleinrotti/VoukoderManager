using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VoukoderManager.Core
{
    public static class RegistryHelper
    {
        private static readonly List<RegistryView> _views;
        private static readonly List<string> _registryProgramPaths;

        /// <summary>
        /// Get a value from registry for the given key path and value name
        /// </summary>
        /// <param name="registryPath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetHKEYLocalValue(string registryPath, string name)
        {
            var value = GetSystemTypeKey.OpenSubKey(registryPath).GetValue(name).ToString();
            return value;
        }

        static RegistryHelper()
        {
            _views = new List<RegistryView>() { RegistryView.Registry32, RegistryView.Registry64 };
            _registryProgramPaths = new List<string>{ @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
            @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" };
        }

        private static List<RegistryEntry> _values;
        private static DateTime _lastRefreshed;

        /// <summary>
        /// Returns all values in that registy path with the given name
        /// </summary>
        /// <param name="registryPath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<RegistryEntry> GetPrograms()
        {
            var curr = DateTime.Now;
            var div = curr - _lastRefreshed;
            if (_values == null || div.TotalSeconds > 1)
            {
                Log.Debug("Getting programs from registry...");
                Stopwatch w = new Stopwatch();
                w.Start();
                _values = new List<RegistryEntry>();
                var currentList = new List<RegistryEntry>();
                RegistryEntry entry = new RegistryEntry();
                foreach (var paths in _registryProgramPaths)
                {
                    foreach (RegistryView v in _views)
                    {
                        using (RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, v).OpenSubKey(paths))
                        {
                            foreach (string skName in rk.GetSubKeyNames())
                            {
                                using (RegistryKey sk = rk.OpenSubKey(skName))
                                {
                                    Log.Verbose("Processing program registry entry", sk);
                                    try
                                    {
                                        entry = new RegistryEntry();
                                        entry.DisplayName = sk.GetValue("DisplayName")?.ToString() ?? "";
                                        entry.DisplayVersion = sk.GetValue("DisplayVersion")?.ToString() ?? "";
                                        entry.PreRelease = entry.DisplayName.Contains("rc") || entry.DisplayName.Contains("beta");
                                        entry.InstallationPath = sk.GetValue("InstallLocation")?.ToString() ?? "";
                                        entry.UninstallString = sk.GetValue("UninstallString")?.ToString() ?? "";
                                        entry.Publisher = sk.GetValue("Publisher")?.ToString() ?? "";
                                        entry.InstallationDate = sk.GetValue("InstallDate")?.ToString() ?? "";
                                        entry.ModifyPath = sk.GetValue("ModifyPath")?.ToString() ?? "";
                                        entry.WindowsInstaller = Convert.ToBoolean(sk.GetValue("WindowsInstaller") ?? false);

                                        if (!currentList.Exists(x => x.DisplayName.Contains(entry.DisplayName)))
                                        {
                                            currentList.Add(entry);
                                        }
                                    }
                                    catch (NullReferenceException ex)
                                    {
                                        Log.Error(ex, $"Error formatting Registry values to RegistryEntry", entry.DisplayName);
                                    }
                                }
                            }
                        }
                    }
                }
                w.Stop();
                Log.Debug($"Finished getting programs from registry in {w.ElapsedMilliseconds} ms");
                _values = currentList;
            }
            _lastRefreshed = DateTime.Now;
            return _values;
        }

        public static void SetValue(string valueName, bool value)
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(@"Software\VoukoderManager", true))
            {
                _registryKey.SetValue(valueName, value, RegistryValueKind.DWord);
                Log.Debug($"Set registry value {valueName}: {value}");
            }
        }

        public static bool GetValue(string valueName)
        {
            using (var _registryKey = Registry.CurrentUser.CreateSubKey(@"Software\VoukoderManager", true))
            {
                Log.Debug($"Getting {valueName} from registry");
                var v = _registryKey.GetValue(valueName);
                if (v == null)
                {
                    _registryKey.SetValue(valueName, false, RegistryValueKind.DWord);
                    Log.Debug($"Set registry value {valueName}: {false}");
                    return false;
                }
                return Convert.ToBoolean(v);
            }
        }

        /// <summary>
        /// Returns the registry base key for 32bit or 64bit
        /// </summary>
        /// <returns></returns>
        private static RegistryKey GetSystemTypeKey
        {
            get
            {
                if (Environment.Is64BitOperatingSystem)
                    return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                else
                    return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }
        }
    }
}