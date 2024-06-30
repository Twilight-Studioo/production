using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Script.Feature.View
{
    public class GameUIView : MonoBehaviour
    {
        public float maxSelectionDistance = 100f;
        public Color selectedColor = Color.red; // 選択されたオブジェクトの色
        private Color originalColor;
        private Camera mainCamera;
        private GameObject[] swappableObjects;
        private int currentIndex = -1;

        private void Start()
        {
            mainCamera = Camera.main;
            // "SwappableObjects" タグがついているオブジェクトを全て取得
            swappableObjects = GameObject.FindGameObjectsWithTag("SwappableObjects");
        }

        private void Update()
        {
            if (Gamepad.current != null && Gamepad.current.rightShoulder.isPressed)
            {
                // RBボタンが押されているときのみ選択を許可
                Vector2 leftStickInput = Gamepad.current.leftStick.ReadValue();

                if (leftStickInput.magnitude > 0.5f)
                {
                    // スティックの入力に基づいてオブジェクトを選択
                    SelectNextObject();
                }

                if (Gamepad.current.aButton.wasPressedThisFrame)
                {
                    // Aボタンが押されたときにオブジェクトを選択
                    ConfirmSelection();
                }
            }
        }

        private void SelectNextObject()
        {
            if (swappableObjects.Length == 0)
                return;

            // 現在選択されているオブジェクトの色をリセット
            if (currentIndex != -1)
            {
                ResetColor(swappableObjects[currentIndex]);
            }

            // 次のオブジェクトを選択
            currentIndex = (currentIndex + 1) % swappableObjects.Length;

            // 新しく選択されたオブジェクトの色を変更
            Renderer renderer = swappableObjects[currentIndex].GetComponent<Renderer>();
            if (renderer != null)
            {
                originalColor = renderer.material.color;
                renderer.material.color = selectedColor;
            }

            Debug.Log("Selected object: " + swappableObjects[currentIndex].name);
        }

        private void ConfirmSelection()
        {
            if (currentIndex != -1)
            {
                GameObject selectedObject = swappableObjects[currentIndex];
                Debug.Log("Confirmed selection: " + selectedObject.name);
                // 選択されたオブジェクトに対する処理をここに記述
            }
        }

        private void ResetColor(GameObject obj)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = originalColor;
            }
        }
    }
}