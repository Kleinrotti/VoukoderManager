using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace VoukoderManager.Language
{
    public class Lang
    {
        /// <summary>
        /// Stores the current selected language
        /// </summary>
        public static LangID LanguageID { get; set; }

        private static byte[] _file;
        private static string _translation;
        private static JObject _json;

        /// <summary>
        /// Triggers when the user selects another language
        /// </summary>
        public static event EventHandler LanguageChanged;

        public Lang()
        {
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

        private void LanguageChange(object sender, EventArgs e)
        {
            LanguageChanged?.Invoke(sender, e);
        }

        /// <summary>
        /// Change the language
        /// </summary>
        /// <param name="id"></param>
        public void ChangeLanguage(LangID id)
        {
            SetLanguage(id);
            Initialize();
            LanguageChange(this, EventArgs.Empty);
        }

        private void SetDefaults()
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey("Software", true))
            {
                _registryKey.CreateSubKey("VoukoderManager");
                _registryKey.SetValue("Language", LangID.English, RegistryValueKind.DWord);
            }
        }

        private void SetLanguage(LangID language)
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey("Software\\VoukoderManager", true))
            {
                _registryKey.SetValue("Language", language, RegistryValueKind.DWord);
            }
        }

        private LangID GetLanguage()
        {
            using (var _registryKey = Registry.CurrentUser.OpenSubKey("Software\\VoukoderManager", true))
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
    }
}