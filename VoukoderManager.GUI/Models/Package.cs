using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace VoukoderManager.GUI.Models
{
    public class Package : IPackage
    {
        public string Name { get; set; }

        public bool Certified
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public string Path { get; set; }

        public PackageType Type
        {
            get
            {
                var info = new FileInfo(Path).Extension;
                if (info == ".msi")
                    return PackageType.MSI;
                else if (info == ".exe")
                    return PackageType.EXE;
                else
                    return PackageType.NONE;
            }
        }

        public IVersion Version
        {
            get
            {
                using (Database db = new Database(Path))
                {
                    return new Version(db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", "ProductVersion") as string);
                }
            }
        }

        public Package(string packageName, string path)
        {
            Path = path;
            Name = packageName;
        }
    }
}