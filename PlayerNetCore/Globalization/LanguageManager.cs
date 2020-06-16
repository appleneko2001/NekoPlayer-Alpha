using NekoPlayer.Core.Utilities;
using NekoPlayer.Globalization.Interfaces;
using NekoPlayer.Wpf.ConverterKinds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NekoPlayer.Globalization
{
    public class LanguageManager : ViewModelBase
    {
        const string RegistryKey = "Language";
        const string FolderName = "Globalization";
        static LanguageManager Instance;
        Dictionary<string, ILanguage> registredLanguagePacks = new Dictionary<string, ILanguage>();
        public ILanguage SelectedPack { get; private set; }
        public ILanguage FallbackPack { get; private set; }
        public const string FallbackDefaultLanguage = "en-US";
        public static void Initialize()
        {
            bool inEditor = Appleneko2001.Utils.IsXamlDesignerMode();
            Instance = new LanguageManager();
            if (!inEditor)
            {
                LoadLanguagePacks();
                LoadSettings();
            }
            else
            {
                if (Instance.FallbackPack == null)
                    Instance.FallbackPack = new FallbackLanguage();
                Instance.registredLanguagePacks.Add(Instance.FallbackPack.Geti18nCode(), Instance.FallbackPack);
                SelectLanguage(FallbackDefaultLanguage);
            }
        }
        private static void LoadLanguagePacks()
        {
            if (Instance.registredLanguagePacks.Count > 0)
                return;
            if (Instance.FallbackPack == null)
                Instance.FallbackPack = new FallbackLanguage();
            List<ILanguage> lists = new List<ILanguage>();
            lists.Add(Instance.FallbackPack);
            string path = Path.Combine(Environment.CurrentDirectory, FolderName);
            if (Directory.Exists(path))
            {
                var files = Directory.GetFiles(path, "*.json");
                foreach(var item in files)
                {
                    try
                    {
                        lists.Add(new ExternalLangPack(item));
                    }
                    catch(Exception e)
                    {
                        ExceptMessage.PopupExcept(e, false);
                    }
                }
            }
            foreach(var l in lists)
            {
                Instance.registredLanguagePacks.Add(l.Geti18nCode(), l);
            }
        }
        public static string Combine(params object[] textOrNode)
        {
            string r = "";
            foreach(object str in textOrNode)
            {
                string s = str.ToString();
                if (s.StartsWith('#'))
                {
                    if (s.StartsWith('!'))
                    {
                        r += s.Substring(2);
                    }
                    else
                    {
                        r += RequestNode(s.Substring(1));
                    }
                }
                else
                    r += s;
            }
            return r;
        }
        public static string RequestNode(string node)
        {
            if (Instance == null)
            {
                Initialize();
                SelectLanguage(FallbackDefaultLanguage);
            }
            if (Instance.SelectedPack != null)
            {
                if (Instance.SelectedPack.ContainNode(node))
                    return Instance.SelectedPack.GetNode(node);
                else if (Instance.FallbackPack.ContainNode(node))
                    return Instance.FallbackPack.GetNode(node);
            }
            else if (Instance.FallbackPack.ContainNode(node))
                return Instance.FallbackPack.GetNode(node);

            return node;
        }
        /// <summary>
        /// Change selected language pack. You have to restart program to completely applied changes.
        /// </summary>
        /// <param name="language"></param>
        public static bool SelectLanguage(string language)
        {
            bool changes = false;
            var inst = Instance;
            try
            {
                if (!inst?.registredLanguagePacks.ContainsKey(language) ?? false)
                {
                    language = FallbackDefaultLanguage;
                }
                var previousPack = inst.SelectedPack;
                var pack = inst.registredLanguagePacks[language];
                if (!pack.IsReady())
                {
                    pack.Load();
                    if (!pack.IsReady())
                        throw new TypeInitializationException(pack.GetType().FullName, null);
                }
                if (previousPack != pack)
                {
                    previousPack?.Unload();
                    previousPack = null;
                    changes = true;
                }
                inst.SelectedPack = pack;
                GC.Collect();
                inst.OnPropertyChanged();
                Variants.LoadStringVariants();
                return changes;
            }
            catch (Exception e)
            {
                ExceptMessage.PopupExcept(e, false);
                throw;
            }
        }
        public static void LoadSettings()
        {
            try
            {
                var setting = SettingsManager.GetValue<string>(RegistryKey, FallbackDefaultLanguage);
                if (setting == null)
                {
                    SelectLanguage(FallbackDefaultLanguage);
                    SaveSettings();
                }
                else if (Instance.registredLanguagePacks.ContainsKey(setting))
                {
                    SelectLanguage(setting);
                }
            }
            catch
            {
                SelectLanguage(FallbackDefaultLanguage);
            }
        }
        public static void SaveSettings()
        {
            SettingsManager.SetValue(RegistryKey, Instance.SelectedPack.Geti18nCode());
        }
        public static Dictionary<string, string> GetLanguagesPackList()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach(var item in Instance.registredLanguagePacks)
            {
                dict.Add(item.Key, item.Value.GetName());
            }
            return dict;
        }

    }
}
