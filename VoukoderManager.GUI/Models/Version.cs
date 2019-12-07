using System;

namespace VoukoderManager.GUI.Models
{
    public class Version : IVersion
    {
        public string PackageVersion { get; set; }

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
                    return 0;
                }
            }
        }

        public bool PreRelease { get; set; }

        public Version(string version)
        {
            PackageVersion = version;
        }

        public Version(string version, bool preRelease)
        {
            PackageVersion = version;
            PreRelease = preRelease;
        }
    }
}