using UnityEngine;

namespace Core.Utilities.Effect
{
    public class EffectInstance<T> where T : MonoBehaviour
    {
        public T Effect { get; private set; }
        public int UsageCount { get; private set; }
        public float LastUsedTime { get; private set; }

        public EffectInstance(T effect)
        {
            Effect = effect;
            UsageCount = 0;
            LastUsedTime = Time.time;
        }

        public void UpdateUsage()
        {
            UsageCount++;
            LastUsedTime = Time.time;
        }
    }
}