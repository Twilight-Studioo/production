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
    }
}