#region

using Core.Utilities;
using Feature.Interface;
using UnityEngine;

#endregion

namespace Feature.View
{
    internal class AnimationWrapper : IAnimator
    {
        private readonly Animator animator;
        private readonly MonoBehaviour monoBehaviour;


        public AnimationWrapper(Animator animator, MonoBehaviour monoBehaviour)
        {
            this.animator = animator;
            this.monoBehaviour = monoBehaviour;
        }

        public void SetBool(int id, bool value) => animator.SetBool(id, value);
        public void SetFloat(int id, float value) => animator.SetFloat(id, value);
        public void SetTrigger(int id) => animator.SetTrigger(id);
        public void SetInteger(int id, int value) => animator.SetInteger(id, value);

        public void SetBoolDelay(int id, bool value, float delay)
        {
            monoBehaviour.StartCoroutine(
                monoBehaviour.DelayMethod(delay, () => animator.SetBool(id, value))
            );
        }
    }

    internal static class AnimationWrapperExtension
    {
        public static AnimationWrapper Create(this MonoBehaviour monoBehaviour, Animator animator) =>
            new(animator, monoBehaviour);
    }
}