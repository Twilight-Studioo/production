#region

using System;
using UnityEngine;
using VContainer;

#endregion

namespace Feature.View
{
    public class WeponView : MonoBehaviour
    {
        private PlayerView playerView;
        private GameObject playerObject;
        private float lifeTime = 0.5f;
        private Animator animator;
        private void Start()
        {
            playerView = GetComponentInParent<PlayerView>();
            playerObject = GameObject.Find("Player").gameObject;
            Vector3 playerPos = playerObject.transform.position;
            Destroy(this.gameObject,lifeTime);
            Destroy(transform.parent.gameObject,lifeTime);
        }

        private void Update()
        {
            // Position.Value = transform.position;
            //
            // 
            // if (!animator.isActiveAndEnabled)
            // {
            //     return;
            // }
            //
            // var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            //
            // // 現在のアニメーションが指定したアニメーションであり、かつそのアニメーションが終了したかどうかを確認
            // if (stateInfo.IsName("SwordRight") && stateInfo.normalizedTime >= 1.0f)
            // {
            //     StopAnimation();
            // }
        }
        // private void StopAnimation()
        // {
        //     animator.SetBool("Right", false);
        // }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}