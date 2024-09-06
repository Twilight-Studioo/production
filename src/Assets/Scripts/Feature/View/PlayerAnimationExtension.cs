using UnityEngine;

namespace Feature.View
{
    /// <summary>
    /// 
    /// </summary>
    internal static class PlayerAnimationExtension
    {
        /// <summary>
        /// Name of the boolean parameter used to indicate whether the player is falling in the animation.
        /// </summary>
        private const string IsFalling = "IsFalling";
        private static readonly int IsFallingHash = Animator.StringToHash(IsFalling);

        /// <summary>
        /// Name of the float parameter used to represent the player's speed in the animation.
        /// </summary>
        private const string Speed = "Speed";
        private static readonly int SpeedHash = Animator.StringToHash(Speed);

        /// <summary>
        /// Name of the trigger parameter used to denote the jump action in the animation.
        /// </summary>
        private const string Jump = "OnJump";
        private static readonly int OnJumpHash = Animator.StringToHash(Jump);

        /// <summary>
        /// Name of the trigger parameter used to denote the take damage action in the animation.
        /// </summary>
        private const string TakeDamage = "OnTakeDamage";
        private static readonly int TakeDamageHash = Animator.StringToHash(TakeDamage);

        /// <summary>
        /// Name of the trigger parameter used to denote the dagger action in the animation.
        /// </summary>
        private const string Dagger = "OnDagger";
        private static readonly int OnDaggerHash = Animator.StringToHash(Dagger);

        /// <summary>
        /// Name of the trigger parameter used to denote the attack action in the animation.
        /// </summary>
        private const string Attack = "OnAttack";
        private static readonly int OnAttackHash = Animator.StringToHash(Attack);

        /// <summary>
        /// Name of the integer parameter used to count the number of attack combos in the animation.
        /// </summary>
        private const string AttackComboCount = "AttackComboCount";
        private static readonly int AttackComboCountHash = Animator.StringToHash(AttackComboCount);
        
        public static void SetIsFalling(this Animator animator, bool isFalling)
        {
            animator.SetBool(IsFallingHash, isFalling);
        }
        
        public static void SetSpeed(this Animator animator, float speed)
        {
            animator.SetFloat(SpeedHash, speed);
        }
        
        public static void OnJump(this Animator animator)
        {
            animator.SetTrigger(OnJumpHash);
        }
        
        public static void OnTakeDamage(this Animator animator)
        {
            animator.SetTrigger(TakeDamageHash);
        }
        
        public static void OnDagger(this Animator animator)
        {
            animator.SetTrigger(OnDaggerHash);
        }
        
        public static void OnAttack(this Animator animator)
        {
            animator.SetTrigger(OnAttackHash);
        }
        
        public static void SetAttackComboCount(this Animator animator, int count)
        {
            animator.SetInteger(AttackComboCountHash, count);
        }
    }
}