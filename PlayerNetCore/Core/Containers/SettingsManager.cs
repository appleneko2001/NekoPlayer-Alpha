using NekoPlayer.Core.Utilities;
using NekoPlayer.Containers;
using System;
using System.IO;
using Appleneko2001;

namespace NekoPlayer
{
    public static class SettingsManager
    {
        private static ISettingsContainer SettingsContainer;
        private static IDataContainer DataContainer;
        public static string OldUserAppdataPath { get; private set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NekoPlayer");
        public static string UserAppdataPath { get; private set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NekoPlayer");
        public static string UserCachesPath { get; private set; } = "Caches";
        public static string UserPlaylistsPath { get; private set; } = "Playlists";
        public static string UserLyricsPath { get; private set; } = "Lyrics";
        public static string UserRecentFilePath { get; private set; } = "recent.nplist";
        public static string UserRecentBackupFilePath { get; private set; } = "recent.nplist.backup";

        public static void Initialize(ISettingsContainer settingsContainer, IDataContainer dataContainer)
        {
            if (Directory.Exists(OldUserAppdataPath))
            {
                try
                {
                    Directory.Move(OldUserAppdataPath, UserAppdataPath);
                }
                catch (IOException)
                {
                    if (Directory.Exists(UserAppdataPath))
                    {
                        try
                        {
                            Utils.CopyFolder(OldUserAppdataPath, UserAppdataPath);
                            Directory.Delete(OldUserAppdataPath,true);
                        }
                        catch(Exception e)
                        {
                            ExceptMessage.PopupExcept(new AggregateException("An exception occurred when moving old application data to the new destination.", e), false, $"Try move folder yourself if you can\r\n OldDir: {OldUserAppdataPath}\r\n NewDir: {UserAppdataPath}");
                        }
                    }
                }
                catch(Exception e)
                {
                    ExceptMessage.PopupExcept( new AggregateException("An exception occurred when moving old application data to the new destination.",e),false, $"Try move folder yourself if you can\r\n OldDir: {OldUserAppdataPath}\r\n NewDir: {UserAppdataPath}");
                }
            }
            if (settingsContainer is null)
                throw new ArgumentNullException(nameof(settingsContainer), "parameter settingsContainer cannot be null.");
            if (dataContainer is null)
                throw new ArgumentNullException(nameof(dataContainer), "parameter dataContainer cannot be null.");
            SettingsContainer = settingsContainer;
            DataContainer = dataContainer;
        }
        public static T GetValue<T>(string key, object defaultValue = null)
        {
            T result = default(T);
            if (SettingsContainer != null)
                result = SettingsContainer.GetValue<T>(key, defaultValue);

            if (result == null)
                return (T)defaultValue;
            else
                return result.ToString().Length == 0 ? (T)defaultValue : result;
        }
        public static void SetValue(string key, object value)
        {
            SettingsContainer?.SetValue(key, value);
        }
        public static void SaveContext(string path, object data)
        {
            if (!DataContainer.TrySave(path, data))
            {
                ExceptMessage.PopupExcept($"Cannot save context to {path}.");
            }
        }
        public static T LoadContext<T>(string path)
        {
            T data;
            if (DataContainer.TryLoad(path, out data))
            {
                return data;
            }
            else
                return default;
        }
        public static bool DeleteContext(string path)
        {
            return DataContainer.TryDelete(path);
        }
        public static string[] GetContexts(string path, string pattern = "*")
        {
            return DataContainer.GetFiles(path, pattern);
        }
        public static void Stop()
        {
            SettingsContainer.StopService();
            DataContainer.StopService();
        }
    }
}
