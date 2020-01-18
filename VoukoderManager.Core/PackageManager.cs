using Octokit;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    public class PackageManager
    {
        private GitHubClient _client;

        public PackageManager()
        {
            _client = new GitHubClient(new ProductHeaderValue("voukodermanager"));
        }

        /// <summary>
        /// Retuns a list of downloadable components
        /// </summary>
        /// <param name="type"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public List<IGitHubEntry> GetDownloadablePackages(ProgramType type, int results)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            var lst = new List<IGitHubEntry>();
            string repo;
            string repopath;

            if (type == ProgramType.VoukoderCore)
            {
                repo = "voukoder";
                var re = GetReleases(_client, "Vouk", repo, results);
                Mouse.OverrideCursor = null;
                return re;
            }
            else
            {
                repo = "voukoder-connectors";
                if (type == ProgramType.VEGAS)
                {
                    repopath = "vegas";
                }
                else if (type == ProgramType.AfterEffects)
                {
                    repopath = "aftereffects";
                }
                else
                {
                    repopath = "premiere";
                }
                var content = GetContent(type, _client, "Vouk", repo, repopath, results);
                Mouse.OverrideCursor = null;
                return content;
            }
        }

        private List<IGitHubEntry> GetContent(ProgramType type, GitHubClient client, string owner, string repo, string filepath, int results)
        {
            try
            {
                var test = client.Repository.Content.GetAllContents(owner, repo, filepath).Result;
                var lst = new List<IGitHubEntry>();
                int entries = test.Count;

                while (results > 0)
                {
                    var v = test[entries - 1];
                    if (v.Name.Contains("connector"))
                    {
                        var version = v.Name.Split('.');
                        lst.Add(new VoukoderEntry
                        {
                            Name = v.Name,
                            DownloadUrl = new Uri(v.DownloadUrl),
                            Version = new Models.Version(version[version.Length - 1]),
                            ComponentType = type,
                            Dependencies = new List<IGitHubEntry>() { GetLatestDownloadablePackage(ProgramType.VoukoderCore) }
                        });
                    }
                    entries--;
                    results--;
                }
                return lst;
            }
            catch (AggregateException)
            {
                return null;
            }
        }

        private List<IGitHubEntry> GetReleases(GitHubClient client, string owner, string repo, int results)
        {
            var lst = new List<IGitHubEntry>();
            try
            {
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
            catch (AggregateException)
            {
                return null;
            }
        }

        public IGitHubEntry GetLatestDownloadablePackage(ProgramType type)
        {
            string repo;
            try
            {
                if (type == ProgramType.VoukoderCore)
                {
                    repo = "voukoder";
                    var release = _client.Repository.Release.GetLatest("Vouk", repo).Result;
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
            catch (AggregateException)
            {
                return null;
            }
        }

        //public IVoukoderEntry GetUpdate(IProgramEntry entry)
        //{
        //    string repo;
        //    var client = new GitHubClient(new ProductHeaderValue("voukodermanager"));

        //    if (entry.Type == ProgramType.VoukoderCore)
        //    {
        //        repo = "voukoder";
        //        var release = client.Repository.Release.GetLatest("Vouk", repo).Result;
        //        if (entry.Version == release.TagName)
        //        {
        //            var entr = new VoukoderEntry()
        //            {
        //                Name = release.Name,
        //                Version = new Models.Version(release.TagName, release.Prerelease),
        //                DownloadUrl = new Uri(release.Assets[0].BrowserDownloadUrl)
        //            };
        //        }
        //        return entr;
        //    }
        //    else
        //    {
        //    }
        //}

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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