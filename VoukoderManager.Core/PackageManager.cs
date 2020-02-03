using Octokit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using VoukoderManager.Core.Models;

namespace VoukoderManager.Core
{
    public class PackageManager
    {
        private static readonly GitHubClient _client;
        public static bool AllowPreReleaseVersion { get; set; } = false;

        public static int RemainingApiRequests { get { return _client.GetLastApiInfo().RateLimit.Remaining; } }

        public static event EventHandler<ApiRequestEventArgs> ApiRequestUsed;

        private void OnRequest(object sender, ApiRequestEventArgs e)
        {
            ApiRequestUsed?.Invoke(sender, e);
        }

        static PackageManager()
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
                var content = GetContent(type, "Vouk", repo, repopath, results, true);
                Mouse.OverrideCursor = null;
                return content;
            }
        }

        private List<IGitHubEntry> GetContent(ProgramType type, string owner, string repo, string filepath, int results, bool includeCore)
        {
            try
            {
                IGitHubEntry corepkg = null;
                var test = _client.Repository.Content.GetAllContents(owner, repo, filepath).Result;
                if (includeCore)
                    corepkg = GetLatestDownloadableCorePackage(ProgramType.VoukoderCore);
                OnRequest(this, new ApiRequestEventArgs(_client.GetLastApiInfo()));
                var re = _client.GetLastApiInfo();
                var lst = new List<IGitHubEntry>();
                int entries = test.Count;
                while (results > 0)
                {
                    var v = test[entries - 1];
                    if (v.Name.Contains("connector"))
                    {
                        var version = v.Name.Where(Char.IsDigit).ToArray();
                        var vkentry = new VKGithubEntry(v.Name, new Models.Version(string.Join(".", version)))
                        {
                            DownloadUrl = new Uri(v.DownloadUrl),
                            ComponentType = type,
                            Dependencies = new List<IGitHubEntry>() { corepkg }
                        };
                        lst.Add(vkentry);
                    }
                    entries--;
                    results--;
                }
                return lst;
            }
            catch (AggregateException)
            {
                throw;
            }
        }

        private List<IGitHubEntry> GetReleases(GitHubClient client, string owner, string repo, int results)
        {
            var lst = new List<IGitHubEntry>();
            try
            {
                var releases = client.Repository.Release.GetAll(owner, repo).Result;
                OnRequest(this, new ApiRequestEventArgs(_client.GetLastApiInfo()));
                int i = 0;
                foreach (var f in releases)
                {
                    if (i >= results)
                        break;

                    var vkentry = new VKGithubEntry(f.Name, new Models.Version(f.Name));
                    vkentry.DownloadUrl = new Uri(f.Assets[0].BrowserDownloadUrl);
                    lst.Add(vkentry);
                    i++;
                }
                return lst;
            }
            catch (AggregateException)
            {
                throw;
            }
        }

        public IGitHubEntry GetLatestDownloadableCorePackage(ProgramType type)
        {
            string repo;
            try
            {
                if (type == ProgramType.VoukoderCore)
                {
                    repo = "voukoder";
                    var release = _client.Repository.Release.GetLatest("Vouk", repo).Result;
                    OnRequest(this, new ApiRequestEventArgs(_client.GetLastApiInfo()));
                    var entry = new VKGithubEntry(release.Name, new Models.Version(release.Name))
                    {
                        DownloadUrl = new Uri(release.Assets[0].BrowserDownloadUrl)
                    };
                    return entry;
                }
                else
                    return null;
            }
            catch (AggregateException)
            {
                throw;
            }
        }

        public IGitHubEntry GetUpdate(IProgramEntry entry)
        {
            string repo;
            if (entry == null)
                return null;
            try
            {
                if (entry.ComponentType == ProgramType.VoukoderCore)
                {
                    repo = "voukoder";
                    var release = _client.Repository.Release.GetLatest("Vouk", repo).Result;
                    OnRequest(this, new ApiRequestEventArgs(_client.GetLastApiInfo()));

                    var entr = new VKGithubEntry(release.Name, new Models.Version(release.TagName))
                    {
                        DownloadUrl = new Uri(release.Assets[0].BrowserDownloadUrl)
                    };
                    if (entry.Version.CompareTo(entr.Version) < 0)
                        return entr;
                    return null;
                }
                else
                {
                    repo = "voukoder-connectors";
                    string repopath;
                    if (entry.ComponentType == ProgramType.VoukoderConnectorVegas)
                    {
                        repopath = "vegas";
                    }
                    else if (entry.ComponentType == ProgramType.VoukoderConnectorAfterEffects)
                    {
                        repopath = "aftereffects";
                    }
                    else
                    {
                        repopath = "premiere";
                    }
                    var content = GetContent(entry.ComponentType, "Vouk", repo, repopath, 1, false);
                    if (entry.Version.CompareTo(content[0].Version) < 0)
                        return content[0];
                    else
                        return null;
                }
            }
            catch (AggregateException ex)
            {
                var v = _client.GetLastApiInfo();
                Trace.WriteLine("API Limit resets at: " + v.RateLimit.Reset.ToLocalTime());
                return null;
                //throw;
            }
        }

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