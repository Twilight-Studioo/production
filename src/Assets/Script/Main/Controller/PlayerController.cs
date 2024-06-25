using UnityEngine;
using VContainer;
using VContainer.Unity;
using Script.Feature.Presenter;
using Script.Feature.Model;
using Script.Feature.View;
using UnityEngine.InputSystem;
using UniRx;
using UnityEngine.Rendering;

namespace Script.Main.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public float jumpMove = 2;
        private PlayerModel playerModel;
        private PlayerView playerView;
        private PlayerPresenter playerPresenter;
        private InputPlayer inputPlayer;
        private float horizontalInput;

        private void Start()
        {
            // プレイヤーのモデル、ビュー、プレゼンターを初期化
            playerModel = new PlayerModel(moveSpeed, jumpForce, jumpMove);
            playerView = FindObjectOfType<PlayerView>(); // シーン内のPlayerViewを見つける
            playerPresenter = new PlayerPresenter(playerView,playerModel);
            inputPlayer = new InputPlayer();
        }

        private void Update()
        {
            // 入力処理
            // var horizontalInput = inputPlayer.Player.Move.ReadValue<Vector2>().x;
            // bool jumpInput = inputPlayer.Player.Jump.triggered;
            // bool attackInput = inputPlayer.Player.Attack.triggered;
            // bool SwapInput = inputPlayer.Player.SwapMode.triggered;
            // プレゼンターに入力を渡す
            playerPresenter.Move(horizontalInput);
            
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase==InputActionPhase.Performed)
            {
                Vector2 input = context.ReadValue<Vector2>();
                
                horizontalInput = input.x;
            }

            if (context.phase == InputActionPhase.Canceled)
            {
                Vector2 input = Vector2.zero;
                horizontalInput = input.x;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                playerPresenter.Jump();
            }  
        }

        public void OnSwap(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                playerPresenter.Swap();
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                playerPresenter.Swap();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                playerPresenter.Attack();
            }
        }
    }
}

