using System.Collections;
using System.Collections.Generic;
using Script.Feature.Model;
using Script.Feature.View;
using UniRx;
using UnityEngine;
using VContainer;

namespace Script.Feature.Presenter
{
    public class PlayerPresenter
    {
        private readonly PlayerModel playerModel;
        private readonly PlayerView playerView;

        
        public PlayerPresenter(
            PlayerView view,
            PlayerModel model
            
        )
        {
            this.playerView = view;
            this.playerModel = model;
            
        }
        public void Move(float direction)
        {
            playerView.Move(direction * playerModel.MoveSpeed,playerModel.JumpMove);
        }

        public void Jump()
        { 
            playerView.Jump(playerModel.JumpForce);

        }

        public void Swap()
        {
            playerView.SwapMode();
        }
        
        public void Attack()
        {
            playerView.Attack(playerModel.Attack); // モデルの攻撃メソッドを呼び出し、ビューの攻撃アニメーションをトリガーする
        }
    }
}
