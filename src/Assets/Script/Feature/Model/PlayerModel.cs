using System;
using UniRx;
using UnityEngine;
using VContainer;
namespace Script.Feature.Model
{
    public class PlayerModel
    {
        public float MoveSpeed { get; private set; }
        public float JumpForce { get; private set; }

        public event Action OnAttack;

        public PlayerModel(float moveSpeed, float jumpForce)
        {
            MoveSpeed = moveSpeed;
            JumpForce = jumpForce;
        }

        public void Attack()
        {
            OnAttack?.Invoke();
        }
        
    }
}