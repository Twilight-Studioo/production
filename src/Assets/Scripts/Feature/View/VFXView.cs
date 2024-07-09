using UnityEngine;
using UnityEngine.VFX;

namespace Feature.View
{
    public class VFXView : MonoBehaviour
    {
        [SerializeField] VisualEffect effect;

        public void PlayVFX()
        {
            if (effect != null)
            {
                effect.SendEvent("OnPlay");
            }
        }
        public void StopVFX()
        {
            if (effect != null)
            {
                effect.SendEvent("StopPlay");
            }
        }
        
    }
}