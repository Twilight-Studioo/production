#region

using UnityEngine;
using UnityEngine.VFX;

#endregion

namespace Feature.Component
{
    public class SelectorEffect : MonoBehaviour
    {
        [SerializeField] private VisualEffect effect;

        public void Selector(Vector3 position)
        {
            transform.position = position;
            PlayVFX();
        }

        public void SelectorStop()
        {
            StopVFX();
        }

        private void PlayVFX()
        {
            effect?.SendEvent("OnPlay");
        }

        private void StopVFX()
        {
            if (effect)
            {
                effect.SendEvent("OnStop");
            }
        }
    }
}