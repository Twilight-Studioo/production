#region

using UnityEngine;

#endregion

namespace Editor.Tests.Common
{
    public static class EditorTestEx
    {
        public static T CreateScriptableObject<T>() where T : ScriptableObject => ScriptableObject.CreateInstance<T>();
    }
}