using NekoPlayer.Globalization.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NekoPlayer.Globalization
{
    public class ExternalLangPack : ILanguage
    {
        private string code;
        private string displayName;
        private string creator;
        private string path;
        private Dictionary<string, string> table;
        public ExternalLangPack (string path)
        {
            string data = File.ReadAllText(path);
            var deserializedObject = JsonConvert.DeserializeObject<LanguagePackDataModel>(data);
            code = deserializedObject.i18nName;
            displayName = deserializedObject.name;
            creator = deserializedObject.creator;
            this.path = path;
        }
        public bool ContainNode(string nodeKey)
        {
            if (IsReady())
            {
                return table.ContainsKey(nodeKey.ToLowerInvariant());
            }
            return false;
        }

        public string GetCreatorName() => creator;

        public string Geti18nCode() => code;

        public string GetName() => displayName;

        public string GetNode(string node)
        {
            if (IsReady())
            {
                return table[node.ToLowerInvariant()];
            }
            return "";
        }

        public bool IsReady() => table != null;

        public void Load()
        {
            if (File.Exists(path))
            {
                string data = File.ReadAllText(path);
                var deserializedObject = JsonConvert.DeserializeObject<LanguagePackDataModel>(data);
                table = new Dictionary<string, string>(deserializedObject.table);
            }
        }

        public void Unload()
        {
            table.Clear();
            table = null;
        }
    }
}
