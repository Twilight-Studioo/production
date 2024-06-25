using UnityEngine;
using VContainer;
using VContainer.Unity;
using Script.Feature.Presenter;
using Script.Feature.Model;
using Script.Feature.View;
using UnityEngine.InputSystem;
using UniRx;

namespace Script.Main.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 10f;

        private PlayerModel playerModel;
        private PlayerView playerView;
        private PlayerPresenter playerPresenter;

        private void Start()
        {
            // プレイヤーのモデル、ビュー、プレゼンターを初期化
            playerModel = new PlayerModel(moveSpeed, jumpForce);
            playerView = FindObjectOfType<PlayerView>(); // シーン内のPlayerViewを見つける
            playerPresenter = new PlayerPresenter(playerView,playerModel);
        }

        private void Update()
        {
            // 入力処理
            float horizontalInput = Input.GetAxis("Horizontal");
            bool jumpInput = Input.GetKeyDown(KeyCode.Space);
            bool attackInput = Input.GetKeyDown(KeyCode.Mouse0); // マウスの左クリック

            // プレゼンターに入力を渡す
            playerPresenter.Move(horizontalInput);
        
            if (jumpInput)
            {
                playerPresenter.Jump();
            }

            if (attackInput)
            {
                playerPresenter.Attack();
            }
        }
    }
}

