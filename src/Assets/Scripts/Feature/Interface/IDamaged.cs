#region

using UnityEngine;

#endregion

namespace Feature.Interface
{
    public record DamageResult(Transform Target = default)
    {
        public Transform Target { get; } = Target;

        public record Killed(Transform Target) : DamageResult(Target);

        public record Damaged(Transform Target) : DamageResult(Target);

        public record Missed : DamageResult;
    }

    public interface IDamaged
    {
        public DamageResult OnDamage(uint damage, Vector3 hitPoint, Transform attacker);
    }

    public delegate DamageResult DamageHandler();

    public delegate DamageResult DamageHandler<in T>(T t1);

    public delegate DamageResult DamageHandler<in T1, in T2>(T1 t1, T2 t2);
}