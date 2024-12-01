#region

using UnityEngine;

#endregion

namespace Core.Utilities.Save
{
    public class LocalSave
    {
        // シングルトンインスタンス
        private static LocalSave instance;

        // コンストラクタ
        protected LocalSave()
        {
        }

        public static LocalSave Instance
        {
            get { return instance ??= new(); }
        }

        // intの保存
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }

        // intの取得
        public int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

        // floatの保存
        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        // floatの取得
        public float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);

        // stringの保存
        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }

        // stringの取得
        public string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

        // boolの保存
        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }

        // boolの取得
        public bool GetBool(string key, bool defaultValue = false)
        {
            var defaultInt = defaultValue ? 1 : 0;
            return PlayerPrefs.GetInt(key, defaultInt) == 1;
        }

        // オブジェクトの保存（Serializableなクラス）
        public void SetObject<T>(string key, T obj) where T : class
        {
            var json = JsonUtility.ToJson(obj);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        // オブジェクトの取得
        public T GetObject<T>(string key) where T : class
        {
            var json = PlayerPrefs.GetString(key, null);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            return JsonUtility.FromJson<T>(json);
        }

        // データの削除
        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        // 全データの削除
        public void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}