using Octokit;
using System;
using System.Collections.Generic;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    public class PackageManager
    {
        public List<IPackage> Query { get; private set; }

        /// <summary>
        /// Retuns a list of downloadable components
        /// </summary>
        /// <param name="type"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public List<IVoukoderEntry> GetDownloadablePackages(ProgramType type, int results)
        {
            var lst = new List<IVoukoderEntry>();
            string repo;
            string repopath;
            var client = new GitHubClient(new ProductHeaderValue("voukodermanager"));

            if (type == ProgramType.VoukoderCore)
            {
                repo = "voukoder";
                return GetReleases(client, "Vouk", repo, results);
            }
            else
            {
                repo = "voukoder-connectors";
                if (type == ProgramType.VoukoderConnectorVegas)
                {
                    repopath = "vegas";
                }
                else if (type == ProgramType.VoukoderConnectorAfterEffects)
                {
                    repopath = "aftereffects";
                }
                else
                {
                    repopath = "premiere";
                }
                return GetContent(type, client, "Vouk", repo, repopath, results);
            }
        }

        private List<IVoukoderEntry> GetContent(ProgramType type, GitHubClient client, string owner, string repo, string filepath, int results)
        {
            var test = client.Repository.Content.GetAllContents(owner, repo, filepath).Result;
            var lst = new List<IVoukoderEntry>();
            int i = 0;
            foreach (var v in test)
            {
                if (v.Name.Contains("connector"))
                {
                    if (i >= results)
                        break;
                    var version = v.Name.Split('.');
                    lst.Add(new VoukoderEntry
                    {
                        Name = v.Name,
                        DownloadUrl = new Uri(v.DownloadUrl),
                        Version = new Models.Version(version[version.Length - 1]),
                        Type = type,
                        Dependencies = new List<IVoukoderEntry>() { GetLatestDownloadablePackage(ProgramType.VoukoderCore) }
                    });
                    i++;
                }
            }
            return lst;
        }

        private List<IVoukoderEntry> GetReleases(GitHubClient client, string owner, string repo, int results)
        {
            var lst = new List<IVoukoderEntry>();
            var releases = client.Repository.Release.GetAll(owner, repo).Result;
            int i = 0;
            foreach (var f in releases)
            {
                if (i >= results)
                    break;
                lst.Add(new VoukoderEntry
                {
                    Name = f.Name,
                    Version = new Models.Version(f.Name, f.Prerelease),
                    DownloadUrl = new Uri(f.Assets[0].BrowserDownloadUrl)
                });
                i++;
            }
            return lst;
        }

        public IVoukoderEntry GetLatestDownloadablePackage(ProgramType type)
        {
            string repo;
            var client = new GitHubClient(new ProductHeaderValue("voukodermanager"));

            if (type == ProgramType.VoukoderCore)
            {
                repo = "voukoder";
                var release = client.Repository.Release.GetLatest("Vouk", repo).Result;
                var entry = new VoukoderEntry()
                {
                    Name = release.Name,
                    Version = new Models.Version(release.Name, release.Prerelease),
                    DownloadUrl = new Uri(release.Assets[0].BrowserDownloadUrl)
                };
                return entry;
            }
            else
                return null;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Query = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~PackageManager()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}