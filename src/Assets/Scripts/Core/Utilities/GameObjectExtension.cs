#region

using UnityEngine;

#endregion

namespace Core.Utilities
{
    public static class GameObjectExtension
    {
        /// <summary>
        ///     Tries to get a component of type T from the specified GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to retrieve.</typeparam>
        /// <param name="gameObject">The GameObject from which to retrieve the component.</param>
        /// <param name="component">When this method returns, contains the component of type T if found; otherwise, null.</param>
        /// <returns>True if the component is found; otherwise, false.</returns>
        public static bool TryGetComponent<T>(this GameObject gameObject, out T component) where T : Component
        {
            component = gameObject.GetComponent<T>();
            return component is not null;
        }

        /// <summary>
        ///     Tries to get a component of type T from the specified GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to retrieve.</typeparam>
        /// <param name="gameObject">The GameObject from which to retrieve the component.</param>
        /// <returns>True if the component is found; otherwise, false.</returns>
        public static bool TryGetComponent<T>(this GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            return component is not null;
        }
    }
}