#region

using UnityEngine;

#endregion

namespace Feature.Common.Parameter
{
    [CreateAssetMenu(fileName = "DaggerParams.asset", menuName = "DaggerParams", order = 0)]
    public class DaggerPrams : ScriptableObject
    {
        public float lifeTime = 3f;
        public float speed = 20;
        public uint damage = 1;
    }
}