#region

using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Feature.Component
{
    public class VFXView : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;
        [SerializeField] private float onStopTime = 1f;

        private bool isPlaySwapEffect;

        private void OnEnable()
        {
            isPlaySwapEffect = false;
            PlayVFX();
        }

        private void PlayVFX()
        {
            if (effect != null && !isPlaySwapEffect)
            {
                effect.SendEvent("OnPlay");
                isPlaySwapEffect = true;
                Invoke(nameof(StopVFX), onStopTime);
            }
        }

        private void StopVFX()
        {
            if (effect != null)
            {
                effect.SendEvent("OnStop");
            }

            isPlaySwapEffect = false;
        }
    }
}