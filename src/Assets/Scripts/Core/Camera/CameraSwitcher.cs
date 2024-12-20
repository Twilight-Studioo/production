﻿#region

using Cinemachine;
using UnityEngine;

#endregion

namespace Core.Camera
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase mainCamera;
        [SerializeField] private CinemachineVirtualCameraBase swapCamera;

        public void UseSwapCamera(bool isSwap)
        {
            if (isSwap)
            {
                mainCamera.Priority = 0;
                swapCamera.Priority = 1;
            }
            else
            {
                mainCamera.Priority = 1;
                swapCamera.Priority = 0;
            }
        }
    }
}