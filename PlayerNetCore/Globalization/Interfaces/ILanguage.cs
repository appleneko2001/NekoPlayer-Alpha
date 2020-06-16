using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Globalization.Interfaces
{
    public interface ILanguage
    {
        public bool ContainNode(string nodeKey);
        public void Load();
        public string Geti18nCode();
        public string GetName();
        public string GetCreatorName();
        public string GetNode(string node);
        public bool IsReady();
        public void Unload();
    }
}
