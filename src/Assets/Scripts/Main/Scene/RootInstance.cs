#region

using System.Collections.Generic;
using Feature.Interface;

#endregion

namespace Main.Scene
{
    public class RootInstance
    {
        private static RootInstance instance;
        private readonly Stack<Generated.Scene> history = new();
        public static RootInstance Shared => instance ??= new();

        public ISceneDataModel CurrentDataModel { get; set; }

        public T TryGetDataModel<T>() where T : class, ISceneDataModel => CurrentDataModel as T;

        public void AddHistory(Generated.Scene scene)
        {
            // Add to history
            history.Push(scene);
        }

        public Stack<Generated.Scene> GetHistory() => history;
    }
}