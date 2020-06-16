using System;
using System.Collections.Generic;
using System.Text;

namespace NekoPlayer.Containers
{
    public interface IDataContainer
    {
        public void StopService();
        public string[] GetFiles(string path, string pattern = "*", string excepts = null);
        public bool TrySave(string path, object data);
        public bool TryLoad(string path, out object data, object emptyData = default);
        public bool TryLoad<T>(string path, out T data, T emptyData = default);
        public bool TryDelete(string path);
    }
}
