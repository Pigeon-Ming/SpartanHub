using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace SpartanHub.Service
{
    public class SettingsService
    {
        private static readonly Lazy<SettingsService> _instance = new Lazy<SettingsService>(() => new SettingsService());
        
        public static SettingsService Instance => _instance.Value;

        private readonly ApplicationDataContainer _localSettings;
        private readonly ApplicationDataContainer _roamingSettings;

        private SettingsService()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
            _roamingSettings = ApplicationData.Current.RoamingSettings;
        }

        public void SetValue(string key, string value, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            container.Values[key] = value;
        }

        public void SetValue(string key, int value, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            container.Values[key] = value;
        }

        public void SetValue(string key, bool value, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            container.Values[key] = value;
        }

        public string GetValue(string key, string defaultValue = null, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            
            if (container.Values.TryGetValue(key, out var value))
            {
                return value as string;
            }

            return defaultValue;
        }

        public int GetValue(string key, int defaultValue, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            
            if (container.Values.TryGetValue(key, out var value))
            {
                return (int)value;
            }

            return defaultValue;
        }

        public bool GetValue(string key, bool defaultValue, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            
            if (container.Values.TryGetValue(key, out var value))
            {
                return (bool)value;
            }

            return defaultValue;
        }

        public T GetValue<T>(string key, T defaultValue, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            
            if (container.Values.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return defaultValue;
        }

        public bool Contains(string key, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            return container.Values.ContainsKey(key);
        }

        public void Remove(string key, bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            
            if (container.Values.ContainsKey(key))
            {
                container.Values.Remove(key);
            }
        }

        public void Clear(bool isRoaming = false)
        {
            var container = isRoaming ? _roamingSettings : _localSettings;
            container.Values.Clear();
        }

        public async Task SaveStringAsync(string fileName, string content)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(file, content);
            }
            catch (Exception)
            {
            }
        }

        public async Task<string> LoadStringAsync(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName);
                return await FileIO.ReadTextAsync(file);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool FileExists(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = folder.GetFileAsync(fileName).AsTask().Result;
                return file != null;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteFileAsync(string fileName)
        {
            try
            {
                var folder = ApplicationData.Current.LocalFolder;
                var file = await folder.GetFileAsync(fileName);
                await file.DeleteAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}
