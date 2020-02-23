using Serilog;
using System;

namespace VoukoderManager.Core.Models
{
    public class Version : IVersion
    {
        public string PackageVersion { get; }
        private bool _preRelease;

        public int Major
        {
            get
            {
                try
                {
                    var s = PackageVersion.Split('.');
                    return Convert.ToInt32(s[0]);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed parsing major version", this);
                    return 0;
                }
            }
        }

        public int Minor
        {
            get
            {
                try
                {
                    var s = PackageVersion.Split('.');
                    return Convert.ToInt32(s[1]);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed parsing minor version", this);
                    return 0;
                }
            }
        }

        public int Patch
        {
            get
            {
                try
                {
                    var s = PackageVersion.Split('.');
                    return Convert.ToInt32(s[2]);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed parsing patch version", this);
                    return 0;
                }
            }
        }

        public bool PreRelease
        {
            get
            {
                return _preRelease;
            }
        }

        public Version(string version)
        {
            PackageVersion = version;
        }

        public Version(string version, bool preRelease)
        {
            PackageVersion = version;
            _preRelease = preRelease;
        }

        public int CompareTo(IVersion obj)
        {
            return PackageVersion.CompareTo(obj.PackageVersion);
        }
    }
}