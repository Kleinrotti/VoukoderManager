using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using VoukoderManager.Core.Models;

namespace VoukoderManager.GUI
{
    internal static class SelfUpdate
    {
        public static void Update(IDownloadEntry entry)
        {
            var pkg = entry.StartPackageDownload().Result;

            string currentDir = Assembly.GetEntryAssembly().Location;
            var process = new Process();
            var inf = new ProcessStartInfo(pkg.Path);
            process.StartInfo = inf;
            process.Start();
            File.Copy(pkg.Path, currentDir);
            Environment.Exit(0);
        }
    }
}