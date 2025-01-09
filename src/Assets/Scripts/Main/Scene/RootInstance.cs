#region

using System.Collections.Generic;
using Feature.Interface;
using VContainer;

#endregion

namespace Main.Scene
{
    public class RootInstance
    {
        private Stack<Generated.Scene> history = new Stack<Generated.Scene>();
        [Inject]
        public RootInstance()
        {
        }

        public ISceneDataModel CurrentDataModel { get; set; }

        public T TryGetDataModel<T>() where T : class, ISceneDataModel
        {
            return CurrentDataModel as T;
        }
        
        public void AddHistory(Generated.Scene scene)
        {
            // Add to history
            history.Push(scene);
        }

        public Stack<Generated.Scene> GetHistory()
        {
            return history;
        }
    }
}