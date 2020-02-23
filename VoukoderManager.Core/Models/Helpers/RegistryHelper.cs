﻿using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;

namespace VoukoderManager.Core
{
    public static class RegistryHelper
    {
        private static List<RegistryView> _views;

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
        }

        /// <summary>
        /// Returns all values in that registy path with the given name
        /// </summary>
        /// <param name="registryPath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<RegistryEntry> GetPrograms(string registryPath)
        {
            List<RegistryEntry> values = new List<RegistryEntry>();
            RegistryEntry entry = new RegistryEntry();
            foreach (RegistryView v in _views)
            {
                using (RegistryKey rk = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, v).OpenSubKey(registryPath))
                {
                    foreach (string skName in rk.GetSubKeyNames())
                    {
                        using (RegistryKey sk = rk.OpenSubKey(skName))
                        {
                            Log.Verbose<RegistryKey>("Processing program registry entry", sk);
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

                                if (!values.Exists(x => x.DisplayName.Contains(entry.DisplayName)))
                                {
                                    values.Add(entry);
                                }
                            }
                            catch (NullReferenceException ex)
                            {
                                Log.Error(ex, $"Error formatting Registry values to RegistryEntry", entry);
                            }
                        }
                    }
                }
            }
            return values;
        }

        public static void SetUseBetaVersion(bool value)
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(@"Software\VoukoderManager", true))
            {
                Log.Debug($"Setting registry value UseBetaVersions: {value}");
                _registryKey.SetValue("UseBetaVersions", value, RegistryValueKind.DWord);
            }
        }

        public static bool GetUseBetaVersion()
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(@"Software\VoukoderManager", true))
            {
                Log.Debug("Getting UseBetaVersion from registry");
                var v = _registryKey.GetValue("UseBetaVersions");
                if (v == null)
                {
                    _registryKey.SetValue("UseBetaVersions", false, RegistryValueKind.DWord);
                    return false;
                }
                return Convert.ToBoolean(v);
            }
        }

        public static void SetLogging(bool value)
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(@"Software\VoukoderManager", true))
            {
                Log.Debug($"Setting registry value logging: {value}");
                _registryKey.SetValue("Logging", value, RegistryValueKind.DWord);
            }
        }

        public static bool GetLogging()
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(@"Software\VoukoderManager", true))
            {
                Log.Debug("Getting logging from registry");
                var v = _registryKey.GetValue("Logging");
                if (v == null)
                {
                    _registryKey.SetValue("Logging", false, RegistryValueKind.DWord);
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