using System;

namespace VoukoderManager.Language
{
    public class LanguageChangeEventArgs : EventArgs
    {
        public LangID CurrentLanguage { get; set; }
        public LangID PreviousLanguage { get; set; }

        public LanguageChangeEventArgs(LangID currentLanguage)
        {
            CurrentLanguage = currentLanguage;
        }
    }
}
