using System;
using UniRx;

namespace Core.Utilities.Save
{
    public class LocalSaveReactive : LocalSave
    {
        private readonly BehaviorSubject<string> onValueChanged = new("");

        public IObservable<string> OnValueChanged => onValueChanged;

        public new void SetInt(string key, int value)
        {
            base.SetInt(key, value);
            onValueChanged.OnNext(key);
        }

        public new void SetFloat(string key, float value)
        {
            base.SetFloat(key, value);
            onValueChanged.OnNext(key);
        }

        public new void SetString(string key, string value)
        {
            base.SetString(key, value);
            onValueChanged.OnNext(key);
        }

        public new void SetBool(string key, bool value)
        {
            base.SetBool(key, value);
            onValueChanged.OnNext(key);
        }

        public new void SetObject<T>(string key, T obj) where T : class
        {
            base.SetObject(key, obj);
            onValueChanged.OnNext(key);
        }

        public new void DeleteKey(string key)
        {
            base.DeleteKey(key);
            onValueChanged.OnNext(key);
        }

        public new void DeleteAll()
        {
            base.DeleteAll();
            onValueChanged.OnNext(null);
        }
    }
}