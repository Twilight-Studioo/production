using UnityEngine;

namespace Feature.Interface
{
    public interface IDamaged
    {
        public void OnDamage(uint damage, Vector3 hitPoint, Transform attacker);
    }
}