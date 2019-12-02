using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Text;

namespace VoukoderManager.Language
{
    public class Lang : IDisposable
    {
        /// <summary>
        /// Stores the current selected language
        /// </summary>
        public static LangID LanguageID { get; private set; }

        private static byte[] _file;
        private static string _translation;
        private static JObject _json;
        private BackgroundWorker _worker;
        private const string VoukoderManagerRegPath = "Software\\VoukoderManager";

        /// <summary>
        /// Triggers when the user selects another language
        /// </summary>
        public static event EventHandler<LanguageChangeEventArgs> LanguageChanged;

        public Lang()
        {
            _worker = new BackgroundWorker();
            Initialize();
        }

        private void Initialize()
        {
            var id = GetLanguage();
            switch (id)
            {
                case LangID.English:
                    _file = Resources.en;
                    break;

                case LangID.German:
                    _file = Resources.de;
                    break;
            }
            LanguageID = id;
            _translation = Encoding.Default.GetString(_file);
            _json = JObject.Parse(_translation);
        }

        private void LanguageChange(object sender, LanguageChangeEventArgs e)
        {
            LanguageChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Change the language
        /// </summary>
        /// <param name="id"></param>
        public void ChangeLanguage(LangID id)
        {
            if (_worker.IsBusy)
                return;
            LanguageID = id;
            _worker.DoWork += SetLanguage;
            _worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => { LanguageChange(this, new LanguageChangeEventArgs(id)); };
            _worker.RunWorkerAsync();
        }

        private void SetDefaults()
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                _registryKey.CreateSubKey("VoukoderManager");
                _registryKey.SetValue("Language", LangID.English, RegistryValueKind.DWord);
            }
        }

        private void SetLanguage(object sender, DoWorkEventArgs e)
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(VoukoderManagerRegPath, true))
            {
                _registryKey.SetValue("Language", LanguageID, RegistryValueKind.DWord);
            }
            Initialize();
        }

        private LangID GetLanguage()
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey(VoukoderManagerRegPath, true))
            {
                if (_registryKey == null)
                {
                    SetDefaults();
                }
                var id = (LangID)_registryKey.GetValue("Language");
                return id;
            }
        }

        /// <summary>
        /// Get a translated text
        /// </summary>
        /// <param name="translationstring"></param>
        /// <returns></returns>
        public static string GetText(string translationstring)
        {
            return (string)_json.SelectToken("translations." + translationstring);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _worker.Dispose();
                    _file = null;
                    _translation = null;
                    _json = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
                GC.SuppressFinalize(this);
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Lang()
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
        #endregion
    }
}