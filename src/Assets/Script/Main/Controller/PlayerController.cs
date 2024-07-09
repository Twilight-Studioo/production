﻿using System;
using System.Linq;
using Core.Camera;
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
        public float attack = 1;
        private PlayerModel playerModel;
        private PlayerView playerView;
        private PlayerPresenter playerPresenter;
        private VFXView vfxView;
        private InputPlayer inputPlayer;
        private float horizontalInput;
        private Vector2 attackDirection;
       
        [SerializeField]
        private TargetGroupManager targetGroupManager;

        [SerializeField]
        private Transform[] enemies;


        private InputAction playerAction;
        
        private void Awake()
        {
            // プレイヤーのモデル、ビュー、プレゼンターを初期化
            playerModel = new PlayerModel(moveSpeed, jumpForce, jumpMove, attack);
            playerView = FindObjectOfType<PlayerView>(); // シーン内のPlayerViewを見つける
            playerPresenter = new PlayerPresenter(playerView,playerModel);
            vfxView = FindObjectOfType<VFXView>();
            inputPlayer = new InputPlayer();
            inputPlayer.Enable();
            if (targetGroupManager == null)
            {
                // TODO: diでちゃんと管理する
                var manager = FindObjectOfType<TargetGroupManager>();
                if (manager == null)
                {
                    throw new NotImplementedException("TargetGroupManagerが見つかりませんでした");
                }
                targetGroupManager = manager;
            }
            targetGroupManager.AddTarget(playerView.transform, CameraTargetGroupTag.Player());
            enemies.ToList().ForEach((enemy) => 
                targetGroupManager.AddTarget(enemy, CameraTargetGroupTag.Enemy())
            );
        }

        private void Update()
        {
            // 入力処理
            // var horizontalInput = inputPlayer.Player.Move.ReadValue<Vector2>().x;
            // bool jumpInput = inputPlayer.Player.Jump.triggered;
            // bool attackInput = inputPlayer.Player.Attack.triggered;
            // bool SwapInput = inputPlayer.Player.SwapMode.triggered;
            // プレゼンターに入力を渡す
            attackDirection = inputPlayer.Player.Move.ReadValue<Vector2>();
            horizontalInput = attackDirection.x;
            playerPresenter.Move(horizontalInput);
            
            if (inputPlayer.Player.Attack.triggered)
            {
                playerPresenter.Attack(attackDirection);
            }

            if (inputPlayer.Player.Jump.triggered)
            {
                playerPresenter.Jump();
            }  
        }
        public void OnSwap(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                playerPresenter.Swap();
                vfxView.PlayVFX();
            }

            if (inputPlayer.Player.SwapMode.triggered)
            {
                playerPresenter.Swap();
                vfxView.StopVFX();
            }
        }

        // public void OnMove(InputAction.CallbackContext context)
        // {
        //     if (context.phase==InputActionPhase.Performed)
        //     { 
        //         attackDirection = context.ReadValue<Vector2>();
        //         
        //         horizontalInput = attackDirection.x;
        //     }
        //     if (context.phase == InputActionPhase.Canceled)
        //     {
        //         attackDirection = Vector2.zero;
        //         horizontalInput = attackDirection.x;
        //     }
        // }

        // public void OnJump(InputAction.CallbackContext context)
        // {
        //     if (context.phase == InputActionPhase.Started)
        //     {
        //         playerPresenter.Jump();
        //     }  
        // }

        // public void OnSwap(InputAction.CallbackContext context)
        // {
        //     if (context.phase == InputActionPhase.Performed)
        //     {
        //         playerPresenter.Swap();
        //     }
        //     if (context.phase == InputActionPhase.Canceled)
        //     {
        //         playerPresenter.Swap();
        //     }
        // }

        // public void OnAttack(InputAction.CallbackContext context)
        // {
        //     if (context.phase == InputActionPhase.Started)
        //     { 
        //         playerPresenter.Attack(attackDirection);
        //     }
        // }
    }
}

