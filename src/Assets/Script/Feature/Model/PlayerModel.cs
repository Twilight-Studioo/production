using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Feature.Model
{
    /// <summary>
    /// Playerの状態を管理するクラス
    /// </summary>
    public class PlayerModel
    {
        public Vector2 MoveDirection { get; set; }
        public bool StayGround { get; set; }
        public float JunpPower = 200f;
        public float OnGroundMoveSpeed = 3f;


        public void SetStayGround(bool stayGround)
        {
            StayGround = stayGround;
        }
    }
}