using Microsoft.Win32;
using NekoPlayer.Core.Utilities;
using System;

namespace NekoPlayer.Containers
{
    /// <summary>
    /// RegistryManager providing settings container with fastest speed, works on Windows platform only.
    /// </summary>
    public class RegistryManager : ISettingsContainer
    {
        private RegistryKey Root;
        public RegistryManager()
        {
            Registry.CurrentUser.CreateSubKey("Software\\NekoPlayer", RegistryKeyPermissionCheck.Default);
            Root = Registry.CurrentUser.OpenSubKey("Software\\NekoPlayer", true);
        }
        public T GetValue<T>(string key, object defaultValue)
        {
            try {
                if (typeof(T).FullName != typeof(bool).FullName) 
                {
                    var result = (T)Root.GetValue(key, (T)defaultValue);
                    return result;
                }
                else
                {
                    string value = (string)Root.GetValue(key, (defaultValue ?? false).ToString());
                    var result = (object)bool.Parse(value);
                    return (T)result;
                }
            }
            catch(Exception e)
            {
                ExceptMessage.PrintConsole(e);
                return (T)defaultValue ?? default;
            }
        }
        public void SetValue(string key, object value)
        {
            Root.SetValue(key, value ?? "");
        }
        public void DeleteValue(string key)
        {
            Root.DeleteValue(key);
        }
        public void StopService()
        {
            Root.Dispose();
        }
    }
}
