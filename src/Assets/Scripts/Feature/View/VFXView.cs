#region

using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Feature.View
{
    public class VFXView : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private float onStopTime = 1f;
        [SerializeField] private bool isSwap = false;

        public void PlayVFX()
        {
            if (effect != null)
            {
                if (isSwap)
                {
                    effect.SendEvent("OnPlay");
                    Invoke("StopVFX", onStopTime);
                }
            }
        }

        public void StopVFX()
        {
            if (effect != null)
            {
                effect.SendEvent("OnStop");
            }
        }

        public void StartSwap()
        {
            isSwap = true;
            Debug.Log($"StartSwap called. isSwapActive = {isSwap}");
            Invoke("EndSwap", onStopTime);
        }

        public void EndSwap()
        {
            isSwap = false;
            Debug.Log($"EndSwap called. isSwapActive = {isSwap}");
        }
        
        public bool IsSwap()
        {
            return isSwap;
        }
    }
}