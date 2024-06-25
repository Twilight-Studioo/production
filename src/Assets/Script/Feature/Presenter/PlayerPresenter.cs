using System;
using Script.Feature.Model;
using Script.Feature.View;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Feature.Presenter
{
    public class PlayerPresenter : MonoBehaviour
    {
        private PlayerModel playerModel;
        [SerializeField]private PlayerView playerView;
        private Controls controls;

         private void Awake()
         {
             playerModel = new PlayerModel();
             playerView = GetComponent<PlayerView>();
             controls = new Controls();
         }

         private void OnEnable()
         {
             controls.Player.Move.performed += OnMove;   
             controls.Player.Move.canceled  += OnMove;
             Enable();
         }

         private void OnDisable()
         {
             controls.Player.Move.performed -= OnMove;
             controls.Player.Move.canceled  -= OnMove;
             Disable();
         }

         private void Update()
         {
             
             if (playerModel.StayGround)
             {
                 playerView.UpDatePosition(new Vector2(playerModel.MoveDirection.x*playerModel.OnGroundMoveSpeed,0));
             }
             else
             {
                 playerView.UpDatePosition(new Vector2(playerModel.MoveDirection.x,0));
             }
             
         }

         /// <summary>
         /// InputActionを有効可
         /// </summary>
         private void Enable()  => controls.Player.Move.Enable();
         /// <summary>
         /// InputActionを無効可
         /// </summary>
         private void Disable() => controls.Player.Move.Disable();
         private void OnMove(InputAction.CallbackContext context)
         {
             // 2軸入力を受け取る
             var move = context.ReadValue<Vector2>();
             playerModel.MoveDirection = move;

             // 2軸入力の値を表示
             print($"move: {move}");
         }

         private void OnJump()
         {
             if (!playerModel.StayGround)
             {
                 print($"Can't Jump");
                 return;
             }
             print($"Jump");
             playerModel.SetStayGround(false);
             playerView.UpDatePosition(Vector2.up*playerModel.JunpPower);
             
         }

         private void OnCollisionEnter(Collision other)
         {
             if (other.gameObject.CompareTag("Ground"))
             {
                 playerModel.SetStayGround(true);
             }
         }
    }
}