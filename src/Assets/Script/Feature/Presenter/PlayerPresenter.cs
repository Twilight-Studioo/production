using System.Collections;
using System.Collections.Generic;
using Script.Feature.Model;
using Script.Feature.View;
using Script.Feature.Interface.Presenter;
using UniRx;
using UnityEngine;
using VContainer;

namespace Script.Feature.Presenter
{
    public class PlayerPresenter
    {
        private readonly PlayerModel playerModel;
        private readonly PlayerView playerView;
        
        [Inject]
        public PlayerPresenter(
            PlayerView view,
            PlayerModel model
        )
        {
            this.playerView = view;
            this.playerModel = model;
            
            // ビューのイベントにプレゼンターのメソッドを登録
            view.OnJump += Jump;
            view.OnAttack += Attack;
        }
        public void Move(float direction)
        {
            playerView.Move(direction * playerModel.MoveSpeed);
        }

        public void Jump()
        { 
            playerView.Jump(playerModel.JumpForce);

        }

        public void Attack()
        {
            playerModel.Attack(); // モデルの攻撃メソッドを呼び出し、ビューの攻撃アニメーションをトリガーする
        }
    }
}
