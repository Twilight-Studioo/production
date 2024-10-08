#region

using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    /// <summary>
    ///     Provides extension methods for controlling player animations in Unity.
    /// </summary>
    public static class PlayerAnimationExtension
    {
        /// <summary>
        ///     Name of the boolean parameter used to indicate whether the player is falling in the animation.
        /// </summary>
        private const string IsFalling = "IsFalling";

        /// <summary>
        ///     Name of the float parameter used to represent the player's speed in the animation.
        /// </summary>
        private const string Speed = "Speed";

        /// <summary>
        ///     Name of the trigger parameter used to denote the jump action in the animation.
        /// </summary>
        private const string Jump = "OnJump";

        /// <summary>
        ///     Name of the trigger parameter used to denote the take damage action in the animation.
        /// </summary>
        private const string TakeDamage = "OnTakeDamage";

        /// <summary>
        ///     Name of the trigger parameter used to denote the dagger action in the animation.
        /// </summary>
        private const string Dagger = "OnDagger";

        /// <summary>
        ///     Name of the trigger parameter used to denote the attack action in the animation.
        /// </summary>
        private const string Attack = "OnAttack";

        /// <summary>
        ///     Name of the boolean parameter used to indicate whether the player is attacking in the animation.
        /// </summary>
        private const string IsAttacking = "IsAttacking";

        /// <summary>
        ///     Name of the integer parameter used to count the number of attack combos in the animation.
        /// </summary>
        private const string AttackComboCount = "AttackComboCount";

        public static readonly int IsFallingHash = Animator.StringToHash(IsFalling);
        public static readonly int SpeedHash = Animator.StringToHash(Speed);
        public static readonly int OnJumpHash = Animator.StringToHash(Jump);
        public static readonly int OnTakeDamageHash = Animator.StringToHash(TakeDamage);
        public static readonly int OnDaggerHash = Animator.StringToHash(Dagger);
        public static readonly int OnAttackHash = Animator.StringToHash(Attack);
        public static readonly int IsAttackingHash = Animator.StringToHash(IsAttacking);
        public static readonly int AttackComboCountHash = Animator.StringToHash(AttackComboCount);

        public static void SetIsFalling(this IAnimator animator, bool isFalling)
        {
            animator.SetBool(IsFallingHash, isFalling);
        }

        public static void SetSpeed(this IAnimator animator, float speed)
        {
            animator.SetFloat(SpeedHash, speed);
        }

        public static void OnJump(this IAnimator animator)
        {
            animator.SetTrigger(OnJumpHash);
        }

        public static void OnTakeDamage(this IAnimator animator)
        {
            animator.SetTrigger(OnTakeDamageHash);
        }

        public static void OnDagger(this IAnimator animator)
        {
            animator.SetTrigger(OnDaggerHash);
        }

        public static void OnAttack(this IAnimator animator, float attackTime)
        {
            animator.SetTrigger(OnAttackHash);
            animator.SetBool(IsAttackingHash, true);
            animator.SetBoolDelay(IsAttackingHash, false, attackTime);
        }

        public static void SetAttackComboCount(this IAnimator animator, float count)
        {
            animator.SetFloat(AttackComboCountHash, count);
        }
    }
}