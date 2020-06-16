using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Containers
{
    /// <summary>
    /// A interface for providing access get or set value to container and long-time data storaging
    /// </summary>
    public interface ISettingsContainer
    {
        public T GetValue<T>(string key, object defaultValue);
        public void SetValue(string key, object value);
        public void DeleteValue(string key);
        public void StopService();
    }
}
