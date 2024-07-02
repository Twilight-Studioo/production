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
        public float JumpMove { get; private set; }
        public float Attack { get; private set; }

        public PlayerModel(float moveSpeed, float jumpForce,float jumpMove, float attack)
        {
            MoveSpeed = moveSpeed;
            JumpForce = jumpForce;
            JumpMove = jumpMove;
            Attack = attack;
        }
        
    }
}