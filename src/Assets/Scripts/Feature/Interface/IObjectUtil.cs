namespace Feature.Interface
{
    public interface IObjectUtil
    {
        T[] FindObjectsOfType<T>() where T : UnityEngine.Object;
    }
    
    public class ObjectUtil: IObjectUtil
    {
        public T[] FindObjectsOfType<T>() where T : UnityEngine.Object
        {
            return UnityEngine.Object.FindObjectsOfType<T>();
        }
    }
}