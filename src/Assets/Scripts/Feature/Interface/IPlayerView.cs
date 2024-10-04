using System;
using UniRx;
using UnityEngine;

namespace Feature.Interface
{
    public interface IPlayerView
    {
        Transform GetTransform();

        event Action<uint> OnDamageEvent;
        
        float SwapRange { set; }
        
        bool IsDrawSwapRange { set; }
        
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

        void SetParam(float comboTimeWindow, float comboAngleOffset, int maxComboCount,
           MonoBehaviour urp,float attackCoolTime);

        Vector3 GetForward();

        IReadOnlyReactiveProperty<float> Speed { get; }
    }
}