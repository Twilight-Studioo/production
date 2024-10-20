#region

using UnityEngine;

#endregion

namespace Feature.Interface
{
    public interface IObjectUtil
    {
        T[] FindObjectsOfType<T>() where T : Object;
    }

    public class ObjectUtil : IObjectUtil
    {
        public T[] FindObjectsOfType<T>() where T : Object => Object.FindObjectsOfType<T>();
    }
}