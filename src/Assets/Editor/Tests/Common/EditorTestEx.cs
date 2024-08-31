namespace Editor.Tests.Common
{
    public static class EditorTestEx
    {
        public static T CreateScriptableObject<T>() where T : UnityEngine.ScriptableObject
        {
            return UnityEngine.ScriptableObject.CreateInstance<T>();
        }
        
    }
}