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

        private bool isPlaySwapEffect;

        public void PlayVFX()
        {
            if (effect != null && !isPlaySwapEffect)
            {
                effect.SendEvent("OnPlay");
                isPlaySwapEffect = true;
                Invoke("StopVFX", onStopTime);
            }
        }

        public void StopVFX()
        {
            if (effect != null)
            {
                effect.SendEvent("OnStop");
            }

            isPlaySwapEffect = false;
        }
    }
}