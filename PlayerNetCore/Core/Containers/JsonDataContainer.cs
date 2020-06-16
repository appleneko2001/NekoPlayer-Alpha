using NekoPlayer.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NekoPlayer.Containers
{
    /// <summary>
    /// JsonDataContainer providing save data to hard drive with json serialization
    /// and supports some modern platform (Windows, Linux, Mac, Android). Required component Newtonsoft.Json.
    /// </summary>
    public class JsonDataContainer : IDataContainer
    {
        public JsonDataContainer(string root)
        {
            RootPath = root;
            CreateIfNotExistDir(RootPath);
        }

        private string RootPath;

        public bool TryLoad(string path, out object data, object emptyData = null)
        {
            data = emptyData;
            string finalPath = Path.Combine(RootPath, path);
            if (!File.Exists(finalPath))
                return false;
            try
            {
                string json = File.ReadAllText(finalPath);
                data = JsonConvert.DeserializeObject(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryLoad<T>(string path, out T data, T emptyData = default)
        {
            data = emptyData;
            string finalPath = Path.Combine(RootPath, path);
            if (!File.Exists(finalPath))
                return false;
            try
            {
                string json = File.ReadAllText(finalPath);
                data = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TrySave(string path, object data)
        {
            string finalPath = Path.Combine(RootPath, path);
            string parentPath = Path.GetDirectoryName(finalPath);
            DirectoryInfo parentDir = new DirectoryInfo(parentPath);
            string oldDirPath = Path.Combine(RootPath, "oldFiles", parentDir.Name);
            CreateIfNotExistDir(oldDirPath);
            CreateIfNotExistDir(parentPath);
            Task.Run(() =>
            {
                if (File.Exists(finalPath))
                {
                    string fileName = Path.GetFileName(finalPath);
                    string dirPath = Path.GetDirectoryName(finalPath);
                    string oldFileName = Path.Combine(oldDirPath, fileName);
                    if (File.Exists(oldFileName))
                        File.Delete(oldFileName);
                    File.Copy(finalPath, oldFileName);
                }
                try
                {
                    string serialized = JsonConvert.SerializeObject(data);
                    File.WriteAllText(finalPath, serialized);
                }
                catch(Exception e)
                {
                    ExceptMessage.PopupExcept(e);
                }
            });
            return true;
        }
        public bool TryDelete(string path)
        {
            string finalPath = Path.Combine(RootPath, path);
            try
            {
                File.Delete(finalPath);
                return true;
            }
            catch(Exception e)
            {
                ExceptMessage.PopupExcept(e);
                return false;
            }
        }
        public string[] GetFiles(string path, string pattern = "*", string excepts = null)
        {
            string finalPath = Path.Combine(RootPath, path);
            if (Directory.Exists(finalPath))
            {
                var result = Directory.GetFiles(finalPath, pattern);
                List<string> list = new List<string>();
                foreach(var item in result)
                {
                    if(excepts != null)
                    {
                        if(RunPathEndFilters(item, excepts.Split('|')))
                            list.Add(Path.GetFileName(item));
                    }
                    else
                        list.Add(Path.GetFileName(item));
                }
                return list.ToArray();
            }
            else
                return Array.Empty<string>();
        }
        /// <summary>
        /// Return true if doesn't match any of filters, otherwize will return false.
        /// </summary>
        /// <param name="filters">Blacklists</param>
        /// <returns></returns>
        private bool RunPathEndFilters (string path, string[] filters)
        {
            foreach(var item in filters)
            {
                if (path.EndsWith(item, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
        public void StopService()
        {
        }

        private void CreateIfNotExistDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
