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

        // damage & hit point
        public event DamageHandler<uint, Vector3> OnDamageEvent;

        public event Action<DamageResult> OnHitHandler;

        IReadOnlyReactiveProperty<Vector3> GetPositionRef();

        GameObject GetGameObject();

        void Move(Vector3 direction, float speed);

        void AddForce(Vector3 force);

        void Jump(float jumpPower);

        void SetPosition(Vector3 position);

        bool CanAttack();

        void Attack(float degree, uint damage, bool voltage);

        void Dagger(float degree, float h, float v);

        void SetParam(float comboTimeWindow, float comboAngleOffset, float maxComboCount, float attackCoolTime,
            float maxComboCoolTime);

        public void VoltageEffect(int voltageValue, int useVoltageAttackValue, int votageTwoAttackValue,
            int maxVoltage);

        Vector3 GetForward();
    }
}