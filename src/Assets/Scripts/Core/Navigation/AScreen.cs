using UnityEngine;

namespace Core.Navigation
{
    public abstract class AScreen: MonoBehaviour
    {
        public abstract void OnCreate();

        public virtual void OnShow()
        {
        }

        public virtual void OnHide()
        {
        }

        public virtual void OnDestroy()
        {
        }
    }
}