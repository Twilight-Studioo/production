#region

using System;
using UniRx;
using UnityEngine;

#endregion

namespace Feature.Interface
{
    public interface IPlayerView
    {
        float SwapRange { set; }

        bool IsDrawSwapRange { set; }

        IReadOnlyReactiveProperty<float> Speed { get; }
        Transform GetTransform();

        event Action<uint> OnDamageEvent;

        IReadOnlyReactiveProperty<Vector3> GetPositionRef();

        GameObject GetGameObject();

        void Move(Vector3 direction, float speed);

        void AddForce(Vector3 force);

        void Jump(float jumpPower);

        void SetPosition(Vector3 position);

        void Attack(float degree, uint damage);

        void Dagger(float degree, float h, float v);

        void SwapTimeStartUrp();

        void SwapTimeFinishUrp();

        void SetParam(float comboTimeWindow, float comboAngleOffset, float maxComboCount,
            MonoBehaviour urp, float attackCoolTime, AudioSource audioSource);

        Vector3 GetForward();
    }
}