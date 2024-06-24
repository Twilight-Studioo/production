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
        [SerializeField]private InputActionReference controls;

         private void Awake()
         {
             playerModel = new PlayerModel();
             playerView = GetComponent<PlayerView>();
         }

         private void OnEnable()
         {
             controls.action.performed += OnMove;
             controls.action.canceled  += OnMove;
             Enable();
         }

         private void OnDisable()
         {
             controls.action.performed -= OnMove;
             controls.action.canceled  -= OnMove;
             Disable();
         }

         private void Update()
         {
             playerView.UpDatePosition(new Vector2(playerModel.MoveDirection.x,0));
         }

         /// <summary>
         /// InputActionを有効可
         /// </summary>
         private void Enable()  => controls.action.Enable();
         /// <summary>
         /// InputActionを無効可
         /// </summary>
         private void Disable() => controls.action.Disable();
         private void OnMove(InputAction.CallbackContext context)
         {
             // 2軸入力を受け取る
             var move = context.ReadValue<Vector2>();
             playerModel.MoveDirection = move;

             // 2軸入力の値を表示
             print($"move: {move}");
         }
    }
}