using System.Linq;
using Core.Utilities.Save;
using UnityEngine;

namespace Main.Controller.Save
{
    public static class GameSaveKeys
    {
        private const string MasterVolume = "MasterVolume";
        public static string MasterVolumeKey => GetKey(MasterVolume);
            
        private const string BgmVolume = "BgmVolume";
        public static string BgmVolumeKey => GetKey(BgmVolume);

        private const string SeVolume = "SeVolume";
        public static string SeVolumeKey => GetKey(SeVolume);
            
        private static string GetKey(string key)
        {
            var app = Application.productName;
            return $"{app}_{key}";
        }
            
        public static string[] GetAllKeys()
        {
            return new[]
            {
                MasterVolumeKey,
                BgmVolumeKey,
                SeVolumeKey,
            };
        }
    }

    public class GameSaveManager: LocalSaveReactive
    {
        public float GetMasterVolume(float defaultValue = 0.6f)
        {
            return GetFloat(GameSaveKeys.MasterVolumeKey, defaultValue);
        }
        
        public void SetMasterVolume(float value)
        {
            SetFloat(GameSaveKeys.MasterVolumeKey, value);
        }
        
        public float GetBgmVolume(float defaultValue = 0.6f)
        {
            return GetFloat(GameSaveKeys.BgmVolumeKey, defaultValue);
        }
        
        public void SetBgmVolume(float value)
        {
            SetFloat(GameSaveKeys.BgmVolumeKey, value);
        }
        
        public float GetSeVolume(float defaultValue = 0.6f)
        {
            return GetFloat(GameSaveKeys.SeVolumeKey, defaultValue);
        }
        
        public void SetSeVolume(float value)
        {
            SetFloat(GameSaveKeys.SeVolumeKey, value);
        }
        
        public new void DeleteAll()
        {
            var app = Application.productName;
            var keys = GameSaveKeys.GetAllKeys().Where(k => k.StartsWith(app)).ToArray();
            foreach (var key in keys)
            {
                DeleteKey(key);
            }
        }
    }
}