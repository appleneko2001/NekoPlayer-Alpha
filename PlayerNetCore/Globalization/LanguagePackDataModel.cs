using NekoPlayer.Globalization.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Globalization
{
    public class LanguagePackDataModel
    {
        public LanguagePackDataModel()
        {
        }
        public LanguagePackDataModel(FallbackLanguage pack)
        {
            if (pack is null)
                throw new ArgumentNullException(nameof(pack));
            name = pack.GetName();
            i18nName = pack.Geti18nCode();
            creator = pack.GetCreatorName();
            table = pack.nodes;
        }

        public string name;
        public string i18nName;
        public string description;
        public string creator;
        public Dictionary<string, string> table;
    }
}
