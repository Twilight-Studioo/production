#region

using System;
using System.Reflection;

#endregion

namespace Editor.Tests.Common
{
    public static class ReflectionHelper
    {
        // 任意のオブジェクトのprivateフィールドの値を設定する汎用メソッド
        public static void SetPrivateField<T>(object obj, string fieldName, T value)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found on type '{obj.GetType().Name}'.");
            }

            field.SetValue(obj, value);
        }

        // 任意のオブジェクトのprivateフィールドの値を取得する汎用メソッド
        public static T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new ArgumentException($"Field '{fieldName}' not found on type '{obj.GetType().Name}'.");
            }

            return (T)field.GetValue(obj);
        }

        // 任意のオブジェクトのprivateプロパティの値を設定する汎用メソッド
        public static void SetPrivateProperty<T>(object obj, string propertyName, T value)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().Name}'.");
            }

            property.SetValue(obj, value);
        }

        // 任意のオブジェクトのprivateプロパティの値を取得する汎用メソッド
        public static T GetPrivateProperty<T>(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().Name}'.");
            }

            return (T)property.GetValue(obj);
        }

        // 任意のオブジェクトのprivateメソッドを呼び出す汎用メソッド
        public static object InvokePrivateMethod(object obj, string methodName, params object[] parameters)
        {
            var method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                throw new ArgumentException($"Method '{methodName}' not found on type '{obj.GetType().Name}'.");
            }

            return method.Invoke(obj, parameters);
        }

        // フィールドまたはプロパティの取得を試みるメソッド
        public static T GetPrivateFieldOrProperty<T>(object obj, string name)
        {
            var field = obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(obj);
            }

            var property = obj.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property != null)
            {
                return (T)property.GetValue(obj);
            }

            throw new ArgumentException($"Field or Property '{name}' not found on type '{obj.GetType().Name}'.");
        }

        // フィールドまたはプロパティの設定を試みるメソッド
        public static void SetPrivateFieldOrProperty<T>(object obj, string name, T value)
        {
            var field = obj.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
                return;
            }

            var property = obj.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new ArgumentException($"Field or Property '{name}' not found on type '{obj.GetType().Name}'.");
            }

            property.SetValue(obj, value);
        }
    }
}