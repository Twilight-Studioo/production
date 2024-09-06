#region

using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    internal class AnimationWrapper : IAnimator
    {
        private readonly Animator animator;

        public AnimationWrapper(Animator animator)
        {
            this.animator = animator;
        }

        public void SetBool(int id, bool value) => animator.SetBool(id, value);
        public void SetFloat(int id, float value) => animator.SetFloat(id, value);
        public void SetTrigger(int id) => animator.SetTrigger(id);
        public void SetInteger(int id, int value) => animator.SetInteger(id, value);
    }
}